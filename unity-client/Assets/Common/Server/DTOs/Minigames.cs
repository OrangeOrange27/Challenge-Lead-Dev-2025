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
        public GameModeDto[] modes;
    }

    [Serializable]
    public class GameModeDto
    {
        public string type;
        public int maxPlayers;
        public RewardModel entryFee;
        public List<RewardModel> prizes;
    }
    
    [Serializable]
    public class GetGamesResponse
    {
        public List<MinigameDto> games;
    }
    
    [Serializable]
    public class GetGameModesResponse
    {
        public List<GameModeDto> modes;
    }
}