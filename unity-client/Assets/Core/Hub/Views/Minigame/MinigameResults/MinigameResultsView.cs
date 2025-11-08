using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views.Minigame.MinigameResults
{
    public class MinigameResultsView : MonoBehaviour, IMinigameResultsView
    {
        [SerializeField] private Transform _leaderboardPanel;

        [SerializeField] private Image _minigameIcon;
        [SerializeField] private Image _minigameBackground;
        [SerializeField] private Transform _leaderboardContent;
        
        [SerializeField] private Button _closeButton;
        
        public event Action OnCloseButtonClicked;
        
        public Transform LeaderboardContent => _leaderboardContent;
        
        public void SetIcon(Sprite icon)
        {
            _minigameIcon.sprite = icon;
        }

        public async UniTask PlayShowAnimation(CancellationToken token)
        {
            _closeButton.interactable = false;
            
            _leaderboardPanel.localScale = Vector3.zero;
            _leaderboardPanel.gameObject.SetActive(true);

            await _leaderboardPanel
                .DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();

            token.ThrowIfCancellationRequested();
            
            _closeButton.interactable = true;
        }

        private void Awake()
        {
            _closeButton.onClick.AddListener(() => OnCloseButtonClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
        }
    }
}