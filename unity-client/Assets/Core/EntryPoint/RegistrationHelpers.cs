using Infra.ControllersTree.Abstractions;
using VContainer;

namespace Core.EntryPoint
{
    public static class RegistrationHelpers
    {
        public static void RegisterController<T>(this IContainerBuilder builder) where T : IBaseController
        {
            builder.Register<T>(Lifetime.Transient).AsSelf().AsImplementedInterfaces();
        }
        
        public static void RegisterController<TAbstraction, TImplementation>(this IContainerBuilder builder) where TAbstraction : IBaseController where  TImplementation : TAbstraction
        {
            builder.Register<TAbstraction, TImplementation>(Lifetime.Transient).AsImplementedInterfaces();
        }
    }
}