using System;

namespace Common.Server.DTOs
{
    /// <summary>
    /// Enter match request
    /// </summary>
    [Serializable]
    public class EnterMatchRequest
    {
        public string gameId;
        public string playerId;
        public string mode; // GameModeType as string
    }

    /// <summary>
    /// Enter match response
    /// </summary>
    [Serializable]
    public class EnterMatchResponse
    {
        public string matchId;
        public string matchState;
        public int currentPlayers;
        public GameModeDto mode;
    }
}