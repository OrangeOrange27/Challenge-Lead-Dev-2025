using Common.Minigames.Models;
using Core.Hub.States;
using UnityEngine;

namespace Common
{
    public class GameContext
    {
        private MinigameBootstrapPayload _selectedMinigameConfiguration = new();

        public MinigameBootstrapPayload SelectedMinigameConfiguration => _selectedMinigameConfiguration;

        public void SelectMinigame(MinigameModel minigameModel, Sprite Icon)
        {
            _selectedMinigameConfiguration.MinigameModel = minigameModel;
            _selectedMinigameConfiguration.MinigameIcon = Icon;
        }
        
        public void SelectMode(MinigameModeModel modeModel)
        {
            _selectedMinigameConfiguration.GameMode = modeModel;
        }
    }
}