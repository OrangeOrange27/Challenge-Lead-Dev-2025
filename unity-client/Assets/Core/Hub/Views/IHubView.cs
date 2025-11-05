using Core.Hub.UI;
using UnityEngine;

namespace Core.Hub
{
    public interface IHubView
    {
        HubTopPanel TopPanel { get; }
        Transform MainPanel { get; }
        Transform MinigamesHolder { get; }
    }
}