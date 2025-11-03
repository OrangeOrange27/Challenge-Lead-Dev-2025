using System;
using UnityEngine;

namespace Core.Hub.UI
{
    public interface IMinigameItemView
    {
        event Action OnClick;
        
        void SetImage(Sprite sprite);
    }
}