using Core.Hub.UI;
using Core.Hub.Views;
using Core.Hub.Views.Results;
using UnityEngine;

namespace Core.Hub
{
    public class HubView : MonoBehaviour, IHubView
    {
        [SerializeField] private HubTopPanel _topPanel;
        [SerializeField] private HubBottomPanel _bottomPanel;
        [SerializeField] private HubResultsView _resultsView;
        
        [SerializeField] private Transform _mainPanel;
        [SerializeField] private CanvasGroup _minigamesHolder;
        
        public HubTopPanel TopPanel => _topPanel;
        public HubBottomPanel BottomPanel => _bottomPanel;
        public Transform MainPanel => _mainPanel;
        public CanvasGroup MinigamesHolder => _minigamesHolder;
        public HubResultsView ResultsView => _resultsView;
    }
}