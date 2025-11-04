using System;
using Common.Minigames;
using Common.Minigames.Models;
using Core.EntryPoint;
using Infra.AssetManagement.ViewLoader;
using Minigames.Match;
using VContainer;

namespace Minigames
{
    public static class MinigamesInstaller
    {
        public static void RegisterMinigames(this IContainerBuilder builder)
        {
            builder.RegisterController<RootMinigameController>();
            
            builder.Register<MatchMinigameFlow>(Lifetime.Transient);

            builder.RegisterFactory<MinigameModel, IMinigameFlow>(c => model =>
            {
                return model.Id switch
                {
                    "match_minigame" => c.Resolve<MatchMinigameFlow>(),
                    _ => throw new ArgumentException($"Unknown minigame {model.Id}")
                };
            }, Lifetime.Transient);
            
            
            builder.RegisterViewLoader<IMatchMinigameView, MatchMinigameView>("match_minigame_view");
        }
    }
}