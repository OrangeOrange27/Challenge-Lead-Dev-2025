using System;
using Common.Models.Economy;

namespace Common.Server
{
    public static class CurrencyAdapter
    {
        // Convert backend string to client enum
        public static CurrencyType FromBackend(string backendType)
        {
            return backendType switch
            {
                "SOFT" => CurrencyType.Gems,
                "HARD" => CurrencyType.Cash,
                _ => throw new ArgumentException($"Unknown backend currency type: {backendType}")
            };
        }

        // Convert client enum to backend string
        public static string ToBackend(CurrencyType clientType)
        {
            return clientType switch
            {
                CurrencyType.Gems => "SOFT",
                CurrencyType.Cash => "HARD",
                _ => throw new ArgumentException($"Unknown client currency type: {clientType}")
            };
        }
    }
}