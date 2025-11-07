import { MatchState, GameModeType } from "../enums";
import {RewardModel} from "./playerBalance";

export interface EnterMatchRequest {
  gameId: string;
  playerId: string;
  mode: GameModeType;
}

export interface EnterMatchResponse {
  matchId?: string;
  mode?: GameModeType;
  matchState?: MatchState;
  maxPlayers?: number;
  currentPlayers?: number;
  entryFee?: RewardModel;
}

export interface SubmitScoreRequest {
  playerId: string;
  score: number;
}

export interface SubmitScoreResponse {
}

export interface LeaderboardEntry {
  playerId: string;
  playerName?: string;
  score: number;
}

export interface GetLeaderboardResponse {
  matchId?: string;
  matchState?: MatchState;
  entries?: LeaderboardEntry[];
  playerEntry?: LeaderboardEntry;
}

export interface ClaimRewardRequest {
  playerId: string;
}

export interface ClaimRewardResponse {
  reward?: RewardModel;
  newBalance?: {
    softCurrency: number;
    hardCurrency: number;
  };
}