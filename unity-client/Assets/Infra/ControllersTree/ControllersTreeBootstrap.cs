using Infra.ControllersTree.Abstractions;
using Infra.ControllersTree.Implementations;
using Infra.ControllersTree.Settings;

namespace Infra.ControllersTree
{
    public static class ControllersTreeBootstrap
    {
        public static IControllerRunner<TPayload, TResult> Create<TPayload, TResult>(
            IControllerWithPayloadAndReturn<TPayload, TResult> initialController,
            IControllerSettings controllerSettings = null)
        {
            var controllerChildren = new ControllerChildren(null, controllerSettings ?? new DefaultControllerSettings());

            var runner =
                controllerChildren.Create<RootController<TPayload, TResult>, TPayload, TResult>(() => new RootController<TPayload, TResult>(initialController));
            return runner;
        }
    }
}