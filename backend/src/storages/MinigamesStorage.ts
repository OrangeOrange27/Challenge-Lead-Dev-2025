import { getOne, getAll } from "../config/database";
import { Game } from "../models/entities";

export class MinigamesStorage {
  async getAll(): Promise<Game[]> {
    const rows = await getAll<any>("SELECT * FROM minigames");
    return rows.map((row) => this.mapRowToGame(row));
  }

  async getById(id: string): Promise<Game | undefined> {
    const row = await getOne<any>("SELECT * FROM minigames WHERE id = ?", [id]);
    if (!row) return undefined;
    return this.mapRowToGame(row);
  }

  async getAvailableGames(): Promise<Game[]> {
    const rows = await getAll<any>("SELECT * FROM minigames");
    return rows.map((row) => this.mapRowToGame(row));
  }

  private mapRowToGame(row: any): Game {
    return {
      id: row.id,
      name: row.game_name,
      iconId: row.icon_id,
    };
  }
}