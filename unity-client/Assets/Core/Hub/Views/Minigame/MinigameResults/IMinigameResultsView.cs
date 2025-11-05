using System;
using UnityEngine;

namespace Core.Hub.Views.Minigame.MinigameResults
{
    public interface IMinigameResultsView
    {
        event Action OnCloseButtonClicked;
        Transform LeaderboardContent { get; }
        
        void SetIcon(Sprite icon);
    }
}