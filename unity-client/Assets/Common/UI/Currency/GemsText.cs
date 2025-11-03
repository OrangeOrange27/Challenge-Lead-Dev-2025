using UnityEngine;

namespace Common.UI
{
    public class GemsText : CurrencyText
    {
        protected override string FormatValue(float value)
        {
            return value switch
            {
                >= 1_000_000 => $"{value / 1_000_000f:0.#}M",
                >= 1_000 => $"{value / 1_000f:0.#}K",
                _ => Mathf.FloorToInt(value).ToString()
            };
        }
    }
}