import { MatchState, GameModeType } from "../enums";
import {RewardModel} from "./playerBalance";

export interface EnterMatchRequest {
  gameId: string;
  playerId: string;
  mode: GameModeType;
}

export interface EnterMatchResponse {
  success: boolean;
  matchId?: string;
  mode?: GameModeType;
  matchState?: MatchState;
  maxPlayers?: number;
  currentPlayers?: number;
  entryFee?: RewardModel;
  error?: string;
}

export interface SubmitScoreRequest {
  playerId: string;
  score: number;
}

export interface SubmitScoreResponse {
  success: boolean;
  rank?: number;
  error?: string;
}

export interface LeaderboardEntry {
  playerId: string;
  playerName?: string;
  score: number;
}

export interface GetLeaderboardResponse {
  success: boolean;
  matchId?: string;
  matchState?: MatchState;
  entries?: LeaderboardEntry[];
  playerEntry?: LeaderboardEntry;
  error?: string;
}

export interface ClaimRewardRequest {
  playerId: string;
}

export interface ClaimRewardResponse {
  success: boolean;
  reward?: RewardModel;
  newBalance?: {
    softCurrency: number;
    hardCurrency: number;
  };
  error?: string;
}