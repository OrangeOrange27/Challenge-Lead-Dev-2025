import { Request, Response, NextFunction } from "express";

function buildErrorResponse(message: string) {
  return {
    success: false,
    error: {
      message,
      statusCode: 400,
      timestamp: new Date().toISOString(),
    },
  };
}

export function validateBody(requiredFields: string[]) {
  return (req: Request, res: Response, next: NextFunction): void => {
    if (!req.body || typeof req.body !== "object") {
      res.status(400).json(
          buildErrorResponse("Invalid or missing request body")
      );
      return;
    }

    const missingFields = requiredFields.filter(
        (f) => req.body[f] === undefined || req.body[f] === null
    );

    if (missingFields.length > 0) {
      res
          .status(400)
          .json(buildErrorResponse(`Missing required fields: ${missingFields.join(", ")}`));
      return;
    }

    next();
  };
}

export function validateParams(requiredParams: string[]) {
  return (req: Request, res: Response, next: NextFunction): void => {
    const missingParams = requiredParams.filter(
        (p) => req.params[p] === undefined || req.params[p] === null
    );

    if (missingParams.length > 0) {
      res
          .status(400)
          .json(buildErrorResponse(`Missing required parameters: ${missingParams.join(", ")}`));
      return;
    }

    next();
  };
}