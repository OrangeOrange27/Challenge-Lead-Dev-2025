import { CurrencyType, TransactionType } from '../enums';

export interface RewardModel {
  CurrencyType: CurrencyType;
  Amount: number;
}

export interface TransactionDto {
  id: string;
  amount: number;
  currencyType: CurrencyType;
  transactionType: TransactionType;
  matchId: string | null;
  description: string;
  balanceAfter: number;
  createdAt: string;
}

export interface GetBalanceResponse {
  success: boolean;
  softCurrency?: number;
  hardCurrency?: number;
  error?: string;
}

export interface GetTransactionsResponse {
  success: boolean;
  transactions?: TransactionDto[];
  error?: string;
}