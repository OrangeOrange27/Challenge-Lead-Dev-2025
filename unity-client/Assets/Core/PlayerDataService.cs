using System;
using Common;
using Common.Authentication.Providers;
using Common.Models;
using Common.Models.Economy;
using Common.Server;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.Disposables;

namespace Core
{
    public class PlayerDataService : IPlayerDataService
    {
        private const string DataKey = "PlayerData";

        private static bool _isOnline;

        private readonly IDataProvider _playerDataProvider;

        public bool IsOnline => _isOnline;
        public bool IsSignedIn { get; private set; }
        public PlayerData PlayerData { get; private set; }
        
        public event Action<PlayerBalanceAssetType, int> OnBalanceChanged;
        
        public PlayerDataService(IDataProvider playerDataProvider)
        {
            _playerDataProvider = playerDataProvider;
        }

        public IDisposable Update()
        {
            return new Disposable(SavePlayerState);
        }

        public async UniTask LoginWithProvider(AuthProvider provider)
        {
            //todo: no server yet
            SetOfflinePlayer();
            PlayerData.OnBalanceChanged += (type, amount) => OnBalanceChanged?.Invoke(type, amount);
        }

        public void GiveBalance(PlayerBalanceAssetType type, int amount)
        {
            if (amount < 0)
                return;

            using (Update())
                PlayerData.ChangeBalance(type, amount);
        }

        public void SpendBalance(PlayerBalanceAssetType type, int amount)
        {
            if (PlayerData.GetBalance(type) < amount)
                return;

            using (Update())
                PlayerData.ChangeBalance(type, -amount);
        }

        public int GetPlayerLevel()
        {
            return 999; //todo: replace with XP calculation
        }

        private void SetOfflinePlayer()
        {
            _isOnline = false;
            PlayerData = GetLocalPlayer();
            IsSignedIn = true;
        }

        private void SavePlayerState()
        {
            SaveLocalPlayer();

            if (_isOnline) _ = ServerAPI.Player.UpdatePlayerDataAsync(PlayerData, PlayerData.AuthToken);
        }

        private void SaveLocalPlayer()
        {
            _playerDataProvider.SetAsync(DataKey, PlayerData);
        }

        private PlayerData GetLocalPlayer()
        {
            var localPlayerData = _playerDataProvider.Get<PlayerData>(DataKey);
            return localPlayerData ?? PlayerData.CreateNew();
        }
    }
}