using System;

namespace Common.Server.DTOs
{
    /// <summary>
    /// Submit score request
    /// </summary>
    [Serializable]
    public class SubmitScoreRequest
    {
        public string playerId;
        public int score;
    }

    /// <summary>
    /// Submit score response
    /// </summary>
    [Serializable]
    public class SubmitScoreResponse
    {
        public bool success;
        public int? rank;
        public string error;
    }
}