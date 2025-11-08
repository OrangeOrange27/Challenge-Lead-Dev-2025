-- Players
CREATE TABLE IF NOT EXISTS players (
  id TEXT PRIMARY KEY,
  device_id TEXT UNIQUE NOT NULL,
  player_name TEXT,
  soft_currency INTEGER NOT NULL DEFAULT 1000,
  hard_currency INTEGER NOT NULL DEFAULT 0,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

-- Minigames
CREATE TABLE IF NOT EXISTS minigames (
    id TEXT PRIMARY KEY,
    game_name TEXT NOT NULL,
    icon_id TEXT NOT NULL
);

-- Game Modes
CREATE TABLE IF NOT EXISTS gamemodes (
    id TEXT PRIMARY KEY,
    mode TEXT NOT NULL,
    max_players INTEGER NOT NULL,
    display_name TEXT NOT NULL,
    entry_fee_json TEXT NOT NULL DEFAULT '{"CurrencyType":"SOFT","Amount":0}',
    prizes_json TEXT NOT NULL DEFAULT '[]'
);

-- Junction table: many-to-many relation between minigames and gamemodes
CREATE TABLE IF NOT EXISTS minigame_modes (
    minigame_id TEXT NOT NULL,
    gamemode_id TEXT NOT NULL,
    PRIMARY KEY (minigame_id, gamemode_id),
    FOREIGN KEY (minigame_id) REFERENCES minigames(id) ON DELETE CASCADE,
    FOREIGN KEY (gamemode_id) REFERENCES gamemodes(id) ON DELETE CASCADE
    );

CREATE TABLE IF NOT EXISTS matches (
    id TEXT PRIMARY KEY,
    game_id TEXT NOT NULL,
    mode_id TEXT NOT NULL,
    match_state TEXT NOT NULL CHECK(match_state IN ('OPEN', 'RUNNING', 'COMPLETE')),
    participants_count INTEGER NOT NULL DEFAULT 0,
    created_at TEXT NOT NULL DEFAULT (datetime('now')),
    finalized_at TEXT,
    FOREIGN KEY (game_id) REFERENCES minigames(id),
    FOREIGN KEY (mode_id) REFERENCES gamemodes(id)
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

-- Seed data: Minigames
INSERT OR IGNORE INTO minigames (id, game_name, icon_id)
VALUES 
  ('match', 'Match', 'minigame_match_icon');

-- Seed data: Game Modes
INSERT OR IGNORE INTO gamemodes (id, mode, display_name, entry_fee_json, max_players, prizes_json)
VALUES 
  ('DEFAULT', 'DEFAULT', 'DEFAULT', '{"CurrencyType":"SOFT","Amount":10}', 5, 
   '[{"CurrencyType":"HARD","Amount":10},{"CurrencyType":"SOFT","Amount":50},{"CurrencyType":"SOFT","Amount":20}]');

-- Link minigame with its game mode
INSERT OR IGNORE INTO minigame_modes (minigame_id, gamemode_id)
VALUES 
  ('match', 'DEFAULT');
       
-- Seed 10 bot players
INSERT OR IGNORE INTO players (id, device_id, player_name, soft_currency, hard_currency)
VALUES
  ('bot_1', 'bot_1_device', 'Bot 1', 1000, 0),
  ('bot_2', 'bot_2_device', 'Bot 2', 1000, 0),
  ('bot_3', 'bot_3_device', 'Bot 3', 1000, 0),
  ('bot_4', 'bot_4_device', 'Bot 4', 1000, 0),
  ('bot_5', 'bot_5_device', 'Bot 5', 1000, 0),
  ('bot_6', 'bot_6_device', 'Bot 6', 1000, 0),
  ('bot_7', 'bot_7_device', 'Bot 7', 1000, 0),
  ('bot_8', 'bot_8_device', 'Bot 8', 1000, 0),
  ('bot_9', 'bot_9_device', 'Bot 9', 1000, 0),
  ('bot_10', 'bot_10_device', 'Bot 10', 1000, 0);
