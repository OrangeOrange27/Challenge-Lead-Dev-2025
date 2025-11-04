namespace Common.Minigames.Models
{
    public class MinigameResult
    {
        public int PointsForPrecision { get; set; }
        public int PointsForReaction { get; set; }
        public int TotalPoints => PointsForPrecision + PointsForReaction;
    }
}