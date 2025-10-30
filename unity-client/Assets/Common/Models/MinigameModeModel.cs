using System.Linq;

namespace Common.Models
{
    public class MinigameModeModel
    {
        public RewardModel[] Prizes { get; set; }
        public RewardModel EntryFee { get; set; }
        public int ParticipantsCount { get; set; }

        public RewardModel GetPrizePool()
        {
            var prizePoolType = RewardType.Cash;
            var prizePool = Prizes
                .Where(p => p.RewardType == prizePoolType)
                .Sum(p => p.Amount);
            if (prizePool <= 0)
            {
                prizePoolType = RewardType.Gems;
                prizePool = Prizes
                    .Where(p => p.RewardType == prizePoolType)
                    .Sum(p => p.Amount);
            }

            return new RewardModel()
            {
                RewardType = prizePoolType,
                Amount = prizePool
            };
        }
    }
}