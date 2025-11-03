using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Infra.AssetManagement.AssetProvider
{
	public class AddressablesAssetProvider : IAssetProvider
	{
		private const int AddressablesAssetProviderReleaseHandleDelayFramesCount = 1;

		private readonly Dictionary<string, object> _cache = new();
		private readonly Dictionary<string, HashSet<string>> _cacheLabels = new();
		private readonly List<AsyncOperationHandle> _getDownloadSizeAsyncHandles = new();
		private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

		private string[] _allKeys;

		public async UniTask Initialize()
		{
			await InitializeAddressables();
		}

		public string[] GetAllKeys()
		{
			return _allKeys ??= Addressables.ResourceLocators.SelectMany(locator => locator.Keys)
				.Cast<string>().ToArray();
		}

		public async UniTask<T> LoadAsync<T>(string key, CancellationToken token = default) where T : Object
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

			if (_cache.ContainsKey(key)) return (T)GetCachedItem(key);

			if (_handles.TryGetValue(key, out var operationHandle))
			{
				try
				{
					await operationHandle.WithCancellation(token);

					return (T)operationHandle.Result;
				}
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (Exception e)
				{
					throw new AssetProviderException(key, e);
				}
			}

			var isKeyValid = await IsKeyValid(key);

			if (!isKeyValid) throw new Exception($"Key doesn't exist in addressables: {key}");

			var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
			_handles.Add(key, asyncOperationHandle);

			try
			{
				var result = await asyncOperationHandle.WithCancellation(token);
				token.ThrowIfCancellationRequested();
				AddToCache(result, key);

				return result;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new AssetProviderException(key, e);
			}
		}

		public async UniTask<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> keys, CancellationToken token = default)
			where T : Object
		{
			var result = await UniTask.WhenAll(Enumerable.Select(keys, key => LoadAsync<T>(key, token)));

			return result;
		}

		public void Dispose()
		{
			ReleaseAllAssetsWithFrameDelay().Forget();
		}

		//We have to release asset after destroy GameObject of asset
		private async UniTaskVoid ReleaseAllAssetsWithFrameDelay()
		{
			await UniTask.DelayFrame(AddressablesAssetProviderReleaseHandleDelayFramesCount,
				PlayerLoopTiming.LastPostLateUpdate);
			await UniTask.SwitchToMainThread();

			foreach (var handleValue in _handles.Values) Addressables.Release(handleValue);

			foreach (var getDownloadSizeAsyncHandle in _getDownloadSizeAsyncHandles)
			{
				if (!getDownloadSizeAsyncHandle.IsValid()) continue;

				Addressables.Release(getDownloadSizeAsyncHandle);
			}

			_cacheLabels.Clear();
			_cache.Clear();
			_handles.Clear();
			_getDownloadSizeAsyncHandles.Clear();
		}

		private async UniTask InitializeAddressables()
		{
			await Addressables.InitializeAsync();
		}

		private object GetCachedItem(string key)
		{
			return _cache.GetValueOrDefault(key);
		}

		private void AddToCache(object obj, string key, string label = "")
		{
			_cache[key] = obj;

			if (!string.IsNullOrEmpty(label))
			{
				if (_cacheLabels.TryGetValue(label, out var listKeys))
				{
					listKeys.Add(key);
				}
				else
				{
					_cacheLabels.Add(label, new HashSet<string> { key });
				}
			}
		}

		private async UniTask<bool> IsKeyValid(string key)
		{
			var locations = await Addressables.LoadResourceLocationsAsync(key);

			return locations.Count > 0;
		}
	}
}