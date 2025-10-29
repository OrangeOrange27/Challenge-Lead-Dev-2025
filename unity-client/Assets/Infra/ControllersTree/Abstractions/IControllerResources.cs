using System;
using System.Collections.Generic;

namespace Infra.ControllersTree.Abstractions
{
    public interface IControllerResources : IDisposable, ICollection<IDisposable>
    {
        void Attach(IDisposable disposable);
    }
}