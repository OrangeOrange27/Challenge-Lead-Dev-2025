using Common.Minigames.Models;
using UnityEngine;

namespace Core.Hub.States
{
    public class MinigameCompletionPayload
    {
        public string MatchId { get; set; }
        public Sprite MinigameIcon { get; set; }
        public MinigameResult Result { get; set; }
    }
}