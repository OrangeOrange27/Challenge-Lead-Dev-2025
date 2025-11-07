import { Request, Response, NextFunction } from "express";
import { AuthService } from "../services/AuthService";

declare global {
  namespace Express {
    interface Request {
      user?: {
        playerId: string;
        deviceId: string;
      };
    }
  }
}

const authService = new AuthService();

export async function authMiddleware(
    req: Request,
    res: Response,
    next: NextFunction
): Promise<void> {
  const authHeader = req.headers["authorization"];

  if (!authHeader) {
    res.status(401).json({
      success: false,
      error: "Authorization header missing",
    });
    return;
  }

  const [scheme, token] = authHeader.split(" ");
  if (scheme?.toLowerCase() !== "bearer" || !token) {
    res.status(401).json({
      success: false,
      error: "Invalid authorization format",
    });
    return;
  }

  try {
    const decoded = authService.verifyToken(token);

    if (!decoded?.playerId || !decoded?.deviceId) {
      res.status(401).json({
        success: false,
        error: "Token payload is invalid",
      });
      return;
    }

    req.user = {
      playerId: decoded.playerId,
      deviceId: decoded.deviceId,
    };

    next();
    return;
  } catch (err) {
    res.status(401).json({
      success: false,
      error: "Token is invalid or expired",
    });
    return;
  }
}