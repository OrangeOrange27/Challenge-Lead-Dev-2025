using System;
using Core.Hub.States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views.Minigame.MinigameCompletion
{
    public class MinigameCompletionView : MonoBehaviour, IMinigameCompletionView
    {
        [SerializeField] private Image _minigameIcon;

        [SerializeField] private TMP_Text _totalScore;
        [SerializeField] private TMP_Text _baseScore;
        [SerializeField] private TMP_Text _timeBonus;

        [SerializeField] private Button _continueButton;
        
        public event Action OnContinueButtonPressed;

        private void Awake()
        {
            _continueButton.onClick.AddListener(() => OnContinueButtonPressed?.Invoke());
        }

        public void SetData(MinigameCompletionPayload payload)
        {
            _minigameIcon.sprite = payload.MinigameIcon;
            _totalScore.text = payload.Result.TotalPoints.ToString();
            _baseScore.text = payload.Result.PointsForPrecision.ToString();
            _timeBonus.text = payload.Result.PointsForReaction.ToString();
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveAllListeners();
        }
    }
}