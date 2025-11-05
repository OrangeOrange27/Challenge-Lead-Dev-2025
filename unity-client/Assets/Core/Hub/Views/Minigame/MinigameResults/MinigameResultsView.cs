using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views.Minigame.MinigameResults
{
    public class MinigameResultsView : MonoBehaviour, IMinigameResultsView
    {
        [SerializeField] private Image _minigameIcon;
        [SerializeField] private Image _minigameBackground;
        [SerializeField] private Transform _leaderboardContent;
        
        [SerializeField] private Button _closeButton;
        
        public event Action _onCloseButtonClicked;
    }
}