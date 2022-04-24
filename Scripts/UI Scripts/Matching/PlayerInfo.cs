
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance;
    private string PlayerName;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public string getPlayerName()
    {
        return PlayerName;
    }

    public void setPlayerName(string playerName)
    {
        this.PlayerName = playerName;
    }


}
