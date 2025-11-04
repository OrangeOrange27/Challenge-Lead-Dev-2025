using System;
using Common.Minigames;
using Common.Minigames.Models;
using Minigames.Match;
using VContainer;

namespace Minigames
{
    public static class MinigamesInstaller
    {
        public static void RegisterMinigames(this IContainerBuilder builder)
        {
            builder.Register<MatchMinigameFlow>(Lifetime.Transient);

            builder.RegisterFactory<MinigameModel, IMinigameFlow>(c => model =>
            {
                return model.Id switch
                {
                    "Match" => c.Resolve<MatchMinigameFlow>(),
                    _ => throw new ArgumentException($"Unknown minigame {model.Id}")
                };
            }, Lifetime.Transient);
        }
    }
}