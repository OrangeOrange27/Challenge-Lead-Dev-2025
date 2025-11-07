import { Request, Response, NextFunction } from "express";

export function errorHandler(
    err: unknown,
    _req: Request,
    res: Response,
    next: NextFunction
): void {
  const error =
      err instanceof Error
          ? err
          : new Error(typeof err === "string" ? err : "Unknown error");

  console.error("[ErrorHandler]", error.message);
  if (error.stack) console.error("[ErrorHandler] Stack:", error.stack);

  if (res.headersSent) return next(error);

  let statusCode = 500;
  let errorMessage = error.message || "Internal server error";

  const normalized = errorMessage.toLowerCase();

  if (normalized.includes("not found")) statusCode = 404;
  else if (
      normalized.includes("already") ||
      normalized.includes("full") ||
      normalized.includes("duplicate")
  )
    statusCode = 409;
  else if (
      normalized.includes("required") ||
      normalized.includes("invalid") ||
      normalized.includes("deadline")
  )
    statusCode = 400;
  else if (
      normalized.includes("unauthorized") ||
      normalized.includes("token") ||
      normalized.includes("forbidden")
  )
    statusCode = 401;

  res.status(statusCode).json({
    success: false,
    error: {
      message: errorMessage,
      type: error.name || "Error",
      statusCode,
      timestamp: new Date().toISOString(),
    },
  });
}