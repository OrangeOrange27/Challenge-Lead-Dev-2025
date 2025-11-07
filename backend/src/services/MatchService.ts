import { PlayerDataService } from "./PlayerDataService";
import { GameMode, Match } from "../models/entities";
import { MatchState, CurrencyType, GameModeType } from "../models/enums";
import { ParticipationsStorage } from "../storages/ParticipationsStorage";
import { MinigamesStorage } from "../storages/MinigamesStorage";
import { GameModesStorage } from "../storages/GameModesStorage";
import { PlayersStorage } from "../storages/PlayersStorage";
import { MatchStorage } from "../storages/MatchStorage";

export class MatchService {
  private matches = new MatchStorage();
  private participations = new ParticipationsStorage();
  private games = new MinigamesStorage();
  private modes = new GameModesStorage();
  private players = new PlayersStorage();

  private playerDataService = new PlayerDataService();

  async joinMatch(
      gameId: string,
      playerId: string,
      modeType: GameModeType
  ): Promise<{ match: Match; participants: number }> {
    const game = await this.ensureGameExists(gameId);
    const player = await this.ensurePlayerExists(playerId);
    const modeConfig = await this.ensureModeExists(modeType);

    await this.validateEntryFee(player, modeConfig);

    let match = await this.matches.getOpenMatch(gameId, modeType);
    if (!match) {
      match = await this.createMatch(gameId, modeConfig);
      console.log(`[MatchManager] New match ${match.id} created for ${game.name}`);
    }

    const existingEntry = await this.participations.getByMatchAndPlayer(match.id, playerId);
    if (existingEntry) {
      const participants = await this.matches.getParticipantCount(match.id);
      return {match, participants};
    }

    await this.checkMatchCapacity(match);
    this.checkEntryDeadline(match);

    await this.processEntryFee(playerId, match, modeConfig);
    await this.participations.create(match.id, playerId);
    console.log(`[MatchManager] Player ${playerId} joined match ${match.id}`);

    if (match.matchState === MatchState.OPEN) {
      await this.matches.updateState(match.id, MatchState.RUNNING);
      match.matchState = MatchState.RUNNING;
    }

    match = (await this.matches.getById(match.id))!;
    const participants = await this.matches.getParticipantCount(match.id);
    return {match, participants};
  }

  async submitMatchScore(matchId: string, playerId: string, score: number): Promise<void> {
    const match = await this.requireMatch(matchId);
    this.ensureMatchOngoing(match);
    this.ensureScoreWindowOpen(match);

    const entry = await this.participations.getByMatchAndPlayer(matchId, playerId);
    if (!entry) throw new Error("Player has not joined the match");
    if (entry.score !== null) throw new Error("Score already recorded");
    if (score < 0) throw new Error("Score cannot be negative");

    await this.participations.updateScore(entry.id, score);
    console.log(`[MatchManager] Player ${playerId} submitted score ${score} for match ${matchId}`);
  }

  async fetchMatch(matchId: string): Promise<{ match: Match; participants: number }> {
    const match = await this.requireMatch(matchId);
    const participants = await this.matches.getParticipantCount(matchId);
    return {match, participants};
  }

  // ---- Private helpers ----

  private async ensureGameExists(gameId: string) {
    const game = await this.games.getById(gameId);
    if (!game) throw new Error("Game not found");
    return game;
  }

  private async ensurePlayerExists(playerId: string) {
    const player = await this.players.getById(playerId);
    if (!player) throw new Error("Player not found");
    return player;
  }

  private async ensureModeExists(modeType: GameModeType) {
    const modeConfig = await this.modes.getByMode(modeType);
    if (!modeConfig) throw new Error("Invalid mode");
    return modeConfig;
  }

  private async requireMatch(matchId: string) {
    const match = await this.matches.getById(matchId);
    if (!match) throw new Error("Match not found");
    return match;
  }

  private async validateEntryFee(player: any, modeConfig: GameMode) {
    if (!modeConfig.entryFee) return;

    const fee = modeConfig.entryFee;
    if (fee.Amount <= 0 || fee.CurrencyType === CurrencyType.NONE) return;

    const canPay = await this.playerDataService.canAfford(player.id, fee.Amount, fee.CurrencyType);
    if (!canPay) {
      const balance =
          fee.CurrencyType === CurrencyType.SOFT ? player.softCurrency : player.hardCurrency;
      const name = fee.CurrencyType === CurrencyType.SOFT ? "soft currency" : "hard currency";
      throw new Error(`Not enough ${name}. Need ${fee.Amount}, have ${balance}`);
    }
  }

  private async createMatch(gameId: string, modeConfig: GameMode) {
    return this.matches.create(gameId, modeConfig);
  }

  private async checkMatchCapacity(match: Match) {
    const count = await this.matches.getParticipantCount(match.id);
    if (count >= match.mode.maxPlayers) throw new Error("Match is full");
  }

  private checkEntryDeadline(match: Match) {
    const now = new Date();
    const deadline = new Date(match.createdAt);
    if (now > deadline) throw new Error("Entry period expired");
  }

  private ensureScoreWindowOpen(match: Match) {
    const now = new Date();
    const deadline = new Date(match.finalizedAt ?? match.createdAt);
    if (now > deadline) throw new Error("Score submission closed");
  }

  private ensureMatchOngoing(match: Match) {
    if (match.matchState === MatchState.COMPLETE)
      throw new Error("Match already completed");
  }

  private async processEntryFee(playerId: string, match: Match, modeConfig: GameMode) {
    if (!modeConfig.entryFee) return;

    const fee = modeConfig.entryFee;
    if (fee.Amount <= 0 || fee.CurrencyType === CurrencyType.NONE) return;

    await this.playerDataService.deductFee(
        playerId,
        fee.Amount,
        fee.CurrencyType,
        match.id
    );
  }
}