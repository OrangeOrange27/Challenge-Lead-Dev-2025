import { getOne, getAll } from "../config/database";
import { GameMode } from "../models/entities";
import { GameModeType } from "../models/enums";

export class GameModesStorage {
  async getByMode(mode: GameModeType): Promise<GameMode | undefined> {
    const row = await getOne<any>(
        "SELECT * FROM gamemodes WHERE mode = ?",
        [mode]
    );
    if (!row) return undefined;
    return this.mapRowToGameMode(row);
  }

  async getAvailableModes(): Promise<GameMode[]> {
    
    // For now, return all modes. In the future, we can filter based on player level or other criteria.
    
    const rows = await getAll<any>(
        "SELECT * FROM gamemodes ORDER BY mode ASC"
    );
    return rows.map((row) => this.mapRowToGameMode(row));
  }

  private mapRowToGameMode(row: any): GameMode {
    return {
      type: row.mode as GameModeType,
      displayName: row.display_name,
      maxPlayers: row.max_players,
      entryFee: JSON.parse(row.entry_fee_json),
      prizes: JSON.parse(row.prizes_json),
    };
  }
}