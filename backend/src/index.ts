import express, { Application } from "express";
import cors from "cors";
import { CONFIG } from "./config/constants";
import { initializeDatabase } from "./scripts/initDatabase";
import { errorHandler } from "./middleware/errorHandler";
import routes from "./routes";

const app: Application = express();

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.use((req, _res, next) => {
  console.log(`[${new Date().toISOString()}] ${req.method} ${req.path}`);
  next();
});

app.use("/api", routes);
app.use(errorHandler);

async function startServer(): Promise<void> {
  try {
    console.log("[Server] Initializing database...");
    await initializeDatabase();

    console.log("[Server] Starting Match Finalizer Job...");

    app.listen(CONFIG.PORT, () => {
      console.log("=".repeat(10));
      console.log("Backend API");
      console.log("=".repeat(10));
      console.log(`Server running on port: ${CONFIG.PORT}`);
      console.log(`Environment: ${process.env.NODE_ENV || "dev"}`);
      console.log(`Database: ${CONFIG.DB_PATH}`);
      console.log(`API Base URL: http://localhost:${CONFIG.PORT}/api`);
      console.log("=".repeat(10));
      console.log("\nAvailable Endpoints:");
      console.log("  POST   /api/auth/login");
      console.log("  GET    /api/minigames");
      console.log("  GET    /api/minigames/:id");
      console.log("  POST   /api/matches/enter");
      console.log("  POST   /api/matches/:id/submit");
      console.log("  GET    /api/matches/:id/leaderboard");
      console.log("  POST   /api/matches/:id/claim");
      console.log("=".repeat(10));
    });
  } catch (error) {
    console.error("[Server] Failed to start server:", error);
    process.exit(1);
  }
}

startServer();