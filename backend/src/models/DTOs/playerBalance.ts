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
  softCurrency?: number;
  hardCurrency?: number;
}

export interface GetTransactionsResponse {
  transactions?: TransactionDto[];
}