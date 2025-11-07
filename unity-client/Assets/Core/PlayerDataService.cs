using System;
using Common;
using Common.Authentication.Providers;
using Common.Models;
using Common.Models.Economy;
using Common.Server;
using Common.Utils;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.Disposables;
using UnityEngine;

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
        
        public event Action<CurrencyType, int> OnBalanceChanged;
        
        public PlayerDataService(IDataProvider playerDataProvider)
        {
            _playerDataProvider = playerDataProvider;
        }

        public IDisposable Update()
        {
            return new Disposable(SavePlayerBalance);
        }

        public async UniTask LoginWithProvider(AuthProvider provider)
        {
            var loginData = new ServerAPI.LoginData
            {
                DeviceId =  SystemInfo.deviceUniqueIdentifier
            };
            
            var loginResponse = await ServerAPI.Login.LoginAsync(loginData);
            
            if (loginResponse.Token.IsNullOrEmpty() || loginResponse.PlayerId.IsNullOrEmpty())
            {
                Debug.LogError("Could not login to server. Check above for server errors.");
                SetOfflinePlayer();
                return;
            }
            
            _isOnline = true;
            
            var authToken = loginResponse.Token;
            var response = await ServerAPI.Player.GetPlayerDataAsync(authToken);
            
            PlayerData = ServerDataAdapter.FromServer(response);
            SaveLocalPlayer();
            
            PlayerData.AuthToken = authToken;
            
            Debug.LogFormat("Got player data from server. Data:{0}", PlayerData);
            IsSignedIn = true;
            
            PlayerData.OnBalanceChanged += (type, amount) => OnBalanceChanged?.Invoke(type, amount);
        }

        public void GiveBalance(CurrencyType type, int amount)
        {
            if (amount < 0)
                return;

            using (Update())
                PlayerData.ChangeBalance(type, amount);
        }

        public void SpendBalance(CurrencyType type, int amount)
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

        private void SavePlayerBalance()
        {
            SaveLocalPlayer();

            if (!_isOnline) return;
            
            _ = ServerAPI.Player.UpdatePlayerBalanceAsync(PlayerData.GetBalance(CurrencyType.Gems), CurrencyType.Gems, PlayerData.AuthToken);
            _ = ServerAPI.Player.UpdatePlayerBalanceAsync(PlayerData.GetBalance(CurrencyType.Cash), CurrencyType.Cash, PlayerData.AuthToken);
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