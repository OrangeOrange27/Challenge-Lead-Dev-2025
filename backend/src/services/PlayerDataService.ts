import { CurrencyType, TransactionType } from '../models/enums';
import { Transaction } from '../models/entities';
import {PlayersStorage} from "../storages/PlayersStorage";
import {TransactionsStorage} from "../storages/TransactionsStorage";
import {RewardModel} from "../models/DTOs/playerBalance";

export class PlayerDataService {
  private players: PlayersStorage;
  private transactions: TransactionsStorage;

  constructor() {
    this.players = new PlayersStorage();
    this.transactions = new TransactionsStorage();
  }

  async canAfford(playerId: string, amount: number, currencyType: CurrencyType): Promise<boolean> {
    if (currencyType === CurrencyType.NONE || amount === 0) return true;

    const player = await this.players.getById(playerId);
    if (!player) throw new Error('Player not found');

    const balance = currencyType === CurrencyType.SOFT ? player.softCurrency : player.hardCurrency;
    return balance >= amount;
  }

  async getBalance(playerId: string): Promise<{ softCurrency: number; hardCurrency: number }> {
    const player = await this.players.getById(playerId);
    if (!player) throw new Error('Player not found');

    return {
      softCurrency: player.softCurrency,
      hardCurrency: player.hardCurrency,
    };
  }

  async deductFee(
      playerId: string,
      amount: number,
      currencyType: CurrencyType,
      matchId?: string
  ): Promise<Transaction> {
    if (currencyType === CurrencyType.NONE || amount === 0) {
      return this.transactions.create(
          playerId,
          {
            CurrencyType: currencyType,
            Amount: amount
          },
          TransactionType.REWARD_CLAIM,
          matchId
      );
    }

    const canAfford = await this.canAfford(playerId, amount, currencyType);
    if (!canAfford) {
      const name = currencyType === CurrencyType.SOFT ? 'soft currency' : 'hard currency';
      throw new Error(`Insufficient ${name}`);
    }

    await this.players.changeCurrency(playerId, -amount, currencyType);

    const player = await this.players.getById(playerId);
    if (!player) throw new Error('Player not found after update');

    const fee: RewardModel = {
      CurrencyType: currencyType,
      Amount: -amount
    };

    return this.transactions.create(
        playerId,
        fee,
        TransactionType.ENTRY_FEE,
        matchId
    );
  }

  async addReward(
      playerId: string,
      reward: RewardModel,
      matchId?: string
  ): Promise<Transaction> {
    if (reward.Amount === 0) {
      const player = await this.players.getById(playerId);
      if (!player) throw new Error('Player not found');

      return this.transactions.create(
          playerId,
          reward,
          TransactionType.REWARD_CLAIM,
          matchId
      );
    }

    await this.players.changeCurrency(playerId, reward.Amount, reward.CurrencyType);

    const player = await this.players.getById(playerId);
    if (!player) throw new Error('Player not found after update');

    return this.transactions.create(
        playerId,
        reward,
        TransactionType.REWARD_CLAIM,
        matchId
    );
  }

  async getTransactionHistory(playerId: string, limit: number = 50): Promise<Transaction[]> {
    return this.transactions.getPlayerTransactions(playerId, limit);
  }
}