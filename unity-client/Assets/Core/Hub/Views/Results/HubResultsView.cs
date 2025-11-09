using Core.Hub.Views.Results;
using TMPro;
using UnityEngine;

namespace Core.Hub.Views
{
    public class HubResultsView : MonoBehaviour, IHubResultsView
    {
        [SerializeField] private TMP_Text _readyForClaimTitleText;
        [SerializeField] private TMP_Text _pendingTitleText;
        [SerializeField] private TMP_Text _claimedTitleText;
        [SerializeField] private Transform _readyForClaimResultsContainer;
        [SerializeField] private Transform _pendingResultsContainer;
        [SerializeField] private Transform _claimedResultsContainer;

        public Transform ReadyForClaimResultsContainer => _readyForClaimResultsContainer;
        public Transform PendingResultsContainer => _pendingResultsContainer;
        public Transform ClaimedResultsContainer => _claimedResultsContainer;

        public void SetTitleActive(int titleIndex, bool isActive)
        {
            switch (titleIndex)
            {
                case 0:
                    _readyForClaimTitleText.gameObject.SetActive(isActive);
                    break;
                case 1:
                    _pendingTitleText.gameObject.SetActive(isActive);
                    break;
                case 2:
                    _claimedTitleText.gameObject.SetActive(isActive);
                    break;
            }
        }
    }
}