
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class LoginPanelController : MonoBehaviourPunCallbacks
{
    public static LoginPanelController Instance;
    public GameObject userMessage;
    public Button BackButton;
    public TMP_InputField username;
    public TMP_InputField password;
    public TextMeshProUGUI connectionState;
    private string storedUsername;


    //#if(UNITY_EDITOR)
    private void Update()
    {
        connectionState.text = PhotonNetwork.NetworkClientState.ToString();
    }
    //#endif

    private void Start()
    {
        if (Instance == null)
            Instance = this;

        if (!PhotonNetwork.IsConnected)
        {
            SetLoginPanelActive();

            string email = PlayerPrefs.GetString("Email");
            if (email != "" || email == null)
            {
                username.text = email;
                username.textComponent.SetText(email);
            }

        }
        else
        {
            SetLobbyPanelActive();
            connectionState.text = "";
        }

    }

    public void SetLobbyPanelActive()
    {
        NonGameUIManager.Instance.loginPanel.SetActive(false);
        NonGameUIManager.Instance.lobbyPanel.SetActive(true);
        userMessage.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }

    public void SetLoginPanelActive()
    {
        NonGameUIManager.Instance.loginPanel.SetActive(true);
        userMessage.SetActive(false);
        BackButton.gameObject.SetActive(false);
        NonGameUIManager.Instance.lobbyPanel.SetActive(false);
        if (NonGameUIManager.Instance.roomPanel != null)
            NonGameUIManager.Instance.roomPanel.SetActive(false);
    }

    public void LoginSuccess()
    {
        SavePlayerNameData(PlayerInfo.Instance.getPlayerName());
        RoomListing.Instance.GetRoomlisting().Clear();
        Connect();
    }
    public void SavePlayerNameData(string username)
    {
        PhotonNetwork.LocalPlayer.NickName = username;            // Set the player name
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.SetString("Email", this.username.textComponent.text.Length > 1 ? this.username.textComponent.text : ""); // Save the player name into Local data
    }
    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void onClickExitGameButton()
    {
        Application.Quit();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        SetLoginPanelActive();
    }
    public override void OnConnectedToMaster()
    {
        SetLobbyPanelActive();
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    public override void OnJoinedLobby()
    {

        userMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Welcome, " + PhotonNetwork.LocalPlayer.NickName;
    }

}
