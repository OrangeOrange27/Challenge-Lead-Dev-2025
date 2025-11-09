using System;
using UnityEngine;
using VContainer;

namespace Infra.AssetManagement.ViewLoader
{
    public static class ViewLoaderDiContainerExtensions
    {
        public static void RegisterViewLoader<TViewInterface, TViewPrefab>(this IContainerBuilder builder,
            string addressablesKey)
            where TViewPrefab : Component, TViewInterface where TViewInterface : class
        {
            builder.RegisterViewLoader<TViewInterface, TViewPrefab, int>(_ => addressablesKey);
        }

        public static void RegisterViewLoader<TViewInterface, TViewPrefab, TKey>(this IContainerBuilder builder,
            Func<TKey, string> convertKeyToAddressablesKey)
            where TViewPrefab : Component, TViewInterface where TViewInterface : class
        {
            builder.Register<ViewLoader<TViewPrefab, TViewInterface, TKey>>(Lifetime.Singleton)
                .WithParameter(convertKeyToAddressablesKey).AsImplementedInterfaces();
        }

        public static void RegisterSharedViewLoader<TViewInterface, TViewPrefab>(this IContainerBuilder builder,
            string addressablesKey)
            where TViewPrefab : Component, TViewInterface where TViewInterface : class
        {
            builder.RegisterViewLoader<TViewInterface, TViewPrefab>(addressablesKey);
            builder.Register<ISharedViewLoader<TViewInterface>, SharedViewLoader<TViewInterface, int>>(
                Lifetime.Singleton);
            builder.RegisterFactory<TViewInterface>(
                resolver => () => resolver.Resolve<ISharedViewLoader<TViewInterface>>().CachedView, Lifetime.Transient);
        }
    }
}