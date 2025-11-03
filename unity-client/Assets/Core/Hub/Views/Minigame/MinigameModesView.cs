using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.Views
{
    public class MinigameModesView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Image _minigameIcon;
        
        [SerializeField] private Transform _modesContainer;
        
        public event Action<string> OnPlayButtonClicked; //todo: should pass game mode
        public event Action OnBackButtonClicked;
    }
}