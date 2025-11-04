using System.Threading;
using Cysharp.Threading.Tasks;
using Infra.ControllersTree;
using UnityEngine;
using VContainer;

namespace Core.EntryPoint
{
    public class EntryPoint : ServicesInstaller
    {
        protected override void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Input.multiTouchEnabled = false;

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterBuildCallback(resolver => Initialize(resolver).Forget());
        }

        private async UniTask Initialize(IObjectResolver resolver)
        {
            var runner = ControllersTreeBootstrap.Create(resolver.Resolve<RootController>());
            await runner.Initialize(CancellationToken.None);
            await runner.Start(default, CancellationToken.None);
            runner.Execute(CancellationToken.None).Forget((_) =>
            {
                //mute exception because it will be processed in LoggerControllerRunner.cs
            });
        }
    }
}