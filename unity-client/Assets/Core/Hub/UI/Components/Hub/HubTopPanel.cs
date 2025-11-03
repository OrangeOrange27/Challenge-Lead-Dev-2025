using TMPro;
using UnityEngine;

namespace Core.Hub.UI
{
    public class HubTopPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelProgressText;
        [SerializeField] private TMP_Text _gemsText;
        [SerializeField] private TMP_Text _balanceText;

        public void UpdateLevelProgress(float progress)
        {
            _levelProgressText.text = $"{progress:0}%";
        }

        public void UpdateGems(int gems)
        {
            _gemsText.text = gems.ToString();
        }

        public void UpdateBalance(int balance)
        {
            _balanceText.text = balance.ToString();
        }
    }
}