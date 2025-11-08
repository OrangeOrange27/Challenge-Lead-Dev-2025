using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI
{
    public class HubBottomPanelTabView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        [SerializeField] private CanvasGroup _selectedPanel;

        private CancellationTokenSource _cts = new();

        public event Action Clicked
        {
            add => _button.onClick.AddListener(value.Invoke);
            remove => _button.onClick.RemoveListener(value.Invoke);
        }

        public void SetSelected(bool isSelected)
        {
            _button.interactable = !isSelected;
            _selectedPanel.gameObject.SetActive(isSelected);

            if (!isSelected)
            {
                _cts.Cancel();
            }
            else
            {
                _cts = new CancellationTokenSource();
                PlaySelectAnimation(_cts.Token).Forget();
            }
        }

        private async UniTask PlaySelectAnimation(CancellationToken token)
        {
            var originalPos = _selectedPanel.transform.localPosition;

            // Move panel below the screen
            _selectedPanel.transform.localPosition = originalPos + new Vector3(0, -300f, 0); // adjust offset as needed
            _selectedPanel.alpha = 0f;
            _selectedPanel.gameObject.SetActive(true);

            // Animate slide + fade simultaneously
            var moveTween = _selectedPanel.transform
                .DOLocalMove(originalPos, 0.25f)
                .SetEase(Ease.OutCubic);

            var fadeTween = _selectedPanel.DOFade(1f, 0.25f);

            // Await both animations
            await UniTask.WhenAll(moveTween.AsyncWaitForCompletion().AsUniTask(),
                fadeTween.AsyncWaitForCompletion().AsUniTask());

            token.ThrowIfCancellationRequested();
        }
    }
}