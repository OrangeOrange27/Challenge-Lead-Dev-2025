using Common.Minigames.Models;
using Infra.ControllersTree.Abstractions;

namespace Common.Minigames
{
    public interface IMinigameFlow : IControllerWithPayloadAndReturn<MinigameModel,MinigameResult>
    {
        
    }
}