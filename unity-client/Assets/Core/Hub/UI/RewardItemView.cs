using Common.Models;
using TMPro;
using UnityEngine;

namespace Core.Hub.UI
{
    public class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Transform _gemsReward;
        [SerializeField] private Transform _cashReward;
        
        [SerializeField] private TMP_Text _gemsAmountText;
        [SerializeField] private TMP_Text _cashAmountText;
        
        public void Set(RewardType rewardType, float amount)
        {
            _gemsAmountText.text = $"{amount:0}";
            _cashAmountText.text = $"{amount:0}";
            
            _gemsReward.gameObject.SetActive(rewardType == RewardType.Gems);
            _cashReward.gameObject.SetActive(rewardType == RewardType.Cash);
        }
    }
}