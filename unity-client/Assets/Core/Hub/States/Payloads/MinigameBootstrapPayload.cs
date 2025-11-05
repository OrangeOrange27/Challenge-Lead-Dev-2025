using Common.Minigames.Models;
using UnityEngine;

namespace Core.Hub.States
{
    public class MinigameBootstrapPayload
    {
        public Sprite MinigameIcon { get; set; }
        public MinigameModel MinigameModel { get; set; }
        public MinigameModeModel GameMode { get; set; }
    }
}