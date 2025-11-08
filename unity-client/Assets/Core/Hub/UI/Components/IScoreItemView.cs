using System.Threading;
using Common.Models;
using Common.Models.Economy;
using Cysharp.Threading.Tasks;

namespace Core.Hub.UI.Components
{
    public interface IScoreItemView
    {
        void SetData(MinigameParticipantModel participantModel, RewardModel rewardModel, bool isHighlighted);
        void SetScore(int score);
        
        UniTask PlayAppearAnimation(CancellationToken token);
    }
}