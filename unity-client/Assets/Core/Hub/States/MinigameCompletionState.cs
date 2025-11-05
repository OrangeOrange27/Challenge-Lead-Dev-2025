using System.Collections.Generic;
using System.Threading;
using Common.Minigames.Models;
using Common.Models;
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
        private MinigameResult _playerResult;
        private MinigameCompletionPayload _payload;

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
            _payload = payload;
            _playerResult = payload.Result;
            
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
            var participantsMock = new List<MinigameParticipantModel>
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

            return new MinigameResultsPayload()
            {
                Participants = participantsMock,
                LocalPlayer = new MinigameParticipantModel()
                {
                    UserId = "0", //todo: get actual user id
                    Name = "You",
                    AvatarId = "avatar_2",
                    Result = _playerResult
                },
                MinigameIcon = _payload.MinigameIcon
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
    }
}