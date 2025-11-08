import { v4 as uuidv4 } from "uuid";
import { getOne, runQuery, getAll } from "../config/database";
import { Participation } from "../models/entities";

export class ParticipationsStorage {
  async create(matchId: string, playerId: string, score: number | null = null): Promise<Participation> {
    const id = uuidv4();
    const createdAt = new Date().toISOString();

    await runQuery(
        "INSERT INTO participations (id, match_id, player_id, created_at) VALUES (?, ?, ?, ?)",
        [id, matchId, playerId, createdAt]
    );

    return {
      id,
      matchId,
      playerId,
      score,
      submittedAt: null,
      createdAt,
    };
  }

  async getByMatchAndPlayer(
      matchId: string,
      playerId: string
  ): Promise<Participation | undefined> {
    const row = await getOne<any>(
        "SELECT * FROM participations WHERE match_id = ? AND player_id = ?",
        [matchId, playerId]
    );

    if (!row) return undefined;
    return this.mapRowToParticipation(row);
  }

  async getByMatch(matchId: string): Promise<Participation[]> {
    const rows = await getAll<any>(
        "SELECT * FROM participations WHERE match_id = ? ORDER BY score DESC",
        [matchId]
    );

    return rows.map((row) => this.mapRowToParticipation(row));
  }

  async updateScore(id: string, score: number): Promise<void> {
    const submittedAt = new Date().toISOString();
    await runQuery(
        "UPDATE participations SET score = ?, submitted_at = ? WHERE id = ?",
        [score, submittedAt, id]
    );
  }

  async getById(id: string): Promise<Participation | undefined> {
    const row = await getOne<any>("SELECT * FROM participations WHERE id = ?", [id]);
    if (!row) return undefined;
    return this.mapRowToParticipation(row);
  }

  async getByPlayer(playerId: string): Promise<Participation[]> {
    const rows = await getAll<any>(
        "SELECT * FROM participations WHERE player_id = ? ORDER BY created_at DESC",
        [playerId]
    );

    return rows.map((row) => this.mapRowToParticipation(row));
  }

  private mapRowToParticipation(row: any): Participation {
    return {
      id: row.id,
      matchId: row.match_id,
      playerId: row.player_id,
      score: row.score,
      submittedAt: row.submitted_at,
      createdAt: row.created_at,
    };
  }
}