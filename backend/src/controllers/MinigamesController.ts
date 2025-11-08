  import { Router, Request, Response, NextFunction } from "express";
  import { GameService } from "../services/GameService";
  import { GameModesStorage } from "../storages/GameModesStorage";
  import { PlayersStorage } from "../storages/PlayersStorage";
  import { authMiddleware } from "../middleware/authMiddleware";
  import { GetGamesResponse, MinigameDto } from "../models/DTOs/minigame";
  import {ResponseModel} from "../models/responseModel";
  
  const router = Router();
  const gameService = new GameService();
  const gameModes = new GameModesStorage();
  const players = new PlayersStorage();
  
  // GET /api/minigames
  router.get(
    "/",
    authMiddleware,
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const playerId = req.user!.playerId;
        const player = await players.getById(playerId);
        if (!player) {
          res.status(404).json({
            success: false,
            error: "Player not found",
          });
          return;
        }
  
        const games = await gameService.getAllGames();
        const gameDtos: MinigameDto[] = games.map((game) =>
          gameService.gameToDto(game)
        );
  
        const response: ResponseModel<GetGamesResponse> = {
          success: true,
          data: {
            games: gameDtos,
          },
        };
  
        res.json(response);
      } catch (error) {
        next(error);
      }
    }
  );
  
  // GET /api/minigames/modes
  router.get(
    "/modes/all",
    authMiddleware,
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const playerId = req.user!.playerId;
        
        console.log(`Fetching game modes for player: ${playerId}`);
  
        const availableGameModes = await gameModes.getAvailableModes();
  
        res.json({
          success: true,
          modes: availableGameModes,
        });
      } catch (error) {
        next(error);
      }
    }
  );
  
  export default router;
