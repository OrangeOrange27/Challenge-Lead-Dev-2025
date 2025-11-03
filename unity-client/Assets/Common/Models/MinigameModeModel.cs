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
            var prizePoolType = CurrencyType.Cash;
            var prizePool = Prizes
                .Where(p => p.CurrencyType == prizePoolType)
                .Sum(p => p.Amount);
            if (prizePool <= 0)
            {
                prizePoolType = CurrencyType.Gems;
                prizePool = Prizes
                    .Where(p => p.CurrencyType == prizePoolType)
                    .Sum(p => p.Amount);
            }

            return new RewardModel()
            {
                CurrencyType = prizePoolType,
                Amount = prizePool
            };
        }
    }
}