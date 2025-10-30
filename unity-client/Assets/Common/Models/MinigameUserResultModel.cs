namespace Common.Models
{
    public class MinigameUserResultModel
    {
        public string AvatarId { get; set; }
        public string Username { get; set; }
        public string Score { get; set; }
        public RewardModel Reward { get; set; }
    }
}