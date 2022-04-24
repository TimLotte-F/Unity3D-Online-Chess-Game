using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class CreateRoomController : MonoBehaviourPun
{
    public GameObject createRoomPanel;
    public GameObject roomLoadingPanel;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomNameHint;
    public GameObject roundTimeToggle;


    private RoundTime[] roundTimeOptions = { RoundTime.One, RoundTime.Two, RoundTime.Three };

    public void Start()
    {
        roomNameHint.text = "";
    }
    public void ClickConfirmCreateRoomButton()
    {
        int toggleValue = getToggleCurrentValue();
        RoomOptions roomOptions = RoomProperty.getRoomOptions(PhotonNetwork.LocalPlayer.NickName, toggleValue);

        if (RoomListing.Instance.isRoomNameRepeat(roomName.text))
        {
            roomNameHint.text = "Duplicate room name";
        }
        else if (roomName.text.Length == 1)
        {
            roomNameHint.text = "Room name empty";
        }
        else
        {
            PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default);

            createRoomPanel.SetActive(false);
            roomLoadingPanel.SetActive(true);
            roomNameHint.text = "";
            print("Create room successfully.");
        }
    }


    public void ClickCancelCreateRoomButton()
    {
        createRoomPanel.SetActive(false);
        roomNameHint.text = "";
    }



    public int getToggleCurrentValue()
    {
        RectTransform toggleRectTransform = roundTimeToggle.GetComponent<RectTransform>();
        int childCount = toggleRectTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            if (toggleRectTransform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                // put the round time from the options into room properties
                return (byte)roundTimeOptions[i];
            }
        }
        return -1;
    }

}
