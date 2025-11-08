using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Minigames.Models;
using Common.Models;
using Common.Server;
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
        private const float WaitingTimeSeconds = 3f;

        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();

        private readonly IViewLoader<IMinigameCompletionView> _viewLoader;
        private readonly IObjectResolver _resolver;
        private readonly IPlayerDataService _playerDataService;

        private IMinigameCompletionView _view;
        private MinigameResult _playerResult;
        private MinigameCompletionPayload _payload;

        public MinigameCompletionState(
            IPlayerDataService playerDataService,
            IViewLoader<IMinigameCompletionView> viewLoader,
            IObjectResolver resolver)
        {
            _viewLoader = viewLoader;
            _resolver = resolver;
            _playerDataService = playerDataService;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(MinigameCompletionPayload payload, IControllerResources resources,
            IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _payload = payload;
            _playerResult = payload.Result;

            _view = await _viewLoader.Load(resources, token, null);

            _view.SetData(payload);
            _view.SetButtonActive(false);

            _view.OnContinueButtonPressed += OnContinueClicked;
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            await UniTask.WaitForSeconds(WaitingTimeSeconds, cancellationToken: token);
            _view.SetButtonActive(true);

            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        private async void OnContinueClicked()
        {
            _view.OnContinueButtonPressed -= OnContinueClicked;
            
            var payload = await GetResultsPayload();
            
            _machineInstructionCompletionSource.TrySetResult(
                StateMachineInstructionSugar.GoTo<MinigameResultsState, MinigameResultsPayload>(_resolver, payload));
        }

        private async UniTask<MinigameResultsPayload> GetResultsPayload()
        {
            var response = await ServerAPI.Matches.GetLeaderboardAsync(_payload.MatchId, _playerDataService.PlayerData.AuthToken);
            var playerEntry = response.playerEntry;

            return new MinigameResultsPayload()
            {
                Participants = response.entries?.Select(ServerDataAdapter.FromServer).ToList(), //MockParticipants(),
                LocalPlayer = new MinigameParticipantModel
                {
                    UserId = playerEntry.playerId,
                    Name = playerEntry.playerName,
                    AvatarId = playerEntry.playerId,
                    Result = _playerResult
                },
                MinigameIcon = _payload.MinigameIcon,
                GameMode = _payload.GameMode
            };
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

        private List<MinigameParticipantModel> MockParticipants()
        {
            return new List<MinigameParticipantModel>
            {
                new()
                {
                    Name = "player1",
                    AvatarId = "avatar_1",
                    Result = new MinigameResult()
                    {
                        PointsForPrecision = 3121,
                        PointsForReaction = 8201,
                    }
                },
                new()
                {
                    Name = "Jesse Pinkman",
                    AvatarId = "avatar_2",
                    Result = new MinigameResult()
                    {
                        PointsForPrecision = 5121,
                        PointsForReaction = 1201,
                    }
                },
                new()
                {
                    Name = "Roblox Enjoyer",
                    AvatarId = "avatar_1",
                    Result = new MinigameResult()
                    {
                        PointsForPrecision = 0,
                        PointsForReaction = 9041,
                    }
                },
                new()
                {
                    Name = "Gamer123",
                    AvatarId = "avatar_3",
                    Result = new MinigameResult()
                    {
                        PointsForPrecision = 111,
                        PointsForReaction = 9201,
                    }
                }
            };
        }
    }
}