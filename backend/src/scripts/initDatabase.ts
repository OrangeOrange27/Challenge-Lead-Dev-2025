import fs from "fs";
import path from "path";
import { getDatabase } from "../config/database";

async function initializeDatabase(): Promise<void> {
  console.log("[DB Init] Starting database setup...");

  const schemaFile = path.resolve(__dirname, "../config/schema.sql");
  const rawSql = fs.readFileSync(schemaFile, "utf-8");

  const statements = rawSql
      .replace(/--.*$/gm, "")
      .split(";")
      .map((stmt) => stmt.trim())
      .filter(Boolean);

  const db = getDatabase();

  await new Promise<void>((resolve, reject) => {
    db.serialize(() => {
      db.run("BEGIN TRANSACTION");
      for (const sql of statements) {
        db.run(sql, (err) => {
          if (err) {
            console.error("[DB Init] Statement failed:", sql.substring(0, 100), "...");
            console.error("[DB Init] Error:", err.message);
            reject(err);
            return;
          }
        });
      }
      db.run("COMMIT", (err) => {
        if (err) {
          console.error("[DB Init] Commit failed:", err.message);
          reject(err);
        } else {
          resolve();
        }
      });
    });
  });

  console.log("[DB Init] Database setup complete. Tables and seed data applied.");
}

if (require.main === module) {
  initializeDatabase()
      .then(() => {
        console.log("[DB Init] Success!");
        process.exit(0);
      })
      .catch((err) => {
        console.error("[DB Init] Initialization failed:", err);
        process.exit(1);
      });
}

export { initializeDatabase };