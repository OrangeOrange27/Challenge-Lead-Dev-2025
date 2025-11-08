using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Common.ConfigSystem;
using Common.Minigames;
using Common.Minigames.Models;
using Common.Models.Economy;
using Common.Server;
using Core.Hub.States;
using Core.Hub.UI;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.AssetManagement.AssetProvider;
using Infra.AssetManagement.ViewLoader;
using Infra.ControllersTree;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using UnityEngine;
using VContainer;

namespace Core.Hub
{
    public class RootHubState : IStateController<EmptyPayloadType>
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();

        private readonly IObjectResolver _resolver;
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerDataService _playerDataService;
        private readonly GameContext _gameContext;
        private readonly IViewLoader<IHubView> _hubViewLoader;
        private readonly IViewLoader<IMinigameItemView> _minigamesItemViewLoader;
        private readonly IConfigProvider<MinigamesConfig> _minigamesConfigProvider;

        private readonly Dictionary<IMinigameItemView, Action> _clickHandlers = new();
        private readonly List<IMinigameItemView> _minigameViews = new();

        private IHubView _hubView;
        private List<MinigameModel> _minigames;

        private IControllerResources _resources;
        private IControllerChildren _controllerChildren;

        public RootHubState(
            IObjectResolver resolver,
            IAssetProvider assetProvider,
            GameContext gameContext,
            IPlayerDataService playerDataService,
            IViewLoader<IHubView> hubViewLoader,
            IViewLoader<IMinigameItemView> minigamesItemViewLoader,
            IConfigProvider<MinigamesConfig> minigamesConfigProvider)
        {
            _resolver = resolver;
            _assetProvider = assetProvider;
            _gameContext = gameContext;
            _playerDataService = playerDataService;
            _hubViewLoader = hubViewLoader;
            _minigamesItemViewLoader = minigamesItemViewLoader;
            _minigamesConfigProvider = minigamesConfigProvider;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(EmptyPayloadType payload, IControllerResources resources,
            IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _minigames = await GetMinigames();

            _hubView = await _hubViewLoader.Load(resources, token, null);

            _hubView.MinigamesHolder.gameObject.SetActive(true);
            
            OnBalanceChanged(CurrencyType.Gems, _playerDataService.PlayerData.Gems);
            OnBalanceChanged(CurrencyType.Cash, _playerDataService.PlayerData.Cash);
            _playerDataService.OnBalanceChanged += OnBalanceChanged;

            await SpawnMinigameViews(_minigames, resources, token);
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            _resources = resources;
            _controllerChildren = controllerChildren;

            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        public UniTask OnStop(CancellationToken token)
        {
            _playerDataService.OnBalanceChanged -= OnBalanceChanged;

            _hubView.MinigamesHolder.gameObject.SetActive(false);

            ClearClickHandlers();

            _minigameViews.Clear();

            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private async UniTask SpawnMinigameViews(List<MinigameModel> models, IControllerResources resources,
            CancellationToken token)
        {
            await UniTask.WhenAll(Enumerable.Select(models, model => CreateMinigameView(model, resources, token)));
        }

        private async UniTask CreateMinigameView(MinigameModel model, IControllerResources resources,
            CancellationToken token)
        {
            var view = await _minigamesItemViewLoader.Load(resources, token, _hubView.MinigamesHolder);

            if (view == null)
            {
                Debug.LogError($"Failed to create minigame view for minigame: {model.Id}");
                return;
            }

            var icon = await _assetProvider.LoadAsync<Sprite>(model.IconId, token);

            if (icon == null)
            {
                Debug.LogError($"Minigame icon not found: {model.IconId}");
            }

            view.SetImage(icon);
            view.OnClick += OnClickHandler;
            _clickHandlers[view] = OnClickHandler;

            _minigameViews.Add(view);

            return;

            void OnClickHandler()
            {
                Debug.Log($"Minigame clicked: {model.Id}");

                _gameContext.SelectMinigame(model, icon);

                OnMinigameClick(model.Id, icon, token).Forget();
            }
        }

        private async UniTask OnMinigameClick(string id, Sprite icon, CancellationToken token)
        {
            var minigameModel = _minigames.FirstOrDefault(m => m.Id == id);
            var payload = new SelectModeStatePayload
            {
                MinigameModel = minigameModel,
                ViewParent = _hubView.MainPanel,
                MinigameIcon = icon
            };
            
            _hubView.MinigamesHolder.gameObject.SetActive(false);

            await _controllerChildren
                .Create<MinigameSelectModeState, SelectModeStatePayload>(_resolver)
                .RunToDispose(payload, token);

            if (_gameContext.SelectedMinigameConfiguration?.MinigameModel == null
                || _gameContext.SelectedMinigameConfiguration?.GameMode == null)
            {
                _hubView.MinigamesHolder.gameObject.SetActive(true);

                return;
            }
            
            LaunchMinigame(_gameContext.SelectedMinigameConfiguration);
        }

        private void LaunchMinigame(MinigameBootstrapPayload payload)
        {
            if (payload.MinigameModel == null)
            {
                Debug.LogError($"Minigame model not found for id: {payload.MinigameModel.Id}");
                return;
            }

            _machineInstructionCompletionSource.TrySetResult(
                StateMachineInstructionSugar.GoTo<MinigameLoadingState, MinigameBootstrapPayload>(_resolver, payload));
        }

        private async UniTask<List<MinigameModel>> GetMinigames()
        {
            var minigames = await FetchMinigamesFromServer();
            if(minigames == null || minigames.Count == 0)
            {
                Debug.LogError("Failed to fetch minigames from server or no minigames available.");
                
                var minigamesConfig = _minigamesConfigProvider.Get();
                minigames = minigamesConfig.Minigames;
            }

            return minigames;
        }

        private async UniTask<List<MinigameModel>> FetchMinigamesFromServer()
        {
            var response = await ServerAPI.Minigames.GetGamesAsync(_playerDataService.PlayerData.AuthToken);
            
            var minigames = response.games?.Select(g => new MinigameModel
            {
                Id = g.id,
                IconId = g.iconId,
                Modes = g.modes.Select(ServerDataAdapter.FromServer).ToList()
            });
            
            return minigames?.ToList();
        }

        private void OnBalanceChanged(CurrencyType assetType, int amount)
        {
            switch(assetType)
            {
                case CurrencyType.Gems:
                    _hubView.TopPanel.UpdateGems(amount);
                    break;
                case CurrencyType.Cash:
                    _hubView.TopPanel.UpdateBalance(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null);
            }
        }

        private void ClearClickHandlers()
        {
            foreach (var kvp in _clickHandlers)
                kvp.Key.OnClick -= kvp.Value;

            _clickHandlers.Clear();
        }
    }
}