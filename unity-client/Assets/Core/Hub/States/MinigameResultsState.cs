using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Models;
using Common.Models.Economy;
using Core.Hub.UI.Components;
using Core.Hub.Views.Minigame.MinigameResults;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.AssetManagement.ViewLoader;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using VContainer;

namespace Core.Hub.States
{
    public class MinigameResultsState : IStateController<MinigameResultsPayload>
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();
        private readonly IViewLoader<IMinigameResultsView> _viewLoader;
        private readonly IViewLoader<IScoreItemView> _scoreItemViewLoader;
        private readonly IObjectResolver _resolver;
        private readonly List<IScoreItemView> _spawnedScoreItemViews = new();

        private IMinigameResultsView _view;
        private MinigameResultsPayload _payload;
        private IControllerResources _resources;

        public MinigameResultsState(
            IViewLoader<IMinigameResultsView> viewLoader,
            IViewLoader<IScoreItemView> scoreItemViewLoader,
            IObjectResolver resolver)
        {
            _viewLoader = viewLoader;
            _scoreItemViewLoader = scoreItemViewLoader;
            _resolver = resolver;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(MinigameResultsPayload payload, IControllerResources resources,
            IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _payload = payload;
            _resources = resources;

            _view = await _viewLoader.Load(resources, token, null);
            _view.OnCloseButtonClicked += OnCloseClicked;
            _view.SetIcon(_payload.MinigameIcon);
            
            await _view.PlayShowAnimation(token);
            await SpawnParticipantsScoreItemViews(token);
            await PlayScoreItemsAnimation(token);
        }

        private void OnCloseClicked()
        {
            _machineInstructionCompletionSource.TrySetResult(
                StateMachineInstructionSugar.GoTo<RootHubState>(_resolver));
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        public UniTask OnStop(CancellationToken token)
        {
            _view.OnCloseButtonClicked -= OnCloseClicked;

            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private async UniTask SpawnParticipantsScoreItemViews(CancellationToken token)
        {
            var participants = new List<MinigameParticipantModel>(_payload.Participants) { _payload.LocalPlayer };

            var sortedParticipants = participants
                .GroupBy(p => p.UserId)
                .Select(g => g.First())
                .OrderByDescending(p => p.Result.TotalPoints);

            await UniTask.WhenAll(Enumerable.Select(sortedParticipants,
                (m, i) => CreateParticipantItemView(m, i, token)));
        }
        
        private async UniTask PlayScoreItemsAnimation(CancellationToken token)
        {
            foreach (var itemView in _spawnedScoreItemViews)
            {
                await itemView.PlayAppearAnimation(token);
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
            }
        }

        private async UniTask CreateParticipantItemView(MinigameParticipantModel model, int rank,
            CancellationToken token)
        {
            var itemView = await _scoreItemViewLoader.Load(_resources, token, _view.LeaderboardContent);
            itemView.SetData(model, GetRewardForRank(rank), model.UserId == _payload.LocalPlayer.UserId);
            _spawnedScoreItemViews.Add(itemView);
        }

        private RewardModel GetRewardForRank(int rank)
        {
            return _payload.GameMode.Prizes[rank];
        }
    }
}