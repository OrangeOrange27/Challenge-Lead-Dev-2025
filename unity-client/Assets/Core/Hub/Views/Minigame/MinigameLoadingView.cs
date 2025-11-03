using Core.Hub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views
{
    public class MinigameLoadingView : MonoBehaviour
    {
        [SerializeField] private Image _minigameBackground;
        [SerializeField] private Image _minigameIcon;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _participantsCountText;
        [SerializeField] private RewardItemView _entryFee;
        
        [SerializeField] private RewardItemView _firstPlaceReward;
        [SerializeField] private RewardItemView _secondPlaceReward;
        [SerializeField] private RewardItemView _thirdPlaceReward;

        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private string _progressBar; //todo: replace with proper Progress Bar

        public void Init()
        {
            
        }
    }
}