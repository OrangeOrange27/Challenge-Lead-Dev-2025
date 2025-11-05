using UnityEngine;

namespace Common.Utils
{
    public static class UiUtils
    {
        public static Vector2 GetRandomLocalPosition(RectTransform parentRect, RectTransform element)
        {
            Vector2 parentSize = parentRect.rect.size;
            Vector2 elementSize = element.rect.size;

            float halfW = elementSize.x / 2f;
            float halfH = elementSize.y / 2f;

            float minX = -parentSize.x / 2f + halfW;
            float maxX =  parentSize.x / 2f - halfW;
            float minY = -parentSize.y / 2f + halfH;
            float maxY =  parentSize.y / 2f - halfH;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);

            return new Vector2(x, y);
        }
    }
}