using System.Threading;
using Common.Models;
using Common.Models.Economy;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI.Components
{
    public class ScoreItemView : MonoBehaviour, IScoreItemView
    {
        [SerializeField] private Image _highlightBG;
        [SerializeField] private TMP_Text _mainText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Image[] _icons;

        [Header("Player Panel")] 
        [SerializeField] private Transform _playerPanel;

        [SerializeField] private Image _playerIcon;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private TMP_Text _playerScore;
        [SerializeField] private RewardItemView _rewardItemView;

        public void SetData(MinigameParticipantModel participantModel, RewardModel rewardModel, bool isHighlighted)
        {
            _highlightBG.gameObject.SetActive(isHighlighted);
            SetIcon(isHighlighted);

            _playerName.text = participantModel.Name;
            _playerScore.text = participantModel.Result.TotalPoints.ToString();

            _rewardItemView.Set(rewardModel);

            _playerPanel.gameObject.SetActive(true);
            _rewardItemView.gameObject.SetActive(true);

            _mainText.gameObject.SetActive(false);
            _scoreText.gameObject.SetActive(false);
        }

        public void SetScore(int score)
        {
            _playerPanel.gameObject.SetActive(false);
            _rewardItemView.gameObject.SetActive(false);

            _mainText.gameObject.SetActive(true);
            _scoreText.gameObject.SetActive(true);

            _scoreText.text = score.ToString();
        }

        public async UniTask PlayAppearAnimation(CancellationToken token)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }

        private void SetIcon(bool isHighlighted)
        {
            var iconIndex = isHighlighted ? 2 : 1;
            for (var i = 0; i < _icons.Length; i++)
            {
                _icons[i].gameObject.SetActive(iconIndex == i);
            }
        }
    }
}