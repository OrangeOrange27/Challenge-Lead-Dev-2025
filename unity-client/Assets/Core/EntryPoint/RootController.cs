using System.Threading;
using Core.Hub;
using Core.SplashScreen;
using Cysharp.Threading.Tasks;
using Infra;
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