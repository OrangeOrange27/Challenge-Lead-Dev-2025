import { v4 as uuidv4 } from "uuid";
import { getOne, runQuery } from "../config/database";
import { MatchReward } from "../models/entities";
import {RewardModel} from "../models/DTOs/playerBalance";

export class RewardStorage {
  async create(
      matchId: string,
      playerId: string,
      reward: RewardModel,
      canClaim = true
  ): Promise<MatchReward> {
    const id = uuidv4();
    const rewardJson = JSON.stringify(reward);

    await runQuery(
        `INSERT INTO rewards (id, match_id, player_id, reward_json, claimable)
         VALUES (?, ?, ?, ?, ?)`,
        [id, matchId, playerId, rewardJson, canClaim ? 1 : 0]
    );

    return {
      id,
      matchId,
      playerId,
      reward,
      canClaim,
      claimedAt: null,
    };
  }

  async getByMatchAndPlayer(
      matchId: string,
      playerId: string
  ): Promise<MatchReward | undefined> {
    const row = await getOne<any>(
        "SELECT * FROM rewards WHERE match_id = ? AND player_id = ?",
        [matchId, playerId]
    );
    if (!row) return undefined;
    return this.mapRowToReward(row);
  }

  async markClaimed(id: string): Promise<void> {
    const claimedAt = new Date().toISOString();
    await runQuery("UPDATE rewards SET claimable = 0, claimed_at = ? WHERE id = ?", [
      claimedAt,
      id,
    ]);
  }

  private mapRowToReward(row: any): MatchReward {
    const reward: RewardModel = JSON.parse(row.reward_json || "");
    return {
      id: row.id,
      matchId: row.match_id,
      playerId: row.player_id,
      reward,
      canClaim: row.claimable === 1,
      claimedAt: row.claimed_at || null,
    };
  }
}