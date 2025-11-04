using System;
using Common.Minigames.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI
{
    public class MinigameModeItemView : MonoBehaviour, IMinigameModeItemView
    {
        [SerializeField] private RewardItemView _prizePool;
        [SerializeField] private RewardItemView _entryFee;
        [SerializeField] private TMP_Text _participantsCountText;

        [SerializeField] private Button _playButton;

        public event Action OnPlayButtonClicked;

        private void Awake()
        {
            _playButton.onClick.AddListener(() => OnPlayButtonClicked?.Invoke());
        }

        public void SetData(MinigameModeModel model)
        {
            var prizePool = model.GetPrizePool();

            _prizePool.Set(prizePool);
            _entryFee.Set(model.EntryFee);

            _participantsCountText.text = $"{model.ParticipantsCount} Players";
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveAllListeners();
        }
    }
}