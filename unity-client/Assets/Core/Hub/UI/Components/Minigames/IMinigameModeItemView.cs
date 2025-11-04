using System;
using Common.Minigames.Models;

namespace Core.Hub.UI
{
    public interface IMinigameModeItemView
    {
        event Action OnPlayButtonClicked;
        void SetData(MinigameModeModel model);
    }
}