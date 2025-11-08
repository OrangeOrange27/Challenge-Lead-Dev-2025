import { getOne, getAll } from "../config/database";
import { Game, GameMode } from "../models/entities";
import {GameModeType} from "../models/enums";

export class MinigamesStorage {

  async getAll(): Promise<Game[]> {
    const minigames = await getAll<any>("SELECT * FROM minigames");
    const games = await Promise.all(minigames.map(row => this.getById(row.id)));
    return games.filter((g): g is Game => g !== undefined);
  }

  async getById(id: string): Promise<Game | undefined> {
    const row = await getOne<any>("SELECT * FROM minigames WHERE id = ?", [id]);
    if (!row) return undefined;

    const modes = await this.getModesForGame(id);
    return this.mapRowToGame(row, modes);
  }

  async getAvailableGames(): Promise<Game[]> {
    return this.getAll();
  }


  private async getModesForGame(minigameId: string): Promise<GameMode[]> {
    const rows = await getAll<any>(
        `SELECT g.*
         FROM gamemodes g
                JOIN minigame_modes mm ON g.id = mm.gamemode_id
         WHERE mm.minigame_id = ?`,
        [minigameId]
    );

    return rows.map(row => ({
      id: row.id,
      mode: row.mode,
      type: row.mode as GameModeType,
      maxPlayers: row.max_players,
      displayName: row.display_name,
      entryFee: JSON.parse(row.entry_fee_json),
      prizes: JSON.parse(row.prizes_json),
    }));
  }

  private mapRowToGame(row: any, modes: GameMode[]): Game {
    return {
      id: row.id,
      name: row.game_name,
      iconId: row.icon_id,
      modes,
    };
  }
}