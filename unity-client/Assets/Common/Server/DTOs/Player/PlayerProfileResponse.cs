using System;

namespace Common.Server.DTOs
{
    [Serializable]
    public class PlayerProfileResponse
    {
        public bool success;
        public PlayerProfile profile;
        public string error;
    }
    
    [Serializable]
    public class PlayerProfile
    {
        public string playerId;
        public string playerName;
        public int softCurrency;
        public int hardCurrency;
        public string createdAt;
    }
}