import {MatchStorage} from "../storages/MatchStorage";
import {ParticipationsStorage} from "../storages/ParticipationsStorage";
import {RewardStorage} from "../storages/RewardStorage";
import {MinigamesStorage} from "../storages/MinigamesStorage";
import {GroupedPlayerHistory, MatchHistoryItem} from "../models/DTOs/player";
import {MatchState} from "../models/enums";

export class PlayerHistoryService {
  private matches = new MatchStorage();
  private participations = new ParticipationsStorage();
  private rewards = new RewardStorage();
  private minigames = new MinigamesStorage();

  async getGroupedPlayerHistory(
      playerId: string,
      limit: number = 50
  ): Promise<GroupedPlayerHistory> {
    const entries = (await this.participations.getByPlayer(playerId)).slice(0, limit);
    const historyItems: MatchHistoryItem[] = [];

    for (const entry of entries) {
      const match = await this.matches.getById(entry.matchId);
      if (!match) continue;

      const game = await this.minigames.getById(match.gameId);
      if (!game) continue;

      const allEntries = await this.participations.getByMatch(entry.matchId);

      const reward = await this.rewards.getByMatchAndPlayer(entry.matchId, playerId);
      const referenceDate = new Date(match.finalizedAt || match.createdAt);
      const timeAgo = this.getTimeAgo(referenceDate);

      historyItems.push({
        matchId: entry.matchId,
        gameName: game.name,
        gameId: game.id,
        mode: match.mode.type,
        entryFee: match.mode.entryFee,
        score: entry.score,
        reward: reward?.reward || null,
        totalPlayers: allEntries.length,
        createdAt: match.createdAt,
        finalizedAt: match.finalizedAt,
        timeAgo,
        gameState: match.matchState,
        rewardClaimed: reward ? reward.claimedAt !== null : false,
      });
    }

    const rewardsToClaim = historyItems.filter(item =>
        item.gameState === MatchState.COMPLETE && item.reward?.Amount && !item.rewardClaimed
    );

    const pendingMatches = historyItems.filter(item =>
        item.gameState === MatchState.OPEN || item.gameState === MatchState.RUNNING
    );

    const pastMatches = historyItems.filter(item =>
        !rewardsToClaim.includes(item) && !pendingMatches.includes(item)
    );

    const sortByRecent = (a: MatchHistoryItem, b: MatchHistoryItem) =>
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();

    return {
      rewardsToClaim: rewardsToClaim.sort(sortByRecent),
      pendingMatches: pendingMatches.sort(sortByRecent),
      pastMatches: pastMatches.sort(sortByRecent),
    };
  }

  async getPlayerHistory(playerId: string, limit: number = 10): Promise<MatchHistoryItem[]> {
    const grouped = await this.getGroupedPlayerHistory(playerId, limit);
    return [...grouped.rewardsToClaim, ...grouped.pendingMatches, ...grouped.pastMatches];
  }

  async getPlayerBestScoreForGame(playerId: string, gameId: string) {
    const game = await this.minigames.getById(gameId);
    if (!game) return {bestScore: null, runCount: 0, updatedAt: null};

    const entries = await this.participations.getByPlayer(playerId);
    const gameEntries = [];
    for (const entry of entries) {
      const match = await this.matches.getById(entry.matchId);
      if (match?.gameId === gameId && entry.score !== null) gameEntries.push(entry);
    }

    if (!gameEntries.length) return {bestScore: null, runCount: 0, updatedAt: null};

    let bestEntry = gameEntries[0];
    for (const entry of gameEntries) {
      if (entry.score! > bestEntry.score!) bestEntry = entry;
    }

    return {
      bestScore: bestEntry.score,
      runCount: gameEntries.length,
      updatedAt: bestEntry.submittedAt || null,
    };
  }

  private getTimeAgo(date: Date): string {
    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (seconds < 60) return `${seconds}s ago`;
    const minutes = Math.floor(seconds / 60);
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  }
}