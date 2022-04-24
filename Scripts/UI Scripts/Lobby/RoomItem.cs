using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviourPun
{
    public TextMeshProUGUI roomOrderNumber;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomOwner;
    public TextMeshProUGUI player2;
    public TextMeshProUGUI playerCountInRoom;
    public TextMeshProUGUI RoundTime;
    public Button enterRoomButton;



    LobbyPanelController manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyPanelController>();
    }
    public void SetRoomOrderNumber(string _roomOrderNumber)
    {
        roomOrderNumber.text = _roomOrderNumber;
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void SetPlayer1(string _player)
    {
        roomOwner.text = _player;
    }
    public void SetPlayer2(string _player)
    {
        player2.text = _player;
    }
    public void SetPlayerCountInRoom(int _playerCountInRoom, int MaxPlayer)
    {
        playerCountInRoom.text = _playerCountInRoom.ToString() + "/" + MaxPlayer.ToString();
    }
    public void SetTimeSet(string _RoundTime)
    {
        RoundTime.text = _RoundTime;
    }

    // buttons event
    public void RemoveButtonAllEvents()
    {
        enterRoomButton.onClick.RemoveAllListeners();

    }
    public void AddButtonEvent()
    {
        enterRoomButton.onClick.AddListener(delegate ()
        {
            OnClickItem();
        });

    }

    // button on click logic
    public void OnClickItem()
    {
        manager.ClickJoinRoomButton(roomName.text);
    }


    /// <summary>
    ///  Buttons setactive handle
    /// </summary>
    public void ShowEnterRoomButton()
    {
        enterRoomButton.gameObject.SetActive(true);
    }
    public void HideEnterRoomButton()
    {
        enterRoomButton.gameObject.SetActive(false);
    }
}
