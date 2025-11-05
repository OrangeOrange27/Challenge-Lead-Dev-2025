using Core.Hub.UI;
using UnityEngine;

namespace Core.Hub
{
    public class HubView : MonoBehaviour, IHubView
    {
        [SerializeField] private HubTopPanel _topPanel;
        [SerializeField] private Transform _mainPanel;
        [SerializeField] private Transform _minigamesHolder;
        
        public HubTopPanel TopPanel => _topPanel;
        public Transform MainPanel => _mainPanel;
        public Transform MinigamesHolder => _minigamesHolder;
    }
}