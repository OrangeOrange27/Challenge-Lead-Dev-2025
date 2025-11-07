import {RewardModel} from "./playerBalance";

export interface PlayerProfileResponse {
  profile?: {
    playerId: string;
    playerName?: string;
    softCurrency: number;
    hardCurrency: number;
    createdAt: string;
  };
}

export interface MatchHistoryItem {
  matchId: string;
  gameName: string;
  gameId: string;
  mode: string;
  entryFee: RewardModel;
  score: number | null;
  reward: RewardModel | null;
  totalPlayers: number;
  createdAt: string;
  finalizedAt: string | null;
  timeAgo: string;
  gameState: string;
  rewardClaimed: boolean;
}

export interface GroupedPlayerHistory {
  rewardsToClaim: MatchHistoryItem[];
  pendingMatches: MatchHistoryItem[];
  pastMatches: MatchHistoryItem[];
}

export interface PlayerHistoryResponse {
  history?: GroupedPlayerHistory;
  total?: number;
}