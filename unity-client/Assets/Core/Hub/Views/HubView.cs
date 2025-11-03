using UnityEngine;

namespace Core.Hub
{
    public class HubView : MonoBehaviour, IHubView
    {
        [SerializeField] private Transform _minigamesHolder;
        
        public Transform MinigamesHolder => _minigamesHolder;
    }
}