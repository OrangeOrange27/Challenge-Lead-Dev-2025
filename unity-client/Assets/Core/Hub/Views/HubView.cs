using UnityEngine;

namespace Core.Hub
{
    public class HubView : MonoBehaviour, IHubView
    {
        [SerializeField] private Transform _mainPanel;
        [SerializeField] private Transform _minigamesHolder;
        
        public Transform MainPanel => _mainPanel;
        public Transform MinigamesHolder => _minigamesHolder;
    }
}