using System;
using UnityEngine;

namespace Core.Hub.Views
{
    public interface IMinigameModesView
    {
        Transform ModesContainer { get; }
        
        event Action OnBackButtonClicked;

        void SetIcon(Sprite icon);
    }
}