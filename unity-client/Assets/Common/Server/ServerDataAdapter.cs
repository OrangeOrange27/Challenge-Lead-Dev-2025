using System;
using Common.Models.Economy;
using Common.Server.DTOs;

namespace Common.Server
{
    public static class ServerDataAdapter
    {
        // Convert backend string to client enum
        public static CurrencyType FromServer(string backendType)
        {
            return backendType switch
            {
                "SOFT" => CurrencyType.Gems,
                "HARD" => CurrencyType.Cash,
                _ => throw new ArgumentException($"Unknown backend currency type: {backendType}")
            };
        }

        // Convert client enum to backend string
        public static string ToServer(CurrencyType clientType)
        {
            return clientType switch
            {
                CurrencyType.Gems => "SOFT",
                CurrencyType.Cash => "HARD",
                _ => throw new ArgumentException($"Unknown client currency type: {clientType}")
            };
        }

        /// <summary>
        /// Maps server PlayerProfileResponse to client PlayerData
        /// </summary>
        public static PlayerData FromServer(PlayerProfileResponse serverResponse)
        {
            if (serverResponse == null || serverResponse.profile == null)
                return null;

            var profile = serverResponse.profile;

            return new PlayerData
            {
                ID = profile.playerId,
                UserName = profile.playerName,
                Cash = profile.hardCurrency, // map server SOFT currency to Cash
                Gems = profile.softCurrency // map server HARD currency to Gems
            };
        }

        /// <summary>
        /// Maps client PlayerData to server PlayerProfile (for sending updates if needed)
        /// </summary>
        public static PlayerProfile ToServer(PlayerData clientData)
        {
            if (clientData == null)
                return null;

            return new PlayerProfile
            {
                playerId = clientData.ID,
                playerName = clientData.UserName,
                softCurrency = clientData.Gems,
                hardCurrency = clientData.Cash,
                createdAt = DateTime.UtcNow.ToString("o") // or leave null if not needed
            };
        }
    }
}