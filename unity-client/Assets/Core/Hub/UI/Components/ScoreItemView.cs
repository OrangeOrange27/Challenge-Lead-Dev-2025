using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI.Components
{
    public class ScoreItemView : MonoBehaviour
    {
        [SerializeField] private Image _highlightBG;
        [SerializeField] private TMP_Text _mainText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Image[] _icons;
        
        public void SetData(string text, int score, int iconId, bool isHighlighted)
        {
            SetScore(score);
            
            _mainText.text = text;
            _highlightBG.gameObject.SetActive(isHighlighted);
            
            for (var i = 0; i < _icons.Length; i++)
            {
                _icons[i].gameObject.SetActive(i == iconId);
            }
        }
        
        public void SetScore(int score)
        {
            _scoreText.text = score.ToString();
        }
    }
}