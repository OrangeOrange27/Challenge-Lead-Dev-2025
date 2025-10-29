using System;
using System.Collections.Generic;

namespace Infra.ControllersTree.Abstractions
{
    public interface IControllerChildren
    {
        IControllerRunner<TPayload, TResult> Create<TController, TPayload, TResult>(Func<TController> factory)
            where TController : IControllerWithPayloadAndReturn<TPayload, TResult>;
        
        IEnumerable<IControllerRunnerBase> GetChildrenRunners(IBaseController controller);
    }
}