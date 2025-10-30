using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views
{
    public class MinigameCompletionView : MonoBehaviour
    {
        [SerializeField] private Image _minigameBackground;
        [SerializeField] private Image _minigameIcon;
        [SerializeField] private TMP_Text _totalScore;
        [SerializeField] private TMP_Text _baseScore;
        [SerializeField] private TMP_Text _timeBonus;
        [SerializeField] private TMP_Text _lifeRemainingBonus;
        
        [SerializeField] private TMP_Text _xpText;
        
        [SerializeField] private TMP_Text _previousHighScore;
        
        [SerializeField] private Button _continueButton;

        public void Init() //todo: implement
        {
            throw new System.NotImplementedException();
        }
    }
}