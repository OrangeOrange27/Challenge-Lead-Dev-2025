using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infra.AssetManagement.AssetProvider;
using Infra.Disposables;
using UnityEngine;

namespace Infra.AssetManagement.ViewLoader
{
    internal class ViewLoader<TViewPrefab, TViewInterface, TKey> : IViewLoader<TViewInterface>,
        IViewLoader<TViewInterface, TKey>
        where TViewPrefab : Component, TViewInterface where TViewInterface : class
    {
        private readonly Func<TKey, string> _convertKeyToAddressablesKey;
        private readonly Func<IAssetProvider> _assetProviderFactory;

        private IAssetProvider _assetProvider;

        public ViewLoader(Func<IAssetProvider> assetProviderFactory, Func<TKey, string> convertKeyToAddressablesKey)
        {
            _assetProviderFactory = assetProviderFactory;
            _convertKeyToAddressablesKey = convertKeyToAddressablesKey;
        }

        public async UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources,
            CancellationToken cancellationToken, Transform parent)
        {
            return await LoadInternal(_convertKeyToAddressablesKey.Invoke(key), resources,
                cancellationToken, parent);
        }

        public async UniTask<TViewInterface> Load(ICollection<IDisposable> resources,
            CancellationToken cancellationToken, Transform parent)
        {
            return await LoadInternal(_convertKeyToAddressablesKey.Invoke(default), resources,
                cancellationToken, parent);
        }

        private async UniTask<TViewPrefab> LoadInternal(string key, ICollection<IDisposable> disposables,
            CancellationToken cancellationToken,
            Transform parent = null)
        {
            var item = await Get(key, parent, cancellationToken);

            disposables.Add(new Disposable(() =>
            {
                if (item != null)
                {
                    UnityEngine.Object.Destroy(item.gameObject);
                }
            }));

            return item;
        }

        private async UniTask<TViewPrefab> Get(string key, Transform parent, CancellationToken cancellationToken)
        {
            _assetProvider ??= _assetProviderFactory.Invoke();

            var component = await LoadPrefab(_assetProvider, key, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            return CreateInstance(component.gameObject, parent).GetComponent<TViewPrefab>();
        }

        private async UniTask<TViewPrefab> LoadPrefab(IAssetProvider assetProvider, string key,
            CancellationToken cancellationToken)
        {
            var prefab = await assetProvider.LoadAsync<GameObject>(key, cancellationToken);

            if (prefab == null)
                throw new Exception(
                    $"{nameof(assetProvider.LoadAsync)} returned null object for {key}");

            var component = prefab.GetComponent<TViewPrefab>();

            if (component == null)
                throw new Exception(
                    $"{nameof(assetProvider.LoadAsync)} returned prefab without {typeof(TViewPrefab).Name} component for '{key}'. {prefab.name} ({prefab.GetInstanceID()}). Is _assetProvide disposed: {_assetProvider == null}. If true, then somebody disposed reference during loading new one");

            return component;
        }

        private GameObject CreateInstance(GameObject prefab, Transform parent)
        {
            var gameObject = GetPrefabInstance(prefab, parent);
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            return gameObject;
        }

        private GameObject GetPrefabInstance<T>(T prefab, Transform parent) where T : UnityEngine.Object
        {
            var instantiate = UnityEngine.Object.Instantiate(prefab, parent);
            switch (instantiate)
            {
                case GameObject go:
                    return go;
                case Component component:
                    return component.gameObject;
                default:
                    Debug.LogError(
                        $"Trying to instantiate {typeof(T)} but supports only GameObject and Components");
                    return null;
            }
        }
    }
}