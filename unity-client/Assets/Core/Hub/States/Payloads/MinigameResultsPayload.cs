using System.Collections.Generic;
using Common.Minigames.Models;
using Common.Models;
using UnityEngine;

namespace Core.Hub.States
{
    public class MinigameResultsPayload
    {
        public Sprite MinigameIcon { get; set; }
        public List<MinigameParticipantModel> Participants { get; set; }
        public MinigameParticipantModel LocalPlayer { get; set; }
        public MinigameModeModel GameMode { get; set; }
    }
}