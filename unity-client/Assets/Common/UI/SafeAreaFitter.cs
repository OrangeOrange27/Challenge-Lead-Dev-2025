using UnityEngine;

namespace Common.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _panel;

        private void Awake() => _panel = GetComponent<RectTransform>();

        private void Update()
        {
            var safe = Screen.safeArea;
            var anchorMin = safe.position;
            var anchorMax = safe.position + safe.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;
        }
    }
}