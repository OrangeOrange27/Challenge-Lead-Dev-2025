using System;
using System.Collections.Generic;
using System.Linq;
using Common.Minigames;
using Common.Minigames.Models;
using Common.Models.Economy;
using Common.Server.DTOs;
using ClientRewardModel = Common.Models.Economy.RewardModel;
using ServerRewardModel = Common.Server.DTOs.RewardModel;

namespace Common.Server
{
    public static class ServerDataAdapter
    {
        // Convert backend string to client enum
        public static CurrencyType FromServer(string backendType)
        {
            var formattedType = backendType?.Trim().ToUpperInvariant();
            return formattedType switch
            {
                "SOFT" or "GEMS" => CurrencyType.Gems,
                "HARD" or "CASH" => CurrencyType.Cash,
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

        public static ClientRewardModel FromServer(ServerRewardModel serverModel)
        {
            if (serverModel == null)
                return default;

            return new ClientRewardModel
            {
                Amount = serverModel.amount,
                CurrencyType = FromServer(serverModel.currencyType)
            };
        }

        public static ServerRewardModel ToServer(ClientRewardModel clientModel)
        {
            return new ServerRewardModel
            {
                amount = (int)clientModel.Amount,
                currencyType = ToServer(clientModel.CurrencyType)
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

        /// <summary>
        /// Adapts server GetGamesResponse to client MinigamesConfig since Config infra is not yet ready on the server
        /// </summary>
        public static MinigamesConfig FromServer(GetGamesResponse gamesResponse)
        {
            var minigames = new List<MinigameModel>();

            return new MinigamesConfig
            {
                Minigames = minigames
            };
        }

        public static List<MinigameModeModel> FromServer(GetGameModesResponse modesResponse)
        {
            if (modesResponse?.modes == null)
                return new List<MinigameModeModel>();

            return modesResponse.modes.Select(m => new MinigameModeModel
            {
                Id = m.type,
                ParticipantsCount = m.maxPlayers,
                EntryFee = FromServer(m.entryFee),
                Prizes = m.prizes?.Select(FromServer).ToArray() ?? new ClientRewardModel[] { },
            }).ToList();
        }

        public static MinigameModeModel FromServer(GameModeDto dto)
        {
            if (dto == null) return null;

            return new MinigameModeModel
            {
                Id = dto.type,
                EntryFee = FromServer(dto.entryFee),
                Prizes = dto.prizes?.Select(FromServer).ToArray() ?? new ClientRewardModel[] { },
                ParticipantsCount = dto.maxPlayers
            };
        }

        public static GameModeDto ToServer(MinigameModeModel clientModel)
        {
            if (clientModel == null) return null;

            return new GameModeDto
            {
                type = clientModel.Id,
                entryFee = ToServer(clientModel.EntryFee),
                prizes = clientModel.Prizes?.Select(ToServer).ToList() ?? new List<ServerRewardModel>(),
                maxPlayers = clientModel.ParticipantsCount
            };
        }
    }
}