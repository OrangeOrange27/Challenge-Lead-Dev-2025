using System;
using Core.Hub.States;

namespace Core.Hub.Views.Minigame.MinigameCompletion
{
    public interface IMinigameCompletionView
    {
        event Action OnContinueButtonPressed;
        void SetData(MinigameCompletionPayload payload);
        
        void SetButtonActive(bool isActive);
    }
}