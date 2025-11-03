using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Infra.AssetManagement.AssetProvider
{
    public interface IAssetProvider : IDisposable
    {
        UniTask Initialize();

        UniTask<T> LoadAsync<T>(string key, CancellationToken token = default) where T : UnityEngine.Object;
        
        string[] GetAllKeys();
    }
}