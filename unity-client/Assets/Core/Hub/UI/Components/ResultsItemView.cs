using System;
using Common.Models.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI.Components
{
    public class ResultsItemView : MonoBehaviour, IResultsItemView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _selectedBG;
        [SerializeField] private Button _claimButton;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private RewardItemView _rewards;

        public Transform Transform => transform;
        public event Action OnClaimButtonClicked;

        private void Awake()
        {
            _claimButton.onClick.AddListener(() => OnClaimButtonClicked?.Invoke());
        }

        public void SetData(string minigameName, Sprite icon, string timeAgo, RewardModel rewardModel, bool isClaimed, bool isHighlighted)
        {
            _nameText.text = minigameName;
            _icon.sprite = icon;
            _rewards.Set(rewardModel);
            
            _claimButton.gameObject.SetActive(!isClaimed);
            _rewards.gameObject.SetActive(isClaimed);

            SetHighlighted(isHighlighted);

            _timeText.text = timeAgo;

            //FormatTimeText(time);
        }

        public void SetHighlighted(bool isHighlighted)
        {
            _selectedBG.gameObject.SetActive(isHighlighted);
            _claimButton.gameObject.SetActive(isHighlighted);
            _rewards.gameObject.SetActive(!isHighlighted);
        }

        private void FormatTimeText(TimeSpan time)
        {
            if (time.TotalDays >= 1)
                _timeText.text = $"{(int)time.TotalDays} day{(time.TotalDays >= 2 ? "s" : "")} ago";
            else if (time.TotalHours >= 1)
                _timeText.text = $"{(int)time.TotalHours} hour{(time.TotalHours >= 2 ? "s" : "")} ago";
            else if (time.TotalMinutes >= 1)
                _timeText.text = $"{(int)time.TotalMinutes} min{(time.TotalMinutes >= 2 ? "s" : "")} ago";
            else
                _timeText.text = "just now";
        }


        private void OnDestroy()
        {
            _claimButton.onClick.RemoveAllListeners();
        }
    }
}