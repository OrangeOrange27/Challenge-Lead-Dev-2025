import {NextFunction, Request, Response, Router} from "express";
import {PlayersStorage} from "../storages/PlayersStorage";
import {PlayerHistoryService} from "../services/PlayerHistoryService";
import {authMiddleware} from "../middleware/authMiddleware";
import {PlayerHistoryResponse, PlayerProfileResponse,} from "../models/DTOs/player";
import {CurrencyType} from "../models/enums";

const router = Router();
const playersStorage = new PlayersStorage();
const playerHistoryService = new PlayerHistoryService();

// GET /api/player/profile
router.get(
    "/profile",
    authMiddleware,
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const playerId = req.user?.playerId;
        if (!playerId) {
          res.status(401).json({
            success: false,
            error: {
              message: "Unauthorized: missing playerId in token",
              statusCode: 401,
              timestamp: new Date().toISOString(),
            },
          });
          return;
        }

        const player = await playersStorage.getById(playerId);
        if (!player) {
          res.status(404).json({
            success: false,
            error: {
              message: "Player not found",
              statusCode: 404,
              timestamp: new Date().toISOString(),
            },
          });
          return;
        }

        const response: PlayerProfileResponse = {
          success: true,
          profile: {
            playerId: player.id,
            playerName: player.playerName,
            softCurrency: player.softCurrency,
            hardCurrency: player.hardCurrency,
            createdAt: player.createdAt,
          },
        };

        res.status(200).json(response);
      } catch (err) {
        next(err);
      }
    }
);


// PATCH /api/player/balance
router.patch(
    "/balance",
    authMiddleware,
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const playerId = req.user?.playerId;
        if (!playerId) {
          res.status(401).json({
            success: false,
            error: {
              message: "Unauthorized: missing playerId in token",
              statusCode: 401,
              timestamp: new Date().toISOString(),
            },
          });
          return;
        }

        const { amount, type } = req.body;
        if (typeof amount !== "number" || ![CurrencyType.SOFT, CurrencyType.HARD].includes(type)) {
          res.status(400).json({
            success: false,
            error: {
              message: "Invalid amount or currency type",
              statusCode: 400,
              timestamp: new Date().toISOString(),
            },
          });
          return;
        }

        // Update balance
        await playersStorage.changeCurrency(playerId, amount, type);

        const player = await playersStorage.getById(playerId);

        res.status(200).json({
          success: true,
          playerId: player?.id,
          softCurrency: player?.softCurrency,
          hardCurrency: player?.hardCurrency,
        });
      } catch (err) {
        next(err);
      }
    }
);


// GET /api/player/history
router.get(
    "/history",
    authMiddleware,
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const playerId = req.user?.playerId;
        if (!playerId) {
          res.status(401).json({
            success: false,
            error: {
              message: "Unauthorized: missing playerId in token",
              statusCode: 401,
              timestamp: new Date().toISOString(),
            },
          });
          return;
        }

        const limit = Number(req.query.limit) || 50;
        const history = await playerHistoryService.getGroupedPlayerHistory(playerId, limit);

        const total =
            history.rewardsToClaim.length +
            history.pendingMatches.length +
            history.pastMatches.length;

        const response: PlayerHistoryResponse = {
          success: true,
          history,
          total,
        };

        res.status(200).json(response);
      } catch (err) {
        next(err);
      }
    }
);

export default router;