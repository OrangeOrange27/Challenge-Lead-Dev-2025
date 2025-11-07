import sqlite3 from 'sqlite3';
import { CONFIG } from './constants';

let db: sqlite3.Database | null = null;

export function getDatabase(): sqlite3.Database {
  if (!db) {
    db = new sqlite3.Database(CONFIG.DB_PATH, (err) => {
      if (err) {
        console.error('[Database] Error opening database:', err.message);
        throw err;
      }
      console.log('[Database] Connected to SQLite database at', CONFIG.DB_PATH);
    });

    db.run('PRAGMA foreign_keys = ON');
  }
  return db;
}

export function closeDatabase(): Promise<void> {
  return new Promise((resolve, reject) => {
    if (db) {
      db.close((err) => {
        if (err) {
          reject(err);
        } else {
          db = null;
          resolve();
        }
      });
    } else {
      resolve();
    }
  });
}

export function runQuery(query: string, params: any[] = []): Promise<void> {
  return new Promise((resolve, reject) => {
    getDatabase().run(query, params, function (err) {
      if (err) {
        reject(err);
      } else {
        resolve();
      }
    });
  });
}

export function getOne<T>(query: string, params: any[] = []): Promise<T | undefined> {
  return new Promise((resolve, reject) => {
    getDatabase().get(query, params, (err, row) => {
      if (err) {
        reject(err);
      } else {
        resolve(row as T | undefined);
      }
    });
  });
}

export function getAll<T>(query: string, params: any[] = []): Promise<T[]> {
  return new Promise((resolve, reject) => {
    getDatabase().all(query, params, (err, rows) => {
      if (err) {
        reject(err);
      } else {
        resolve(rows as T[]);
      }
    });
  });
}