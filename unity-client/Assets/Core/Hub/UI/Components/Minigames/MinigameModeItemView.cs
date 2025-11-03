using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI
{
    public class MinigameModeItemView : MonoBehaviour
    {
        [SerializeField] private RewardItemView _prizePool;
        [SerializeField] private RewardItemView _entryFee;
        [SerializeField] private TMP_Text _participantsCountText;
        
        [SerializeField] private Button _playButton;
        
        public event Action OnPlayButtonClicked;

        public void Init()
        {
            
        }
    }
}