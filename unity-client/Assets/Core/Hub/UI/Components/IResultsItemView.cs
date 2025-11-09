using System;
using Common.Models.Economy;
using UnityEngine;

namespace Core.Hub.UI.Components
{
    public interface IResultsItemView
    {
        Transform Transform { get; }
        
        event Action OnClaimButtonClicked;

        void SetData(string minigameName, Sprite icon, string timeAgo, RewardModel rewardModel, bool isClaimed,
            bool isHighlighted);
        
        void SetHighlighted(bool isHighlighted);
    }
}