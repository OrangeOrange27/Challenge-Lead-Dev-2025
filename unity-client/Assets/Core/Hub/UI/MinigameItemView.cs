using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI
{
    public class MinigameItemView : MonoBehaviour, IMinigameItemView
    {
        [SerializeField] private Image _mainImage;
        [SerializeField] private GameObject _loadingIndicator;
        
        public void SetImage(Sprite sprite)
        {
            _loadingIndicator.SetActive(false);
            _mainImage.sprite = sprite;
        }
    }
}