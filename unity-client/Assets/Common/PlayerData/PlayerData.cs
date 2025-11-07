using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Economy;

namespace Common
{
    public class PlayerData : BasePlayerData
    {
        public int Cash { get; set; }
        public int Gems { get; set; }

        public event Action<CurrencyType, int> OnBalanceChanged;

        private Dictionary<CurrencyType, (Func<int> get, Action<int> add)> _balanceAccessors =>
            new()
            {
                {
                    CurrencyType.Cash, (() => Cash, value =>
                    {
                        Cash += value;
                        OnBalanceChanged?.Invoke(CurrencyType.Cash, Cash);
                    })
                },
                {
                    CurrencyType.Gems, (() => Gems, value =>
                    {
                        Gems += value;
                        OnBalanceChanged?.Invoke(CurrencyType.Gems, Gems);
                    })
                }
            };

        public int GetBalance(CurrencyType balanceType)
        {
            return _balanceAccessors[balanceType].get();
        }

        public void ChangeBalance(CurrencyType balanceType, int amount)
        {
            _balanceAccessors[balanceType].add(amount);
        }
        
        public static PlayerData CreateNew()
        {
            PlayerData playerData = new()
            {
                ID = Guid.NewGuid().ToString(),
            };

            return playerData;
        }
    }
}