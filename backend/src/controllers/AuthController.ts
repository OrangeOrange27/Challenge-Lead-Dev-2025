import { Router, Request, Response, NextFunction } from "express";
import { AuthService } from "../services/AuthService";
import { validateBody } from "../middleware/requestValidation";
import { LoginRequest, LoginResponse } from "../models/DTOs/auth";

const router = Router();
const authService = new AuthService();

// POST /api/auth/login

router.post(
    "/login",
    validateBody(["deviceId"]),
    async (req: Request, res: Response, next: NextFunction): Promise<void> => {
        try {
            const { deviceId } = req.body as LoginRequest;
            if (deviceId.trim().length === 0) {
                res.status(400).json({
                    success: false,
                    error: {
                        message: "Invalid deviceId format",
                        statusCode: 400,
                        timestamp: new Date().toISOString(),
                    },
                });
                return;
            }

            const { token, playerId } = await authService.loginWithDevice(deviceId);

            const response: LoginResponse = {
                success: true,
                token,
                playerId,
            };

            res.status(200).json(response);
        } catch (err) {
            next(err);
        }
    }
);

export default router;