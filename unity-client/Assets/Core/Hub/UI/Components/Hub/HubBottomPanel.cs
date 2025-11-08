using System;
using UnityEngine;

namespace Core.Hub.UI
{
    public class HubBottomPanel : MonoBehaviour
    {
        [SerializeField] private HubBottomPanelTabView _homeButton;
        [SerializeField] private HubBottomPanelTabView _resultsButton;
        
        public event Action<int> OnTabSelected; //todo: replace int with enum

        private void Awake()
        {
            _homeButton.Clicked += OnHomeButtonClicked;
            _resultsButton.Clicked += OnResultsButtonClicked;
        }

        private void OnResultsButtonClicked()
        {
            _homeButton.SetSelected(false);
            _resultsButton.SetSelected(true);
            
            OnTabSelected?.Invoke(1);
        }

        private void OnHomeButtonClicked()
        {
            _resultsButton.SetSelected(false);
            _homeButton.SetSelected(true);
            
            OnTabSelected?.Invoke(0);
        }

        private void OnDestroy()
        {
            _homeButton.Clicked -= OnHomeButtonClicked;
            _resultsButton.Clicked -= OnResultsButtonClicked;
        }
    }
}