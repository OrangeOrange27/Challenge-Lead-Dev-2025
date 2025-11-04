using Common.Minigames.Models;
using Core.Hub.States;

namespace Common
{
    public class GameContext
    {
        private MinigameBootstrapPayload _selectedMinigameConfiguration = new();

        public MinigameBootstrapPayload SelectedMinigameConfiguration => _selectedMinigameConfiguration;

        public void SelectMinigame(MinigameModel minigameModel)
        {
            _selectedMinigameConfiguration.MinigameModel = minigameModel;
        }
        
        public void SelectMode(MinigameModeModel modeModel)
        {
            _selectedMinigameConfiguration.GameMode = modeModel;
        }
    }
}