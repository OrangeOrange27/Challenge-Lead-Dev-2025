using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.ConfigSystem;
using Common.Minigames;
using Common.Minigames.Models;
using Common.Models;
using Core.Hub.States;
using Core.Hub.UI;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.AssetManagement.AssetProvider;
using Infra.AssetManagement.ViewLoader;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using Minigames;
using UnityEngine;
using VContainer;

namespace Core.Hub
{
    public class RootHubState : IStateController<EmptyPayloadType>
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();

        private readonly IObjectResolver _resolver;
        private readonly IAssetProvider _assetProvider;
        private readonly IViewLoader<IHubView> _hubViewLoader;
        private readonly IViewLoader<IMinigameItemView> _minigamesItemViewLoader;
        private readonly IConfigProvider<MinigamesConfig> _minigamesConfigProvider;
        
        private readonly Dictionary<IMinigameItemView, Action> _clickHandlers = new();
        private readonly List<IMinigameItemView> _minigameViews = new ();

        private IHubView _hubView;
        
        public RootHubState(IObjectResolver resolver, IAssetProvider assetProvider,
            IViewLoader<IHubView> hubViewLoader,
            IViewLoader<IMinigameItemView> minigamesItemViewLoader,
            IConfigProvider<MinigamesConfig> minigamesConfigProvider)
        {
            _resolver = resolver;
            _assetProvider = assetProvider;
            _hubViewLoader = hubViewLoader;
            _minigamesItemViewLoader = minigamesItemViewLoader;
            _minigamesConfigProvider = minigamesConfigProvider;
        }
        
        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(EmptyPayloadType payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            var minigamesConfig = _minigamesConfigProvider.Get();
            
            _hubView = await _hubViewLoader.Load(resources, token, null);
            
            _hubView.MinigamesHolder.gameObject.SetActive(true);
            
            await SpawnMinigameViews(minigamesConfig.Minigames, resources, token);
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }
        
        public UniTask OnStop(CancellationToken token)
        {
            _hubView.MinigamesHolder.gameObject.SetActive(false);
            
            foreach (var kvp in _clickHandlers)
                kvp.Key.OnClick -= kvp.Value;

            _clickHandlers.Clear();
            _minigameViews.Clear();
            
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private async UniTask SpawnMinigameViews(List<MinigameModel> models, IControllerResources resources, CancellationToken token)
        {
            foreach (var model in models)
            {
                var view = await CreateMinigameView(model, resources, token);
                if (view == null)
                {
                    Debug.LogError($"Failed to create minigame view for minigame: {model.Id}");
                    continue;
                }

                view.OnClick += OnClickHandler;
                _clickHandlers[view] = OnClickHandler;
                
                _minigameViews.Add(view);
                
                continue;
            
                void OnClickHandler()
                {
                    Debug.Log($"Minigame clicked: {model.Id}");
                    OnMinigameClick(model.Id);
                }
            }
        }

        private async UniTask<IMinigameItemView> CreateMinigameView(MinigameModel model, IControllerResources resources,
            CancellationToken token)
        {
            var view = await _minigamesItemViewLoader.Load(resources, token, _hubView.MinigamesHolder);
            var icon = await _assetProvider.LoadAsync<Sprite>(model.IconId, token);

            if (icon == null)
            {
                Debug.LogError($"Minigame icon not found: {model.IconId}");
            }

            return view;
        }

        private void OnMinigameClick(string id)
        {
            var minigameModel = _minigamesConfigProvider.Get().Minigames.FirstOrDefault(m => m.Id == id);
            if (minigameModel == null)
            {
                Debug.LogError($"Minigame model not found for id: {id}");
                return;
            }

            _machineInstructionCompletionSource.TrySetResult(
                StateMachineInstructionSugar.GoTo<MinigameLoadingState, MinigameModel>(_resolver, minigameModel));
        }
    }
}