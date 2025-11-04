using Common.Minigames.Models;
using Common.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI
{
    public class MinigameResultItemView : MonoBehaviour
    {
        [SerializeField] private Image _minigameIcon;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _placeText;
        [SerializeField] private TMP_Text _timeText;

        [SerializeField] private RewardItemView _reward;
        
        [SerializeField] private Button _claimButton;
        
        [SerializeField] private TMP_Text _participantsCountText;
        
        public void Init(MinigameResultStatus status)
        {
            
        }
    }
}