import { ParticipationsStorage } from "../storages/ParticipationsStorage";
import { MatchStorage } from "../storages/MatchStorage";
import { MinigamesStorage } from "../storages/MinigamesStorage";
import {LeaderboardEntry} from "../models/DTOs/match";
import {Participation} from "../models/entities";

export class LeaderboardService {
  private participations = new ParticipationsStorage();
  private matches = new MatchStorage();
  private minigames = new MinigamesStorage();

  async getLeaderboard(
      matchId: string,
  ): Promise<{
    entries: LeaderboardEntry[];
  }> {
    const match = await this.matches.getById(matchId);
    if (!match) throw new Error("Match not found");

    const game = await this.minigames.getById(match.gameId);
    if (!game) throw new Error("Game not found");

    const participations = await this.participations.getByMatch(matchId);
    const scored = participations.filter((p) => p.score !== null);

    const sorted = this.sortParticipations(scored);

    return {entries: sorted};
  }

  private sortParticipations(participations: Participation[]): LeaderboardEntry[] {
    const result: LeaderboardEntry[] = [];

    const sorted = [...participations].sort((a, b) => {
      if (a.score === null && b.score === null) return 0;
      if (a.score === null) return 1;
      if (b.score === null) return -1;
      return b.score - a.score;
    });

    for (let i = 0; i < sorted.length; i++) {
      const entry = sorted[i];

      result.push({
        playerId: entry.playerId,
        playerName: entry.playerName,
        score: entry.score!,
      });
    }

    return result;
  }
}