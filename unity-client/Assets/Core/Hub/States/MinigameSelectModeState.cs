using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Common.Minigames.Models;
using Core.Hub.UI;
using Core.Hub.Views;
using Cysharp.Threading.Tasks;
using Infra.AssetManagement.ViewLoader;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;

namespace Core.Hub.States
{
    public class MinigameSelectModeState : IStateController<SelectModeStatePayload>
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();

        private readonly GameContext _gameContext;
        private readonly IViewLoader<IMinigameModesView> _viewLoader;
        private readonly IViewLoader<IMinigameModeItemView> _modesViewLoader;

        private readonly List<IMinigameModeItemView> _spawnedViews = new();
        private readonly Dictionary<IMinigameModeItemView, Action> _clickHandlers = new();

        private IControllerResources _resources;
        private MinigameModel _model;
        private IMinigameModesView _view;
        
        public MinigameSelectModeState(
            GameContext gameContext,
            IViewLoader<IMinigameModesView> viewLoader,
            IViewLoader<IMinigameModeItemView> modesViewLoader)
        {
            _gameContext = gameContext;
            _viewLoader = viewLoader;
            _modesViewLoader = modesViewLoader;
        }
        
        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        
        public async UniTask OnStart(SelectModeStatePayload payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _model = payload.MinigameModel;
            _resources = resources;
            
            _view = await _viewLoader.Load(resources, token, payload.ViewParent);
            _view.SetIcon(payload.MinigameIcon);
            _view.OnBackButtonClicked += OnBackButtonClicked;
            
            await SpawnModesViews(token);
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        public async UniTask OnStop(CancellationToken token)
        {
            ClearClickHandlers();
            _spawnedViews.Clear();
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private UniTask SpawnModesViews(CancellationToken token)
        {
            return UniTask.WhenAll(Enumerable.Select(_model.Modes, mode => CreateModeItemView(mode, token)));
        }

        private async UniTask CreateModeItemView(MinigameModeModel model, CancellationToken token)
        {
            var itemView = await _modesViewLoader.Load(_resources, token, _view.ModesContainer);
            
            itemView.SetData(model);
            itemView.OnPlayButtonClicked += OnClickHandler;
            _clickHandlers[itemView] = OnClickHandler;
            
            _spawnedViews.Add(itemView);
            
            return;

            void OnClickHandler()
            {
                _gameContext.SelectMode(model);
                _machineInstructionCompletionSource.TrySetResult(StateMachineInstruction.GoBack);
            }
        }
        
        private void OnBackButtonClicked()
        {
            _machineInstructionCompletionSource.TrySetResult(StateMachineInstruction.GoBack);
        }

        private void ClearClickHandlers()
        {
            foreach (var (view, handler) in _clickHandlers)
            {
                view.OnPlayButtonClicked -= handler;
            }

            _clickHandlers.Clear();
        }
    }
}