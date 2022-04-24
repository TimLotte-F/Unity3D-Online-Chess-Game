using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class kickPlayerButton : MonoBehaviourPun
{
    [SerializeField] private Button button;

    public void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }
    public void RemoveButtonAllEvents()
    {
        button.onClick.RemoveAllListeners();
    }

    public void AddKickPlayerEvent()
    {
        button.onClick.AddListener(delegate () { OnClickKickPlayerButton(); });
    }
    public void ShowButton()
    {
        RemoveButtonAllEvents();
        button.gameObject.SetActive(true);
        AddKickPlayerEvent();
    }

    public void HideButton()
    {
        button.gameObject.SetActive(false);
    }

    public void OnClickKickPlayerButton()
    {
        // print("PlayerListOther length: " + PhotonNetwork.PlayerListOthers.Length);
        foreach (var p in PhotonNetwork.PlayerListOthers)
        {
            if (!p.IsMasterClient)
            {
                PhotonNetwork.CloseConnection(p);
            }
        }
    }
}
