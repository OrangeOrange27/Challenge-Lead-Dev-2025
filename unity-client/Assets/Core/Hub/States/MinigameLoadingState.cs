using System.Threading;
using Cysharp.Threading.Tasks;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;

namespace Core.Hub.States
{
    public class MinigameLoadingState : IStateController<string>
    {
        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnStop(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnDispose(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnStart(string payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}