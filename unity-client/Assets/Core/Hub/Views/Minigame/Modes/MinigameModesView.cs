using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views
{
    public class MinigameModesView : MonoBehaviour, IMinigameModesView
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Image _minigameIcon;
        
        [SerializeField] private Transform _modesContainer;
        
        public Transform ModesContainer => _modesContainer;

        public event Action OnBackButtonClicked;
        
        public void SetIcon(Sprite icon)
        {
            _minigameIcon.sprite = icon;
        }
    }
}