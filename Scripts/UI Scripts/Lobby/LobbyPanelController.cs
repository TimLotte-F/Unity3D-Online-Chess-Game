using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class LobbyPanelController : MonoBehaviourPunCallbacks
{

    public GameObject userMessage;
    public Button backButton;
    public GameObject lobbyLoadingLabel;
    public GameObject roomLoadingLabel;
    public GameObject roomMessagePanel;

    public Button randomJoinButton;

    public GameObject createRoomPanel;
    public RoomItem roomItemPrefab;
    public Transform roomContent;

    private List<RoomInfo> roomInfo;
    private RoomItem[] roomMessage;
    private List<RoomItem> roomItemsList = new List<RoomItem>();

    public override void OnEnable()
    {
        base.OnEnable(); // get the call back

        lobbyLoadingLabel.SetActive(true);
        roomLoadingLabel.SetActive(false);
        if (createRoomPanel != null)
            createRoomPanel.SetActive(false);

        initPanelInfo();
        RoomToLobby();

    }

    #region RoomList Update logic
    public void initPanelInfo()
    {
        ButtonControl();
        roomMessage = roomMessagePanel.GetComponentsInChildren<RoomItem>();
        foreach (var room in roomMessage)
            Destroy(room.gameObject);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomListing.Instance.OnUpdatedRoomList(roomList);
        roomInfo = RoomListing.Instance.GetRoomlisting();
        updatePanelInfo();
    }

    private void ShowRoomMessage()
    {
        foreach (RoomItem item in roomItemsList)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        for (int i = 0; i < roomInfo.Count; i++)
        {
            string roomName = roomInfo[i].Name;
            RoomItem newRoom = SetRoomItemInfo(roomInfo[i].Name, i);
            roomItemsList.Add(newRoom);

        }
    }
    private RoomItem SetRoomItemInfo(string roomName, int i)
    {
        RoomItem newRoom = Instantiate(roomItemPrefab, roomContent);

        newRoom.SetRoomOrderNumber((i + 1).ToString());
        newRoom.SetRoomName(roomName);
        newRoom.SetPlayerCountInRoom(roomInfo[i].PlayerCount, roomInfo[i].MaxPlayers);
        newRoom.SetTimeSet(roomInfo[i].CustomProperties[RoomProperty.RoundTime].ToString());

        string player1Name = RoomProperty.getPlayerNameInRoom(roomInfo[i], RoomProperty.Owner);
        newRoom.SetPlayer1(player1Name);
        string player2Name = RoomProperty.getPlayerNameInRoom(roomInfo[i], RoomProperty.Player2);
        newRoom.SetPlayer2(player2Name);

        newRoom.RemoveButtonAllEvents();
        if (roomInfo[i].PlayerCount == roomInfo[i].MaxPlayers || !roomInfo[i].IsOpen)
            newRoom.HideEnterRoomButton();
        else
        {
            newRoom.AddButtonEvent();
            newRoom.ShowEnterRoomButton();
        }
        newRoom.gameObject.SetActive(true);

        return newRoom;
    }

    void updatePanelInfo()
    {
        ButtonControl();
        ShowRoomMessage();
    }
    #endregion
    #region buttons Controller
    private void ButtonControl()
    {
        randomJoinButton.interactable = true;

        if (roomInfo == null || roomInfo.Count == 0)
        {
            randomJoinButton.interactable = false;
        }

    }

    // on click events
    public void RoomToLobby()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate ()
        {
            PhotonNetwork.Disconnect();
            NonGameUIManager.Instance.loginPanel.SetActive(true);
            NonGameUIManager.Instance.lobbyPanel.SetActive(false);
            userMessage.SetActive(false);
            backButton.gameObject.SetActive(false);
        });
    }
    private void LobbyToLogin()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(delegate ()
        {
            PhotonNetwork.Disconnect();
            NonGameUIManager.Instance.loginPanel.SetActive(true);
            NonGameUIManager.Instance.lobbyPanel.SetActive(false);
            backButton.gameObject.SetActive(false);
        });
    }
    public void ClickJoinRoomButton(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void ClickCreateRoomButton()
    {
        createRoomPanel.SetActive(true);
    }
    public void ClickRandomJoinButton()
    {
        if (roomInfo.Count > 0)
            PhotonNetwork.JoinRandomRoom();
    }
    public void ClickMatchingButton()
    {
        int randomNumber = Random.Range(1, 9999);
        string randomRoomName = string.Format("{0} #({1})", PhotonNetwork.LocalPlayer.NickName, randomNumber.ToString());
        
        // if repeat the room name, will call again matching button.
        if (!RoomListing.Instance.isRoomNameRepeat(randomRoomName))
        {
            PhotonNetwork.JoinRandomOrCreateRoom(
                roomName: randomRoomName,
                roomOptions: RoomProperty.getRoomOptions(PhotonNetwork.LocalPlayer.NickName, (int)RoundTime.Two),
                typedLobby: TypedLobby.Default);
        }
        else
        {
            ClickMatchingButton();
        }
    }
    #endregion
    #region Photon Call backs
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print(returnCode + " Failed message: " + message);
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        // show message window
        print(returnCode + " Failed message: " + message);
        base.OnJoinRandomFailed(returnCode, message);
        PhotonNetwork.JoinLobby();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print(returnCode + " Failed message: " + message);
        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby()
    {
        print("ON join lobby");
        lobbyLoadingLabel.SetActive(false);
        PhotonNetwork.EnableCloseConnection = false;
        NonGameUIManager.Instance.lobbyPanel.SetActive(true);
        RoomToLobby();
    }

    public override void OnJoinedRoom()
    {
        LobbyToLogin();
        RoomListing.Instance.GetRoomlisting().Clear();
        // print("Room list length is: " + RoomListing.Instance.GetRoomlisting().Count);
        PhotonNetwork.EnableCloseConnection = true;
        NonGameUIManager.Instance.lobbyPanel.SetActive(false);
        NonGameUIManager.Instance.roomPanel.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.Owner] = PhotonNetwork.LocalPlayer.NickName;
    }
    public override void OnLeftRoom()
    {
        print("OnLeftRoom()");
        PhotonNetwork.JoinLobby();
        NonGameUIManager.Instance.roomPanel.SetActive(false);
        backButton.gameObject.SetActive(true);
    }

    #endregion


}
