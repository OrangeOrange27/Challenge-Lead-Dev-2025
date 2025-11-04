using TMPro;
using UnityEngine;

namespace Minigames.Match
{
    public interface IMatchMinigameView
    {
        RectTransform PlayingField { get; }
        RectTransform Target { get; }
        TMP_Text Text { get; }
    }
}