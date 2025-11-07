import { v4 as uuidv4 } from "uuid";
import { getOne, runQuery, getAll } from "../config/database";
import { Transaction } from "../models/entities";
import { TransactionType } from "../models/enums";
import {RewardModel} from "../models/DTOs/playerBalance";

export class TransactionsStorage {
    async create(
        playerId: string,
        reward: RewardModel,
        transactionType: TransactionType,
        matchId?: string
    ): Promise<Transaction> {
        const id = uuidv4();
        const createdAt = new Date().toISOString();

        await runQuery(
            `INSERT INTO transactions
             (id, player_id, amount, currency_type, transaction_type, match_id, created_at)
             VALUES (?, ?, ?, ?, ?, ?, ?, ?)`,
            [
                id,
                playerId,
                reward.Amount,
                reward.CurrencyType,
                transactionType,
                matchId || null,
                createdAt,
            ]
        );

        return {
            id,
            playerId,
            reward,
            transactionType,
            matchId: matchId || null,
            createdAt,
        };
    }

    async getPlayerTransactions(
        playerId: string,
        limit: number = 50
    ): Promise<Transaction[]> {
        const rows = await getAll<any>(
            `SELECT *
             FROM transactions
             WHERE player_id = ?
             ORDER BY created_at DESC LIMIT ?`,
            [playerId, limit]
        );

        return rows.map((row) => this.mapRowToTransaction(row));
    }

    async getTransactionsByMatch(matchId: string): Promise<Transaction[]> {
        const rows = await getAll<any>(
            `SELECT *
             FROM transactions
             WHERE match_id = ?
             ORDER BY created_at ASC`,
            [matchId]
        );

        return rows.map((row) => this.mapRowToTransaction(row));
    }

    async getById(id: string): Promise<Transaction | undefined> {
        const row = await getOne<any>(
            "SELECT * FROM transactions WHERE id = ?",
            [id]
        );
        if (!row) return undefined;
        return this.mapRowToTransaction(row);
    }

    private mapRowToTransaction(row: any): Transaction {
        return {
            id: row.id,
            playerId: row.player_id,
            reward: row.reward,
            transactionType: row.transaction_type as TransactionType,
            matchId: row.match_id || null,
            createdAt: row.created_at,
        };
    }
}