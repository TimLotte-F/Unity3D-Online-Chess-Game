using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class PlayerItem : MonoBehaviourPunCallbacks
{

    public TextMeshProUGUI playerName;

    public Color highlightColor;

    public TextMeshProUGUI statusText;
    public Button readyButton;
    public bool isReady;

    Player player;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        statusText.text = "Not Ready";
        isReady = false;
    }
    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
    }

    public void ApplyLocalChanges()
    {

        if (isReady == false)
        {
            statusText.text = "Ready";
        }
        else
        {
            statusText.text = "Not Ready";

        }
    }

   public void OnClickReadyButton()
    {
        if (playerProperties.ContainsKey("R"))
        {
            statusText.text = "Ready";
        }
        else
        {
            statusText.text = "Not Ready";
        }

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }


}
