using System;
using Infra.ControllersTree.Abstractions;

namespace Infra.ControllersTree.Implementations
{
	public class ControllerResources : CompositeDisposable, IControllerResources
	{
		public void Attach(IDisposable disposable)
		{
			Add(disposable);
		}
	}
}
