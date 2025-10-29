using Core.SplashScreen;
using Infra.StateMachine;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
            builder.RegisterController<StateMachineController>();
            builder.RegisterController<RootController>();
        }
    }
}