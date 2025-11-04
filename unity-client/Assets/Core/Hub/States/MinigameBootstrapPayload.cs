using Common.Minigames.Models;

namespace Core.Hub.States
{
    public class MinigameBootstrapPayload
    {
        public MinigameModel MinigameModel { get; set; }
        public MinigameModeModel GameMode { get; set; }
    }
}