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
        public int Experience { get; set; }

        public event Action<PlayerBalanceAssetType, int> OnBalanceChanged;

        private Dictionary<PlayerBalanceAssetType, (Func<int> get, Action<int> add)> _balanceAccessors =>
            new()
            {
                {
                    PlayerBalanceAssetType.Cash, (() => Cash, value =>
                    {
                        Cash += value;
                        OnBalanceChanged?.Invoke(PlayerBalanceAssetType.Cash, Cash);
                    })
                },
                {
                    PlayerBalanceAssetType.Gems, (() => Gems, value =>
                    {
                        Gems += value;
                        OnBalanceChanged?.Invoke(PlayerBalanceAssetType.Gems, Gems);
                    })
                },
                {
                    PlayerBalanceAssetType.Xp, (() => Experience, value =>
                    {
                        Experience += value;
                        OnBalanceChanged?.Invoke(PlayerBalanceAssetType.Xp, Experience);
                    })
                }
            };

        public int GetBalance(PlayerBalanceAssetType balanceType)
        {
            return _balanceAccessors[balanceType].get();
        }

        public void ChangeBalance(PlayerBalanceAssetType balanceType, int amount)
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