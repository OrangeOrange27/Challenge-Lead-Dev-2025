# Backend

## Installation

### 1. Install Dependencies

```bash
npm install
```

Edit `.env` with your configuration.

### 2. Initialize Database

```bash
npm run init-db
```

### 3. Running the Server

```bash
npm run dev
```

## Core API Endpoints

### Authentication

- `POST /api/auth/login` - Login (currently only using device id)

### Minigames

- `GET /api/minigames` - Get all minigames
- `GET /api/minigames/modes` - Get game modes

### Mathces

- `POST /api/mathces/enter` - Enter a match
- `POST /api/mathces/:id/submit` - Submit score
- `GET /api/mathces/:id/leaderboard` - Get current match leaderboard
- `POST /api/mathces/:id/claim` - Claim reward

### Player

- `GET /api/player/profile` - Get player
- `GET /api/player/history` - Get match history
- `PATCH /api/player/balance` - Change player balance