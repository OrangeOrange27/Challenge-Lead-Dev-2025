using System.Collections.Generic;

namespace Common.Minigames.Models
{
    public class MinigameModel
    {
        public string Id { get; set; }
        public string IconId { get; set; }
        public List<MinigameModeModel> Modes { get; set; }
    }
}