using System;
using System.Threading;
using Common.Minigames.Models;
using Core.Hub.States;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.ControllersTree;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using VContainer;

namespace Common.Minigames
{
    public class RootMinigameController : IStateController<MinigameBootstrapPayload>
    {
        private readonly IObjectResolver _resolver;
        private readonly Func<MinigameModel, IMinigameFlow> _flowFactory;

        private MinigameBootstrapPayload _minigameModel;

        public RootMinigameController(IObjectResolver resolver, Func<MinigameModel, IMinigameFlow> flowFactory)
        {
            _resolver = resolver;
            _flowFactory = flowFactory;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStart(MinigameBootstrapPayload payload, IControllerResources resources,
            IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _minigameModel = payload;

            return UniTask.CompletedTask;
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            var result = await controllerChildren.Create<MinigameModel, MinigameResult>(ConvertFactory(_flowFactory))
                .RunToDispose(_minigameModel.MinigameModel, token);

            var minigameCompletionPayload = new MinigameCompletionPayload()
            {
                MinigameIcon = _minigameModel.MinigameIcon,
                Result = result
            };

            return StateMachineInstructionSugar.GoTo<MinigameCompletionState, MinigameCompletionPayload>(_resolver,
                minigameCompletionPayload);
        }

        public UniTask OnStop(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private Func<IMinigameFlow> ConvertFactory(Func<MinigameModel, IMinigameFlow> flowFactory)
        {
            var model = _minigameModel;
            return () => flowFactory(model.MinigameModel);
        }
    }
}