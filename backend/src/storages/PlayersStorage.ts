import { v4 as uuidv4 } from "uuid";
import { getOne, runQuery } from "../config/database";
import { Player } from "../models/entities";
import { CurrencyType } from "../models/enums";

interface PlayerRow {
    id: string;
    device_id: string;
    player_name: string | null;
    soft_currency: number;
    hard_currency: number;
    created_at: string;
}

export class PlayersStorage {
    async getByDevice(deviceId: string): Promise<Player | undefined> {
        const row = await getOne<PlayerRow>(
            "SELECT * FROM players WHERE device_id = ?",
            [deviceId]
        );
        return row ? this.mapRowToPlayer(row) : undefined;
    }

    async getById(id: string): Promise<Player | undefined> {
        const row = await getOne<PlayerRow>(
            "SELECT * FROM players WHERE id = ?",
            [id]
        );
        return row ? this.mapRowToPlayer(row) : undefined;
    }

    async getBalance(playerId: string, type: CurrencyType): Promise<number> {
        const player = await this.getById(playerId);
        if (!player) throw new Error("Player not found");

        return type === CurrencyType.SOFT ? player.softCurrency : player.hardCurrency;
    }

    async registerNew(deviceId: string): Promise<Player> {
        const id = uuidv4();
        const createdAt = new Date().toISOString();
        const playerName = `Player_${Math.random().toString(36).substring(2, 10)}`;

        const player: Player = {
            id,
            deviceId,
            playerName,
            softCurrency: 1000,
            hardCurrency: 0,
            createdAt,
        };

        await runQuery(
            `INSERT INTO players
                 (id, device_id, player_name, soft_currency, hard_currency, created_at)
             VALUES (?, ?, ?, ?, ?, ?)`,
            [
                id,
                deviceId,
                playerName,
                player.softCurrency,
                player.hardCurrency,
                createdAt,
            ]
        );

        return player;
    }

    async changeCurrency(
        playerId: string,
        amount: number,
        type: CurrencyType
    ): Promise<void> {
        const column = type === CurrencyType.SOFT ? "soft_currency" : "hard_currency";

        const row = await getOne<{ [key: string]: number }>(
            `SELECT ${column}
             FROM players
             WHERE id = ?`,
            [playerId]
        );
        if (!row) throw new Error("Player not found");

        const newBalance = Math.max(0, (row[column] ?? 0) + amount);

        await runQuery(
            `UPDATE players
             SET ${column} = ?
             WHERE id = ?`,
            [newBalance, playerId]
        );
    }

    private mapRowToPlayer(row: PlayerRow): Player {
        return {
            id: row.id,
            deviceId: row.device_id,
            playerName: row.player_name ?? `Player_${row.id.substring(0, 8)}`,
            softCurrency: row.soft_currency,
            hardCurrency: row.hard_currency,
            createdAt: row.created_at,
        };
    }
}