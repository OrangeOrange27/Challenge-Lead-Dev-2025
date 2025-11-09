using UnityEngine;

namespace Core.Hub.Views.Results
{
    public interface IHubResultsView
    {
        CanvasGroup CanvasGroup { get; }
        Transform ReadyForClaimResultsContainer { get; }
        Transform PendingResultsContainer { get; }
        Transform ClaimedResultsContainer { get; }

        void SetTitleActive(int titleIndex, bool isActive);
    }
}