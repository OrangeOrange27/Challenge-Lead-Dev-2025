import { v4 as uuidv4 } from "uuid";
import { getOne, runQuery, getAll } from "../config/database";
import {GameMode, Match} from "../models/entities";
import { MatchState } from "../models/enums";

export class MatchStorage {
    async create(gameId: string, mode: GameMode): Promise<Match> {
        const id = uuidv4();
        const match_state = MatchState.OPEN;
        const createdAt = new Date().toISOString();
        const participantsCount = 0;

        await runQuery(
            `INSERT INTO matches (id, game_id, mode, match_state, participants_count, created_at)
             VALUES (?, ?, ?, ?, ?, ?)`,
            [
                id,
                gameId,
                mode.type,
                match_state,
                participantsCount,
                createdAt,
            ]
        );

        return {
            id,
            gameId,
            mode: mode as any,
            matchState: match_state,
            participantsCount: participantsCount,
            createdAt,
            finalizedAt: null,
        };
    }

    async getById(id: string): Promise<Match | undefined> {
        const row = await getOne<any>("SELECT * FROM matches WHERE id = ?", [id]);
        if (!row) return undefined;

        return this.mapRowToMatch(row);
    }

    async getOpenMatch(
        gameId: string,
        mode: string
    ): Promise<Match | undefined> {
        const row = await getOne<any>(
            `
                SELECT m.*, gm.max_players, COUNT(p.id) AS participants_count
                FROM matches m
                         LEFT JOIN participations p ON m.id = p.match_id
                         JOIN gamemodes gm ON m.mode = gm.mode
                WHERE m.game_id = ?
                  AND m.mode = ?
                  AND m.match_state = ?
                GROUP BY m.id
                HAVING participants_count < gm.max_players
                ORDER BY m.created_at DESC LIMIT 1
            `,
            [gameId, mode, MatchState.OPEN]
        );

        if (!row) return undefined;
        return this.mapRowToMatch(row);
    }

    async getMatchesToFinalize(): Promise<Match[]> {
        const rows = await getAll<any>(
            `SELECT m.*, COUNT(p.id) AS participants_count
             FROM matches m
                      LEFT JOIN participations p ON m.id = p.match_id
             WHERE m.match_state IN (?, ?)
             GROUP BY m.id
             HAVING participants_count > 0`,
            [MatchState.OPEN, MatchState.RUNNING]
        );

        return rows.map((row) => this.mapRowToMatch(row));
    }

    async updateState(
        id: string,
        match_state: MatchState,
        finalizedAt?: string
    ): Promise<void> {
        if (finalizedAt) {
            await runQuery(
                "UPDATE matches SET match_state = ?, finalized_at = ? WHERE id = ?",
                [match_state, finalizedAt, id]
            );
        } else {
            await runQuery("UPDATE matches SET match_state = ? WHERE id = ?", [match_state, id]);
        }
    }

    async getParticipantCount(matchId: string): Promise<number> {
        const result = await getOne<{ count: number }>(
            "SELECT COUNT(*) as count FROM participations WHERE match_id = ?",
            [matchId]
        );
        return result?.count || 0;
    }

    private mapRowToMatch(row: any): Match {
        return {
            id: row.id,
            gameId: row.game_id,
            mode: row.mode,
            matchState: row.match_state,
            participantsCount: row.participants_count || 0,
            createdAt: row.created_at,
            finalizedAt: row.finalized_at || null,
        };
    }
}