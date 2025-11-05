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

            await SpawnParticipantsScoreItemViews(token);
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
                .OrderByDescending(participant => participant.Result.TotalPoints);
            
            await UniTask.WhenAll(Enumerable.Select(sortedParticipants,
                model => CreateParticipantItemView(model, token)));
        }

        private async UniTask CreateParticipantItemView(MinigameParticipantModel model, CancellationToken token)
        {
            var itemView = await _scoreItemViewLoader.Load(_resources, token, _view.LeaderboardContent);
            itemView.SetData(model, GetRewardForParticipant(model), model.UserId == _payload.LocalPlayer.UserId);
        }

        private RewardModel GetRewardForParticipant(MinigameParticipantModel participant)
        {
            // For demo purposes, everyone gets the same reward
            return new RewardModel
            {
                CurrencyType = CurrencyType.Gems,
                Amount = 100
            };
        }
    }
}