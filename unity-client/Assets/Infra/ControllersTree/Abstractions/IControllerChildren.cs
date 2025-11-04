using System;
using System.Collections.Generic;

namespace Infra.ControllersTree.Abstractions
{
    public interface IControllerChildren
    {
        IControllerRunner<TPayload, TResult> Create<TController, TPayload, TResult>(Func<TController> factory)
            where TController : IControllerWithPayloadAndReturn<TPayload, TResult>;

        IControllerRunner<TPayload, TResult> Create<TPayload, TResult>(Func<IBaseController> factory);
        
        IEnumerable<IControllerRunnerBase> GetChildrenRunners(IBaseController controller);
    }
}