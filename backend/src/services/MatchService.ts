import { GameMode, Match } from "../models/entities";
import { ParticipationsStorage } from "../storages/ParticipationsStorage";
import { MinigamesStorage } from "../storages/MinigamesStorage";
import { GameModesStorage } from "../storages/GameModesStorage";
import { MatchStorage } from "../storages/MatchStorage";
import {GameModeType, MatchState} from "../models/enums";

export class MatchService {
  private matches = new MatchStorage();
  private participations = new ParticipationsStorage();
  private games = new MinigamesStorage();
  private modes = new GameModesStorage();
  
  async joinMatch(
      gameId: string,
      playerId: string,
      modeType: GameModeType
  ): Promise<{ match: Match; participants: number }> {
    const game = await this.ensureGameExists(gameId);
    const modeConfig = await this.ensureModeExists(modeType);
    
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
    
    await this.participations.create(match.id, playerId);
    console.log(`[MatchManager] Player ${playerId} joined match ${match.id}`);
    
    //todo: create mock players

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

  private async createMatch(gameId: string, modeConfig: GameMode) {
    return this.matches.create(gameId, modeConfig);
  }

  private ensureMatchOngoing(match: Match) {
    if (match.matchState === MatchState.COMPLETE)
      throw new Error("Match already completed");
  }
}