using System.Threading;
using Common.Minigames;
using Common.Minigames.Models;
using Cysharp.Threading.Tasks;
using Infra.ControllersTree.Abstractions;

namespace Minigames.Match
{
    public class MatchMinigameFlow : IMinigameFlow
    {
        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnStop(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnDispose(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask OnStart(MinigameModel payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<MinigameResult> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}