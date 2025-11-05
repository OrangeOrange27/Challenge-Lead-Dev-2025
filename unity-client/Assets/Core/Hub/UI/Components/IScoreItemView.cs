using Common.Models;
using Common.Models.Economy;

namespace Core.Hub.UI.Components
{
    public interface IScoreItemView
    {
        void SetData(MinigameParticipantModel participantModel, RewardModel rewardModel, bool isHighlighted);
        void SetScore(int score);
    }
}