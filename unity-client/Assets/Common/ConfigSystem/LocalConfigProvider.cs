using System;
using Core.EntryPoint;
using Cysharp.Threading.Tasks;
using Infra.AssetManagement.AssetProvider;
using Infra.Serialization;
using UnityEngine;

namespace Common.ConfigSystem
{
    public class LocalConfigProvider<T> : IConfigProvider<T>, IBeforeAuthInitialize where T : BaseConfig
    {
        private readonly IAssetProvider _assetProvider;

        private readonly string _builtInKey;
        private readonly ISerializer _serializer;
        private T _cachedData;

        public event Action OnUpdated;

        public LocalConfigProvider(string builtInKey, IAssetProvider assetProvider, ISerializer serializer)
        {
            _assetProvider = assetProvider;
            _serializer = serializer;
            _builtInKey = builtInKey;
        }

        public async UniTask InitializeBeforeAuth()
        {
            await LoadFromBuiltIn();
        }

        public T Get()
        {
            return _cachedData;
        }

        private async UniTask LoadFromBuiltIn()
        {
            var loadAsync = await _assetProvider.LoadAsync<TextAsset>(_builtInKey);
            _cachedData = await _serializer.DeserializeAsync<T>(loadAsync.text);
            Debug.LogFormat("Loaded data from built-in {0}", _builtInKey);
        }
    }
}