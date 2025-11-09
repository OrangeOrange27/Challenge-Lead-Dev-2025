using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infra.ControllersTree;
using Infra.Disposables;
using UnityEngine;

namespace Infra.AssetManagement.ViewLoader
{
	internal class SharedViewLoader<TViewInterface, TKey> : ISharedViewLoader<TViewInterface>,
		ISharedViewLoader<TViewInterface, TKey> where TViewInterface : class
	{
		private readonly IViewLoader<TViewInterface, TKey> _viewLoader;

		private readonly Dictionary<TKey, TViewInterface> _cachedValue = new();
		private readonly Dictionary<TKey, CompositeDisposable> _compositeDisposable = new();
		private readonly Dictionary<TKey, int> _referenceCount = new();

		public SharedViewLoader(IViewLoader<TViewInterface, TKey> viewLoader)
		{
			_viewLoader = viewLoader;
		}

		public async UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources,
			CancellationToken cancellationToken, Transform parent)
		{
			_referenceCount.TryAdd(key, 0);
			_referenceCount[key]++;
			var disposable = new Disposable(() =>
			{
				_referenceCount[key]--;
				DisposeLoaderIfReferenceCountZero(key);
			});
			resources.Add(disposable);

			if (_cachedValue.ContainsKey(key))
				return _cachedValue[key];

			if (!_compositeDisposable.ContainsKey(key))
				_compositeDisposable.Add(key, new CompositeDisposable());
			_cachedValue[key] = await _viewLoader.Load(key, _compositeDisposable[key], cancellationToken, parent);

			return _cachedValue[key];
		}

		public TViewInterface CachedView => _cachedValue.FirstOrDefault().Value;

		public async UniTask<TViewInterface> Load(ICollection<IDisposable> resources,
			CancellationToken cancellationToken, Transform parent)
		{
			return await Load(default, resources, cancellationToken, parent);
		}

		private void DisposeLoaderIfReferenceCountZero(TKey key)
		{
			if (_referenceCount[key] > 0)
			{
				return;
			}

			_compositeDisposable[key].Dispose();
			_compositeDisposable[key] = new CompositeDisposable();
			_referenceCount[key] = 0;
			_cachedValue.Remove(key);
		}
	}
}