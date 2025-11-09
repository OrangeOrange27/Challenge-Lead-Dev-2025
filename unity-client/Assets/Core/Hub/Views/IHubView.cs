using Core.Hub.UI;
using Core.Hub.Views;
using Core.Hub.Views.Results;
using UnityEngine;

namespace Core.Hub
{
    public interface IHubView
    {
        HubTopPanel TopPanel { get; }
        HubBottomPanel BottomPanel { get; }
        
        Transform MainPanel { get; }
        CanvasGroup MinigamesHolder { get; }
        
        HubResultsView ResultsView { get; }
    }
}