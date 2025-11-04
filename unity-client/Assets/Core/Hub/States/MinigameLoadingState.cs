using System.Threading;
using Common.Minigames;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using VContainer;

namespace Core.Hub.States
{
    public class MinigameLoadingState : IStateController<MinigameBootstrapPayload>
    {
        private readonly IObjectResolver _resolver;

        private MinigameBootstrapPayload _payload; 
        
        public MinigameLoadingState(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        
        public UniTask OnStart(MinigameBootstrapPayload payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _payload = payload;
            
            return UniTask.CompletedTask;
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return StateMachineInstructionSugar.GoTo<RootMinigameController, MinigameBootstrapPayload>(_resolver, _payload);
        }

        public UniTask OnStop(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}