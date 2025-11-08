using System;
using System.Threading;
using Common.Minigames.Models;
using Common.Server;
using Core;
using Core.Hub.States;
using Cysharp.Threading.Tasks;
using Infra;
using Infra.ControllersTree;
using Infra.ControllersTree.Abstractions;
using Infra.StateMachine;
using VContainer;

namespace Common.Minigames
{
    public class RootMinigameController : IStateController<MinigameBootstrapPayload>
    {
        private readonly IObjectResolver _resolver;
        private readonly Func<MinigameModel, IMinigameFlow> _flowFactory;
        private readonly IPlayerDataService _playerDataService;

        private MinigameBootstrapPayload _payload;
        private string _matchId;

        public RootMinigameController(IObjectResolver resolver, Func<MinigameModel, IMinigameFlow> flowFactory, IPlayerDataService playerDataService)
        {
            _resolver = resolver;
            _flowFactory = flowFactory;
            _playerDataService = playerDataService;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(MinigameBootstrapPayload payload, IControllerResources resources,
            IControllerChildren controllerChildren,
            CancellationToken token)
        {
            _payload = payload;
            
            await JoinMatchAsync();
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            var result = await controllerChildren.Create<MinigameModel, MinigameResult>(ConvertFactory(_flowFactory))
                .RunToDispose(_payload.MinigameModel, token);
            
            SubmitScoreAsync(result.TotalPoints).Forget();

            var minigameCompletionPayload = new MinigameCompletionPayload()
            {
                MinigameIcon = _payload.MinigameIcon,
                Result = result,
                MatchId = _matchId,
                GameMode = _payload.GameMode
            };

            return StateMachineInstructionSugar.GoTo<MinigameCompletionState, MinigameCompletionPayload>(_resolver,
                minigameCompletionPayload);
        }

        public UniTask OnStop(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        private Func<IMinigameFlow> ConvertFactory(Func<MinigameModel, IMinigameFlow> flowFactory)
        {
            var model = _payload;
            return () => flowFactory(model.MinigameModel);
        }

        private async UniTask JoinMatchAsync()
        {
            var response = await ServerAPI.Matches.EnterMatchAsync(_payload.MinigameModel.Id, _payload.GameMode.Id, _playerDataService.PlayerData.AuthToken);

            _matchId = response.matchId;
            
            _playerDataService.SpendBalance(ServerDataAdapter.FromServer(response.mode.entryFee.currencyType),
                response.mode.entryFee.amount);
        }

        private async UniTask SubmitScoreAsync(int score)
        {
            var response =
                await ServerAPI.Matches.SubmitScoreAsync(_matchId, score, _playerDataService.PlayerData.AuthToken);
        }
    }
}