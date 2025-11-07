using System;
using System.Collections.Generic;

namespace Common.Server.DTOs
{
    /// <summary>
    /// Leaderboard entry
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public string playerName;
        public int score;
    }

    /// <summary>
    /// Leaderboard response
    /// </summary>
    [Serializable]
    public class GetLeaderboardResponse
    {
        public bool success;
        public string matchId;
        public string matchState;
        public List<LeaderboardEntry> entries;
        public LeaderboardEntry playerEntry;
        public string error;
    }
}