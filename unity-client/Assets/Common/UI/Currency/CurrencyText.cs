using TMPro;
using UnityEngine;

namespace Common.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public abstract class CurrencyText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void SetValue(float value)
        {
            text.text = FormatValue(value);
        }
        
        protected abstract string FormatValue(float value);
    }
}