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
using Common.Server.DTOs;
using Core.Hub.States;
using Core.Hub.UI;
using Core.Hub.UI.Components;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    // Ideally, this state would be split into smaller states for better maintainability. (HubResultsState and HubMinigamesState)
    // However, due to time constraints, it has been implemented as a single state for now.
    public class RootHubState : IStateController<EmptyPayloadType>
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();

        private readonly IObjectResolver _resolver;
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerDataService _playerDataService;
        private readonly GameContext _gameContext;
        private readonly ISharedViewLoader<IHubView> _hubViewLoader;
        private readonly IViewLoader<IMinigameItemView> _minigamesItemViewLoader;
        private readonly IViewLoader<IResultsItemView> _resultsItemViewLoader;
        private readonly IConfigProvider<MinigamesConfig> _minigamesConfigProvider;

        private readonly Dictionary<IMinigameItemView, Action> _minigameClickHandlers = new();
        private readonly Dictionary<IResultsItemView, Action> _resultClickHandlers = new();
        private readonly List<IMinigameItemView> _minigameViews = new();

        private IHubView _hubView;
        private List<MinigameModel> _minigames;
        private List<MatchHistoryItem> _results;

        private IControllerChildren _controllerChildren;
        private CancellationTokenSource _animCts;

        public RootHubState(
            IObjectResolver resolver,
            IAssetProvider assetProvider,
            GameContext gameContext,
            IPlayerDataService playerDataService,
            ISharedViewLoader<IHubView> hubViewLoader,
            IViewLoader<IMinigameItemView> minigamesItemViewLoader,
            IViewLoader<IResultsItemView> resultsItemViewLoader,
            IConfigProvider<MinigamesConfig> minigamesConfigProvider)
        {
            _resolver = resolver;
            _assetProvider = assetProvider;
            _gameContext = gameContext;
            _playerDataService = playerDataService;
            _hubViewLoader = hubViewLoader;
            _minigamesItemViewLoader = minigamesItemViewLoader;
            _minigamesConfigProvider = minigamesConfigProvider;
            _resultsItemViewLoader = resultsItemViewLoader;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(EmptyPayloadType payload, IControllerResources resources,
            IControllerChildren controllerChildren,
            CancellationToken token)
        {
            await FetchDataAsync();

            _hubView = await _hubViewLoader.Load(resources, token, null);

            _hubView.MinigamesHolder.gameObject.SetActive(true);
            _hubView.ResultsView.gameObject.SetActive(false);

            _hubView.BottomPanel.OnTabSelected += OnTabSelected;

            OnBalanceChanged(CurrencyType.Gems, _playerDataService.PlayerData.Gems);
            OnBalanceChanged(CurrencyType.Cash, _playerDataService.PlayerData.Cash);
            _playerDataService.OnBalanceChanged += OnBalanceChanged;


            await SpawnViewsAsync(resources, token);
        }

        private UniTask FetchDataAsync()
        {
            return UniTask.WhenAll(GetMinigames().ContinueWith(m => _minigames = m),
                FetchResultsFromServer().ContinueWith(r => _results = r));
        }

        private void OnTabSelected(int tabIndex)
        {
            _animCts?.Cancel();
            _animCts = new CancellationTokenSource();

            switch (tabIndex)
            {
                case 0:
                    SelectMinigamesPanel(_animCts.Token).Forget();
                    break;
                case 1:
                    SelectResultsPanel(_animCts.Token).Forget();
                    break;
                default:
                    Debug.LogError($"Unknown tab index selected: {tabIndex}");
                    break;
            }
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            _controllerChildren = controllerChildren;

            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        public UniTask OnStop(CancellationToken token)
        {
            _playerDataService.OnBalanceChanged -= OnBalanceChanged;
            _hubView.BottomPanel.OnTabSelected -= OnTabSelected;

            _hubView.MinigamesHolder.gameObject.SetActive(false);
            _hubView.ResultsView.gameObject.SetActive(false);

            ClearClickHandlers();

            _minigameViews.Clear();

            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private UniTask SpawnViewsAsync(IControllerResources resources, CancellationToken token)
        {
            return UniTask.WhenAll(SpawnMinigameViews(_minigames, resources, token),
                SpawnResultItems(_results, resources, token));
        }

        private async UniTask SpawnResultItems(List<MatchHistoryItem> items, IControllerResources resources,
            CancellationToken token)
        {
            await UniTask.WhenAll(Enumerable.Select(items, item => CreateResultItemView(item, resources, token)));
        }

        private async UniTask CreateResultItemView(MatchHistoryItem item, IControllerResources resources,
            CancellationToken token)
        {
            if (item?.reward == null || item?.reward?.amount <= 0)
                return;

            var container = item.rewardClaimed
                ? _hubView.ResultsView.ClaimedResultsContainer
                : _hubView.ResultsView.ReadyForClaimResultsContainer;

            var view = await _resultsItemViewLoader.Load(resources, token, container);

            view.SetData(item.gameName, null, item.timeAgo, ServerDataAdapter.FromServer(item.reward),
                item.rewardClaimed, !item.rewardClaimed);

            view.OnClaimButtonClicked += OnClickHandler;
            _resultClickHandlers[view] = OnClickHandler;

            return;

            async void OnClickHandler()
            {
                view.SetHighlighted(false);
                view.Transform.parent = _hubView.ResultsView.ClaimedResultsContainer;

                var response =
                    await ServerAPI.Matches.ClaimRewardAsync(item.matchId, _playerDataService.PlayerData.AuthToken);
                var reward = ServerDataAdapter.FromServer(response.reward);

                _playerDataService.GiveBalance(reward.CurrencyType, (int)reward.Amount);
            }
        }

        private async UniTask SpawnMinigameViews(List<MinigameModel> models, IControllerResources resources,
            CancellationToken token)
        {
            await UniTask.WhenAll(Enumerable.Select(models, model => CreateMinigameView(model, resources, token)));
        }

        private async UniTask CreateMinigameView(MinigameModel model, IControllerResources resources,
            CancellationToken token)
        {
            var view = await _minigamesItemViewLoader.Load(resources, token, _hubView.MinigamesHolder.transform);

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
            _minigameClickHandlers[view] = OnClickHandler;

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
            _hubView.BottomPanel.gameObject.SetActive(false);

            await _controllerChildren
                .Create<MinigameSelectModeState, SelectModeStatePayload>(_resolver)
                .RunToDispose(payload, token);

            if (_gameContext.SelectedMinigameConfiguration?.MinigameModel == null
                || _gameContext.SelectedMinigameConfiguration?.GameMode == null)
            {
                _hubView.MinigamesHolder.gameObject.SetActive(true);
                _hubView.BottomPanel.gameObject.SetActive(true);

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
            if (minigames == null || minigames.Count == 0)
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

        private async UniTask<List<MatchHistoryItem>> FetchResultsFromServer()
        {
            var response = await ServerAPI.Player.GetPlayerHistoryAsync(_playerDataService.PlayerData.AuthToken);

            var results = new List<MatchHistoryItem>();
            results.AddRange(response.history.pastMatches);
            results.AddRange(response.history.pendingMatches);
            results.AddRange(response.history.rewardsToClaim);

            return results;
        }

        private async UniTask SelectMinigamesPanel(CancellationToken token)
        {
            var minigames = _hubView.MinigamesHolder;
            var results = _hubView.ResultsView;

            minigames.gameObject.SetActive(true);
            minigames.transform.localPosition = new Vector3(Screen.width * 3, 0, 0);

            var seq = DOTween.Sequence();
            seq.Join(results.transform.DOLocalMoveX(-Screen.width * 3, 0.5f).SetEase(Ease.InOutQuart));
            seq.Join(results.CanvasGroup.DOFade(0, 0.4f));
            seq.Join(minigames.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
            seq.Join(minigames.DOFade(1, 0.4f));

            await seq.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);

            results.gameObject.SetActive(false);
        }

        private async UniTask SelectResultsPanel(CancellationToken token)
        {
            var minigames = _hubView.MinigamesHolder;
            var results = _hubView.ResultsView;

            results.gameObject.SetActive(true);
            results.transform.localPosition = new Vector3(-Screen.width * 3, 0, 0);

            var seq = DOTween.Sequence();
            seq.Join(minigames.transform.DOLocalMoveX(Screen.width * 3, 0.5f).SetEase(Ease.InOutQuart));
            seq.Join(minigames.DOFade(0, 0.4f));
            seq.Join(results.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
            seq.Join(results.CanvasGroup.DOFade(1, 0.4f));

            await seq.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);

            minigames.gameObject.SetActive(false);
        }

        private void OnBalanceChanged(CurrencyType assetType, int amount)
        {
            switch (assetType)
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
            foreach (var kvp in _minigameClickHandlers)
                kvp.Key.OnClick -= kvp.Value;

            _minigameClickHandlers.Clear();

            foreach (var kvp in _resultClickHandlers)
                kvp.Key.OnClaimButtonClicked -= kvp.Value;

            _resultClickHandlers.Clear();
        }
    }
}