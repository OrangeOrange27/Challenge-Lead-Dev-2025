import {MinigamesStorage} from "../storages/MinigamesStorage";
import {Game} from "../models/entities";
import {MinigameDto} from "../models/DTOs/minigame";

export class GameService {
  private gameRepository: MinigamesStorage;

  constructor() {
    this.gameRepository = new MinigamesStorage();
  }

  async getAllGames(): Promise<Game[]> {
    return this.gameRepository.getAll();
  }

  async getAvailableGames(): Promise<Game[]> {
    return this.gameRepository.getAvailableGames();
  }

  async getGameById(gameId: string): Promise<Game> {
    const game = await this.gameRepository.getById(gameId);
    if (!game) {
      throw new Error("Game not found");
    }
    return game;
  }

  gameToDto(game: Game): MinigameDto {
    return {
      id: game.id,
      name: game.name,
      iconId: game.iconId,
    };
  }
}