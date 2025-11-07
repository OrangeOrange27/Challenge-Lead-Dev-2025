import dotenv from "dotenv";

dotenv.config();
export const CONFIG = {
  PORT: parseInt(process.env.PORT || "8080", 10),
  TOKEN_SECRET: process.env.TOKEN_SECRET || "default",
  TOKEN_TTL: (process.env.TOKEN_TTL || "31d") as string,
  DB_PATH: process.env.DB_PATH || "./database.sqlite",
  FINALIZATION_JOB_INTERVAL_MS: 15000, // 15 seconds
};