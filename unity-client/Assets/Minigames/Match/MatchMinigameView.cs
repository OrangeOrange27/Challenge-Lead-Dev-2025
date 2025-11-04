using UnityEngine;

namespace Minigames.Match
{
    public class MatchMinigameView : MonoBehaviour, IMatchMinigameView
    {
        [SerializeField] private RectTransform _playingField;
        [SerializeField] private RectTransform _target;

        public RectTransform PlayingField => _playingField;
        public RectTransform Target => _target;
    }
}