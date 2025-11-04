using System;
using System.Linq;
using Infra.ControllersTree.Abstractions;
using Infra.ControllersTree.Implementations;

namespace Infra.ControllersTree.Settings
{
	public class DefaultControllerSettings : IControllerSettings
	{
		public IControllerRunner<TPayload, TResult> GetRunner<TController, TPayload, TResult>(
			IControllerRunnerBase parentControllerRunner,
			Func<TController> factory)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			var controller = factory.Invoke();
			if (controller == null)
				throw new ControllerException($"{typeof(TController).FullName} factory invoking returned null");

			return CreateRunner<TController, TPayload, TResult>(parentControllerRunner, controller);
		}

		public IControllerRunner<TPayload, TResult> GetRunner<TPayload, TResult>(IControllerRunnerBase parentControllerRunner,
			Func<IBaseController> factory)
		{
			var controller = factory.Invoke();
			if (controller == null)
				throw new ControllerException($"{typeof(IBaseController).FullName} factory invoking returned null");

			var instance = factory.Invoke();
			var baseType = instance.GetType();
			var genericTypes = baseType.GetInterfaces()
				.First(inter => inter.Name.Contains("IControllerWithPayloadAndReturn")).GenericTypeArguments;
			var generics = new Type[genericTypes.Length + 1];
			generics[0] = baseType;
			genericTypes.CopyTo(generics, 1);

			var mi = typeof(DefaultControllerSettings).GetMethods().First(info => info.Name == nameof(CreateRunner));
			var methodRef = mi.MakeGenericMethod(generics);
			return (IControllerRunner<TPayload, TResult>)methodRef.Invoke(this, new object[] { parentControllerRunner, instance });
		}

		private IControllerRunner<TPayload, TResult> CreateRunner<TController, TPayload, TResult>(
			IControllerRunnerBase parentControllerRunner,
			TController controller)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			return new ControllerRunner<TPayload, TResult>(parentControllerRunner?.PathInTree ?? string.Empty,
				controller,
				runner => new ControllerChildren(runner, this),
				new ControllerResources());
		}
	}
}