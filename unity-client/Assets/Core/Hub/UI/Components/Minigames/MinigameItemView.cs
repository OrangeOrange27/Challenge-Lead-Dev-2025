using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI
{
    public class MinigameItemView : MonoBehaviour, IMinigameItemView
    {
        [SerializeField] private Image _mainImage;
        [SerializeField] private GameObject _loadingIndicator;
        [SerializeField] private Button _button;

        public event Action OnClick;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnClick?.Invoke());
        }

        public void SetImage(Sprite sprite)
        {
            _mainImage.sprite = sprite;
            _mainImage.gameObject.SetActive(true);
            _loadingIndicator.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}