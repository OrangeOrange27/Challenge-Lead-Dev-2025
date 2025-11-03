using Core.Hub;
using Core.SplashScreen;
using Infra;
using Infra.AssetManagement.AssetProvider;
using Infra.AssetManagement.DataProvider;
using Infra.AssetManagement.DataProvider.Storage;
using Infra.AssetManagement.ViewLoader;
using Infra.Encoding;
using Infra.Serialization;
using Infra.StateMachine;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using JsonSerializer = Infra.Serialization.JsonSerializer;

namespace Core.EntryPoint
{
    public class ServicesInstaller : LifetimeScope
    {
        [SerializeField] private SplashSceneView _splashSceneView;

        public static readonly JsonSerializerSettings JsonSettings = new()
        {
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ISerializer, JsonSerializer>(Lifetime.Singleton);
            RegisterDataProvider(builder);

            builder.RegisterController<StateMachineController>();
            builder.RegisterController<RootController>();

            builder.Register<IAssetProvider, AddressablesAssetProvider>(Lifetime.Transient);
            builder.RegisterSelfFactory<IAssetProvider>();

            builder.RegisterViewLoader<IHubView, HubView>("HubView");
        }

        private void RegisterDataProvider(IContainerBuilder builder)
        {
            builder.Register<IDataProvider, DataProviderBase>(Lifetime.Singleton);
            builder.Register<IDataStorage, PlayerDataFileDataStorage>(Lifetime.Singleton);
            builder.Register<IEncoder, GenericEncoder>(Lifetime.Singleton);
        }
    }
}