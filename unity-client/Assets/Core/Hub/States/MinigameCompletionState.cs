using System;
using System.Threading;
using Core.Hub.Views.Minigame.MinigameCompletion;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.AssetManagement.ViewLoader;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using VContainer;

namespace Core.Hub.States
{
    public class MinigameCompletionState : IStateController<MinigameCompletionPayload>
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();

        private readonly IViewLoader<IMinigameCompletionView> _viewLoader;
        private readonly IObjectResolver _resolver;
        
        private IMinigameCompletionView _view;

        public MinigameCompletionState(
            IViewLoader<IMinigameCompletionView> viewLoader,
            IObjectResolver resolver)
        {
            _viewLoader = viewLoader;
            _resolver = resolver;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        
        public async UniTask OnStart(MinigameCompletionPayload payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _view = await _viewLoader.Load(resources, token, null);
            
            _view.SetData(payload);
            
            _view.OnContinueButtonPressed += OnContinueClicked;
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        private void OnContinueClicked()
        {
            _machineInstructionCompletionSource.TrySetResult(
                StateMachineInstructionSugar.GoTo<MinigameResultsState, MinigameResultsPayload>(_resolver, BuildResultsPayload()));
        }

        private MinigameResultsPayload BuildResultsPayload()
        {
            throw new NotImplementedException();
        }

        public UniTask OnStop(CancellationToken token)
        {
            _view.OnContinueButtonPressed -= OnContinueClicked;

            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}