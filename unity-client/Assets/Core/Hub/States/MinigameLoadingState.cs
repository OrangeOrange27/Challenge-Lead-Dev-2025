using System.Linq;
using System.Threading;
using Common.ConfigSystem;
using Common.Minigames;
using Common.Minigames.Models;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using VContainer;

namespace Core.Hub.States
{
    public class MinigameLoadingState : IStateController<MinigameModel>
    {
        private readonly IObjectResolver _resolver;
        private readonly IConfigProvider<MinigamesConfig> _configProvider;
        
        public MinigameLoadingState(IObjectResolver resolver, IConfigProvider<MinigamesConfig> configProvider)
        {
            _resolver = resolver;
            _configProvider = configProvider;
        }
        
        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        
        public UniTask OnStart(MinigameModel payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            var model = _configProvider.Get().Minigames.FirstOrDefault(); //todo: replace with proper model
            
            return StateMachineInstructionSugar.GoTo<RootMinigameController, MinigameModel>(_resolver, model);
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