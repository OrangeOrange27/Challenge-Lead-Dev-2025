using Core.Hub;
using Core.Hub.UI;
using Core.SplashScreen;
using Infra;
using Infra.AssetManagement.AssetProvider;
using Infra.AssetManagement.DataProvider;
using Infra.AssetManagement.DataProvider.Storage;
using Infra.AssetManagement.ViewLoader;
using Infra.Encoding;
using Infra.Serialization;
using Infra.StateMachine;
using Minigames;
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
            builder.RegisterInstance(JsonSettings);
            builder.Register<ISerializer, JsonSerializer>(Lifetime.Singleton);
            RegisterDataProvider(builder);
            
            RegisterControllersTree(builder);
            
            builder.Register<IAssetProvider, AddressablesAssetProvider>(Lifetime.Transient);
            builder.RegisterSelfFactory<IAssetProvider>();

            builder.RegisterInstance(_splashSceneView);

            builder.RegisterConfig<MinigamesConfig>("minigames_config"); //todo: add remote link
            
            RegisterHub(builder);
        }
        
        private void RegisterControllersTree(IContainerBuilder builder)
        {
            builder.RegisterController<StateMachineController>();
            builder.RegisterController<RootController>();
            builder.RegisterController<InitializeGameAfterAuthController>();
            builder.RegisterController<InitializeGameBeforeAuthController>();
        }

        private void RegisterHub(IContainerBuilder builder)
        {
            builder.RegisterController<RootHubState>();

            builder.RegisterViewLoader<IHubView, HubView>("HubView");
            builder.RegisterViewLoader<IMinigameItemView, MinigameItemView>("MinigameItemView");
        }

        private void RegisterDataProvider(IContainerBuilder builder)
        {
            builder.Register<IDataProvider, DataProviderBase>(Lifetime.Singleton);
            builder.Register<IDataStorage, PlayerDataFileDataStorage>(Lifetime.Singleton);
            builder.Register<IEncoder, GenericEncoder>(Lifetime.Singleton);
        }
    }
}