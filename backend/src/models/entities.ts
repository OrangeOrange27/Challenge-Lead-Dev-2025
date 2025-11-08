import { MatchState, GameModeType, TransactionType } from "./enums";
import {RewardModel} from "./DTOs/playerBalance";

export interface Player {
  id: string;
  deviceId: string;
  playerName?: string;
  softCurrency: number;
  hardCurrency: number;
  createdAt: string;
}

export interface Game {
  id: string;
  name: string;
  iconId?: string;
  modes: GameMode[];
}

export interface GameMode {
  type: GameModeType;
  displayName: string;
  maxPlayers: number;
  entryFee: RewardModel;
  prizes: RewardModel[];
}

export interface Match {
  id: string;
  gameId: string;
  mode: GameMode;
  matchState: MatchState;
  participantsCount: number;
  createdAt: string;
  finalizedAt: string | null;
}

export interface Participation {
  id: string;
  matchId: string;
  playerId: string;
  playerName?: string;
  score: number | null;
  submittedAt: string | null;
  createdAt: string;
}

export interface MatchReward {
  id: string;
  matchId: string;
  playerId: string;
  reward: RewardModel;
  canClaim: boolean;
  claimedAt: string | null;
}

export interface Transaction {
  id: string;
  playerId: string;
  reward: RewardModel;
  transactionType: TransactionType;
  matchId: string | null;
  createdAt: string;
}