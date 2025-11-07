using System;
using System.Collections.Generic;

namespace Common.Server.DTOs
{
    [Serializable]
    public class MinigameDto
    {
        public string id;
        public string name;
        public string iconId;
    }

    [Serializable]
    public class GameModeDto
    {
        public string type;
        public string displayName;
        public int maxPlayers;
        public RewardModel entryFee;
        public List<RewardModel> prizes;
    }
    
    [Serializable]
    public class GetGamesResponse
    {
        public bool success;
        public List<MinigameDto> games;
        public string error;
    }
    
    [Serializable]
    public class GetGameModesResponse
    {
        public bool success;
        public List<GameModeDto> modes;
        public string error;
    }
}