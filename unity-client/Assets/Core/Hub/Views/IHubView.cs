using UnityEngine;

namespace Core.Hub
{
    public interface IHubView
    {
        Transform MainPanel { get; }
        Transform MinigamesHolder { get; }
    }
}