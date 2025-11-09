using System;
using System.Collections.Generic;

namespace Common.Server.DTOs
{
    /// <summary>
    /// Reward model (used in entryFee and reward)
    /// </summary>
    [Serializable]
    public class RewardModel
    {
        public int amount;
        public string currencyType; // "SOFT" or "HARD"
    }

    /// <summary>
    /// Individual match history item
    /// </summary>
    [Serializable]
    public class MatchHistoryItem
    {
        public string matchId;
        public string gameName;
        public string gameId;
        public string mode;
        public RewardModel entryFee;
        public int? score;
        public RewardModel reward;
        public int totalPlayers;
        public string createdAt;
        public string finalizedAt;
        public string timeAgo;
        public string gameState;
        public bool rewardClaimed;
    }

    /// <summary>
    /// Grouped player history
    /// </summary>
    [Serializable]
    public class GroupedPlayerHistory
    {
        public List<MatchHistoryItem> rewardsToClaim = new List<MatchHistoryItem>();
        public List<MatchHistoryItem> pendingMatches = new List<MatchHistoryItem>();
        public List<MatchHistoryItem> pastMatches = new List<MatchHistoryItem>();
    }

    /// <summary>
    /// Player history response
    /// </summary>
    [Serializable]
    public class PlayerHistoryResponse
    {
        public GroupedPlayerHistory history;
        public int total;
    }
}