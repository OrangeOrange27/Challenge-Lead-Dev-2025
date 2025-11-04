using UnityEngine;

namespace Common.Utils
{
    public static class UiUtils
    {
        public static Vector2 GetRandomScreenPosition(RectTransform rect, Vector2 elementSize)
        {
            Vector2 canvasSize = rect.rect.size;

            float minX = elementSize.x / 2f;
            float maxX = canvasSize.x - elementSize.x / 2f;
            float minY = elementSize.y / 2f;
            float maxY = canvasSize.y - elementSize.y / 2f;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);

            return new Vector2(x, y);
        }
    }
}