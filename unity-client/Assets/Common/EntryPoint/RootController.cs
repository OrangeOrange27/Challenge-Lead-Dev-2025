using System.Threading;
using Core.Hub;
using Core.SplashScreen;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.ControllersTree;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using UnityEngine;
using VContainer;

namespace Core.EntryPoint
{
    public class RootController : IStateController
    {
        private IControllerRunner<IStateMachineInstruction, IStateMachineInstruction> _stateMachineRunner;
        private readonly IObjectResolver _objectResolver;
        
        private readonly SplashSceneView _splashSceneView;
        private readonly GameInitManager _gameInitManager;

        public RootController(IObjectResolver objectResolver, SplashSceneView splashSceneView, GameInitManager gameInitManager)
        {
            _objectResolver = objectResolver;
            _splashSceneView = splashSceneView;
            _gameInitManager = gameInitManager;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStop(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(EmptyPayloadType payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;
            
            await controllerChildren.Create<InitializeGameBeforeAuthController, EmptyPayloadType, EmptyPayloadType>(_objectResolver)
                .RunToDispose(default, token);

            await _gameInitManager.Login();
            
            await controllerChildren.Create<InitializeGameAfterAuthController, EmptyPayloadType, EmptyPayloadType>(_objectResolver)
                .RunToDispose(default, token);
            
            var initialState = StateMachineInstruction.GoToMany(
                //todo: add states
                StateMachineInstructionSugar.GoTo<RootHubState>(_objectResolver));
            
            _stateMachineRunner = controllerChildren.Create<StateMachineController, IStateMachineInstruction>(_objectResolver);
            await _stateMachineRunner.Initialize(token);
            await _stateMachineRunner.Start(initialState, token);
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            _splashSceneView.HideView().Forget();
            return await _stateMachineRunner.Execute(token);
        }
    }
}