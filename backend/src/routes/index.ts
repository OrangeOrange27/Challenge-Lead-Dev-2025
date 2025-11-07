import { Router } from "express";
import authController from "../controllers/AuthController";
import minigamesController from "../controllers/MinigamesController";
import matchesController from "../controllers/MatchesController";
import playerController from "../controllers/PlayerController";

const router = Router();

router.use("/auth", authController);
router.use("/games", minigamesController);
router.use("/matches", matchesController);
router.use("/player", playerController);

export default router;