using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views
{
    public class MinigameResultsView : MonoBehaviour
    {
        [SerializeField] private Image _minigameIcon;
        [SerializeField] private Image _minigameBackground;
        [SerializeField] private Transform _leaderboardContent;
        
        [SerializeField] private Button _closeButton;
        
        public event Action _onCloseButtonClicked;
    }
}