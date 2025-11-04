using System;
using Common;
using Common.Authentication.Providers;
using Common.Models;
using Common.Models.Economy;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IPlayerDataService
    {
        bool IsOnline { get; }
        bool IsSignedIn { get; }
        PlayerData PlayerData { get; }
        public event Action<PlayerBalanceAssetType, int> OnBalanceChanged;

        IDisposable Update();
        UniTask LoginWithProvider(AuthProvider provider);

        void GiveBalance(PlayerBalanceAssetType type, int amount);
        void SpendBalance(PlayerBalanceAssetType type, int amount);

        int GetPlayerLevel();
    }
}