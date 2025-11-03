using System;
using Infra.AssetManagement.ViewLoader;
using Infra.ControllersTree.Abstractions;
using UnityEngine;
using VContainer;

namespace Core.EntryPoint
{
    public static class RegistrationHelpers
    {
        public static void RegisterController<T>(this IContainerBuilder builder) where T : IBaseController
        {
            builder.Register<T>(Lifetime.Transient).AsSelf().AsImplementedInterfaces();
        }

        public static void RegisterController<TAbstraction, TImplementation>(this IContainerBuilder builder)
            where TAbstraction : IBaseController where TImplementation : TAbstraction
        {
            builder.Register<TAbstraction, TImplementation>(Lifetime.Transient).AsImplementedInterfaces();
        }
        
        public static void RegisterSelfFactory<T>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Transient)
        {
            builder.RegisterFactory<T>(
                resolver => () => resolver.Resolve<T>(),
                lifetime);
        }
    }
}