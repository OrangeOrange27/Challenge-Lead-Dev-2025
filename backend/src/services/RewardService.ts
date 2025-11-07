import { PlayerDataService } from "./PlayerDataService";
import { MatchReward } from "../models/entities";
import { MatchState } from "../models/enums";
import {GameModesStorage} from "../storages/GameModesStorage";
import {MatchStorage} from "../storages/MatchStorage";
import {RewardStorage} from "../storages/RewardStorage";
import {LeaderboardService} from "./LeaderboardService";

export class RewardService {
  private rewards = new RewardStorage();
  private matches = new MatchStorage();
  private gameModes = new GameModesStorage();

  private leaderboardService = new LeaderboardService();
  private playerDataService = new PlayerDataService();

  async assignRewards(matchId: string): Promise<void> {
    const match = await this.matches.getById(matchId);
    if (!match) throw new Error("Match not found");
    if (match.matchState !== MatchState.COMPLETE) throw new Error("Match must be complete to assign rewards");

    const modeConfig = await this.gameModes.getByMode(match.mode.type);
    if (!modeConfig) throw new Error("Mode configuration not found");

    const {entries} = await this.leaderboardService.getLeaderboard(matchId);

    for (let i = 0; i < modeConfig.prizes.length; i++) {
      const entry = entries[i];

      const existing = await this.rewards.getByMatchAndPlayer(matchId, entry.playerId);
      if (existing) continue;

      const reward = modeConfig.prizes[i];

      await this.rewards.create(matchId, entry.playerId, reward);
      console.log(`[RewardManager] Created reward for player ${entry.playerId}: ${reward.CurrencyType} ${reward.Amount}`);
    }
  }

  async claimReward(matchId: string, playerId: string): Promise<{ reward: MatchReward; }> {
    const match = await this.matches.getById(matchId);
    if (!match) throw new Error("Match not found");
    if (match.matchState !== MatchState.COMPLETE) throw new Error("Match must be complete to claim reward");

    const reward = await this.rewards.getByMatchAndPlayer(matchId, playerId);
    if (!reward) throw new Error("Reward not found");
    if (!reward.canClaim) throw new Error("Reward is not claimable");
    if (reward.claimedAt) throw new Error("Reward already claimed");

    await this.playerDataService.addReward(playerId, reward.reward, matchId);
    await this.rewards.markClaimed(reward.id);
    console.log(`[RewardManager] Player ${playerId} claimed reward for match ${matchId}`);

    return {
      reward: {...reward, claimedAt: new Date().toISOString()}
    };
  }
}