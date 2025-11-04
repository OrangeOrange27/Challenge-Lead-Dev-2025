using Common.Minigames.Models;
using UnityEngine;

namespace Core.Hub.States
{
    public class SelectModeStatePayload
    {
        public MinigameModel MinigameModel { get; set; }
        public Sprite MinigameIcon { get; set; }
        public Transform ViewParent { get; set; }
    }
}