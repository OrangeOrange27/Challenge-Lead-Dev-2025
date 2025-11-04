using TMPro;
using UnityEngine;

namespace Minigames.Match
{
    public class MatchMinigameView : MonoBehaviour, IMatchMinigameView
    {
        [SerializeField] private RectTransform _playingField;
        [SerializeField] private RectTransform _target;
        [SerializeField] private TMP_Text _text;

        public RectTransform PlayingField => _playingField;
        public RectTransform Target => _target;
        public TMP_Text Text => _text;
    }
}