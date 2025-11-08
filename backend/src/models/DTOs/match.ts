import { MatchState, GameModeType } from "../enums";
import {RewardModel} from "./playerBalance";
import {GameMode} from "../entities";

export interface EnterMatchRequest {
  gameId: string;
  playerId: string;
  mode: GameModeType;
}

export interface EnterMatchResponse {
  matchId?: string;
  mode?: GameMode;
  matchState?: MatchState;
  currentPlayers?: number;
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