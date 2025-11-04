using Common.Models;
using Common.Models.Economy;
using Common.UI;
using UnityEngine;

namespace Core.Hub.UI
{
    public class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Transform _gemsReward;
        [SerializeField] private Transform _cashReward;
        
        [SerializeField] private GemsText _gemsAmountText;
        [SerializeField] private CashText _cashAmountText;
        
        public void Set(RewardModel reward)
        {
            Set(reward.CurrencyType, reward.Amount);
        }
        
        public void Set(CurrencyType currencyType, float amount)
        {
            _gemsAmountText.gameObject.SetActive(currencyType == CurrencyType.Gems);
            _cashAmountText.gameObject.SetActive(currencyType == CurrencyType.Cash);
            
            _gemsAmountText.SetValue(amount);
            _cashAmountText.SetValue(amount);
            
            _gemsReward.gameObject.SetActive(currencyType == CurrencyType.Gems);
            _cashReward.gameObject.SetActive(currencyType == CurrencyType.Cash);
        }
    }
}