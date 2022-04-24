using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TeamMemberItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerName;
    [SerializeField] private TextMeshProUGUI PlayerStatusText;
    [SerializeField] private Image PlayerStatusIcon;
    [SerializeField] private kickPlayerButton kickPlayerButton;

 
    public void SetPlayerName(string playerName)
    {
        PlayerName.text = playerName;
    }

    public void SetPlayerStatusText(string playerStatus)
    {
        PlayerStatusText.text = playerStatus;
    }

    public void SetPlayerStatusIcon(Color playerStatusColor)
    {
        PlayerStatusIcon.color = playerStatusColor;
    }
   
    public kickPlayerButton getKickPlayerButton()
    {
        return kickPlayerButton;
    }
    
}


