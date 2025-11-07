-- Players
CREATE TABLE IF NOT EXISTS players (
  id TEXT PRIMARY KEY,
  device_id TEXT UNIQUE NOT NULL,
  soft_currency INTEGER NOT NULL DEFAULT 1000,
  hard_currency INTEGER NOT NULL DEFAULT 0,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

-- Game Modes
CREATE TABLE IF NOT EXISTS gamemodes (
  mode TEXT PRIMARY KEY CHECK(mode IN ('DEFAULT')),
  max_players INTEGER NOT NULL,
  display_name TEXT NOT NULL,
  entry_fee_json TEXT NOT NULL DEFAULT '{"CurrencyType":"Gems","Amount":0}',
  prizes_json TEXT NOT NULL DEFAULT '[]'
);

-- Minigames
CREATE TABLE IF NOT EXISTS minigames (
  id TEXT PRIMARY KEY,
  game_name TEXT NOT NULL,
  icon_id TEXT NOT NULL
);

-- Matches
CREATE TABLE IF NOT EXISTS matches (
  id TEXT PRIMARY KEY,
  game_id TEXT NOT NULL,
  mode TEXT NOT NULL CHECK(mode IN ('DEFAULT')),
  match_state TEXT NOT NULL CHECK(match_state IN ('OPEN', 'RUNNING', 'COMPLETE')),
  participants_count INTEGER NOT NULL DEFAULT 0,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  finalized_at TEXT,
  FOREIGN KEY (game_id) REFERENCES minigames(id),
  FOREIGN KEY (mode) REFERENCES gamemodes(mode)
);

-- Participations
CREATE TABLE IF NOT EXISTS participations (
  id TEXT PRIMARY KEY,
  match_id TEXT NOT NULL,
  player_id TEXT NOT NULL,
  score INTEGER,
  submitted_at TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  FOREIGN KEY (match_id) REFERENCES matches(id),
  FOREIGN KEY (player_id) REFERENCES players(id),
  UNIQUE(match_id, player_id)
);

-- Rewards
CREATE TABLE IF NOT EXISTS rewards (
  id TEXT PRIMARY KEY,
  match_id TEXT NOT NULL,
  player_id TEXT NOT NULL,
  reward_json TEXT NOT NULL,
  claimable INTEGER NOT NULL DEFAULT 0,
  claimed_at TEXT,
  FOREIGN KEY (match_id) REFERENCES matches(id),
  FOREIGN KEY (player_id) REFERENCES players(id),
  UNIQUE(match_id, player_id)
);

-- Transactions
CREATE TABLE IF NOT EXISTS transactions (
  id TEXT PRIMARY KEY,
  player_id TEXT NOT NULL,
  amount INTEGER NOT NULL,
  currency_type TEXT NOT NULL CHECK(currency_type IN ('SOFT', 'HARD')),
  transaction_type TEXT NOT NULL CHECK(transaction_type IN ('ENTRY_FEE', 'REWARD_CLAIM')),
  match_id TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  FOREIGN KEY (player_id) REFERENCES players(id),
  FOREIGN KEY (match_id) REFERENCES matches(id)
);

-- Seed data: Game Modes
INSERT OR IGNORE INTO gamemodes (mode, display_name, entry_fee_json, max_players, prizes_json)
VALUES 
  ('DEFAULT', 'DEFAULT', '{"CurrencyType":"Gems","Amount":10}', 5, '[{"CurrencyType":"Gems","Amount":100},{"CurrencyType":"Gems","Amount":50},{"CurrencyType":"Gems","Amount":20}]');

-- Seed data: Minigames
INSERT OR IGNORE INTO minigames (id, game_name, icon_id)
VALUES 
  ('match', 'Match', 'minigame_match_icon');