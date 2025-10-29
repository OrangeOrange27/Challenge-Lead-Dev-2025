using System;
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