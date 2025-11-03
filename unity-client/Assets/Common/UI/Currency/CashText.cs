namespace Common.UI
{
    public class CashText : CurrencyText
    {
        protected override string FormatValue(float value)
        {
            return $"${value:0.##}";
        }
    }
}