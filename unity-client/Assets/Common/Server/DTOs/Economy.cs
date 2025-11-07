using System;

namespace Common.Server.DTOs
{
    /// <summary>
    /// Claim reward request
    /// </summary>
    [Serializable]
    public class ClaimRewardRequest
    {
        public string playerId;
    }

    /// <summary>
    /// Claim reward response
    /// </summary>
    [Serializable]
    public class ClaimRewardResponse
    {
        public bool success;
        public RewardModel reward;
        public PlayerBalance newBalance;
        public string error;
    }

    /// <summary>
    /// Player balance structure
    /// </summary>
    [Serializable]
    public class PlayerBalance
    {
        public int softCurrency;
        public int hardCurrency;
    }
}