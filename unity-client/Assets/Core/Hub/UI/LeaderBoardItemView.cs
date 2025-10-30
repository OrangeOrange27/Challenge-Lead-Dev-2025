using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Hub.UI
{
    public class LeaderBoardItemView : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private TMP_Text _playerScore;
        [SerializeField] private TMP_Text _playerPlace;
        
        [SerializeField] private RewardItemView _reward;
    }
}