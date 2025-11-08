using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Hub.Views.Minigame.MinigameResults
{
    public interface IMinigameResultsView
    {
        event Action OnCloseButtonClicked;
        Transform LeaderboardContent { get; }
        
        void SetIcon(Sprite icon);
        
        UniTask PlayShowAnimation(CancellationToken token);
    }
}