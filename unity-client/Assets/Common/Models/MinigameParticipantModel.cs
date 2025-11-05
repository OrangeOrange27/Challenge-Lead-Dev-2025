using Common.Minigames.Models;

namespace Common.Models
{
    // Ideally we need to pass avatar and other relevant player data here
    // But for now we will just mock other players results for thew player
    public class MinigameParticipantModel
    {
        public string UserId { get; set; }
        public string AvatarId { get; set; }
        public string Name { get; set; }
        public MinigameResult Result { get; set; }
    }
}