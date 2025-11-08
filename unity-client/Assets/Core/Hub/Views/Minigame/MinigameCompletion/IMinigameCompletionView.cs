using System;
using System.Threading;
using Core.Hub.States;
using Cysharp.Threading.Tasks;

namespace Core.Hub.Views.Minigame.MinigameCompletion
{
    public interface IMinigameCompletionView
    {
        event Action OnContinueButtonPressed;
        
        void SetData(MinigameCompletionPayload payload);
        UniTask PlayAnimation(CancellationToken token);
    }
}