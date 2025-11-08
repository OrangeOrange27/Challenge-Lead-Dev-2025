import { Router, Request, Response, NextFunction } from "express";
import { LeaderboardService } from "../services/LeaderboardService";
import { RewardService } from "../services/RewardService";
import { PlayerDataService } from "../services/PlayerDataService";
import { authMiddleware } from "../middleware/authMiddleware";
import {
  EnterMatchRequest,
  EnterMatchResponse,
  SubmitScoreRequest,
  SubmitScoreResponse,
  GetLeaderboardResponse,
  ClaimRewardResponse,
} from "../models/DTOs/match";
import { MatchService } from "../services/MatchService";
import { validateBody } from "../middleware/requestValidation";
import {ResponseModel} from "../models/responseModel";

const router = Router();
const matchService = new MatchService();
const leaderboardService = new LeaderboardService();
const rewardService = new RewardService();
const playerDataService = new PlayerDataService();

// POST /api/matches/enter
router.post(
    "/enter",
    authMiddleware,
    validateBody(["gameId", "mode"]),
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const { gameId, mode } = req.body as EnterMatchRequest;
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

        const result = await matchService.joinMatch(gameId, playerId, mode);

        const response: ResponseModel<EnterMatchResponse> = {
          success: true,
          data: {
            matchId: result.match.id,
            matchState: result.match.matchState,
            currentPlayers: result.participants,
            mode: result.match.mode,
          }
        };

        res.status(200).json(response);
      } catch (err) {
        next(err);
      }
    }
);

// POST /api/matches/:id/submit
router.post(
    "/:id/submit",
    authMiddleware,
    validateBody(["score"]),
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const { id: matchId } = req.params;
        const { score } = req.body as SubmitScoreRequest;
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

        await matchService.submitMatchScore(matchId, playerId, score);

        const response: SubmitScoreResponse = { success: true };
        res.status(200).json(response);
      } catch (err) {
        next(err);
      }
    }
);

// GET /api/matches/:id/leaderboard
router.get(
    "/:id/leaderboard",
    authMiddleware,
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const { id: matchId } = req.params;
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

        const { match } = await matchService.fetchMatch(matchId);
        const leaderboard = await leaderboardService.getLeaderboard(matchId);

        const response: ResponseModel<GetLeaderboardResponse> = {
          success: true,
          data: {
            matchId: match.id,
            matchState: match.matchState,
            entries: leaderboard.entries,
            playerEntry: leaderboard.entries.find(e => e.playerId === playerId),
          }
        };

        res.status(200).json(response);
      } catch (err) {
        next(err);
      }
    }
);

// POST /api/matches/:id/claim
router.post(
    "/:id/claim",
    authMiddleware,
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
      try {
        const { id: matchId } = req.params;
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

        const result = await rewardService.claimReward(matchId, playerId);
        const newBalance = await playerDataService.getBalance(playerId);

        const response: ResponseModel<ClaimRewardResponse> = {
          success: true,
          data: {
            reward: result.reward.reward,
            newBalance: newBalance,
          }
        };

        res.status(200).json(response);
      } catch (err) {
        next(err);
      }
    }
);

export default router;