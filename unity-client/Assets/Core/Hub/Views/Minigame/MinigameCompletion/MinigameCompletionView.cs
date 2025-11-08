using System;
using System.Threading;
using Core.Hub.States;
using Core.Hub.UI.Components;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        
        [Header("Animation targets")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _subTitleText;
        [SerializeField] private ScoreItemView[] _scoreItems;
        
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

        public async UniTask PlayAnimation(CancellationToken token)
        {
            // Ensure DOTween sequences are cleared
            DOTween.Kill(this);

            _continueButton.interactable = false;
            
            _titleText.transform.localScale = Vector3.zero;
            _subTitleText.transform.localScale = Vector3.zero;

            await _titleText.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
            await _subTitleText.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

            // Sequentially animate score items
            foreach (var item in _scoreItems)
            {
                token.ThrowIfCancellationRequested();

                // Await the UniTask animation
                await item.PlayAppearAnimation(token);

                // Small delay between items
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
            }
            
            _continueButton.interactable = true;
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveAllListeners();
        }
    }
}