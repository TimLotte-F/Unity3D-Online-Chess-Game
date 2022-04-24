
using Photon.Pun;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RoundTime
{
    One = 1,
    Two = 2,
    Three = 3

}
public class RoundTimeDropdown : MonoBehaviourPun
{

    [SerializeField] public TMP_Dropdown roundTimeDropdown { get; set; }
    private RoundTime _selectedRoundTime;
    private Toggle[] _options;


    public RoundTime SelectedRoundTime
    {
        get
        { return _selectedRoundTime; }
        set
        { _selectedRoundTime = value; }
    }

    private void Awake()
    {
        if (roundTimeDropdown == null)
            roundTimeDropdown = GetComponent<TMP_Dropdown>();
        roundTimeDropdown.ClearOptions();

        roundTimeDropdown.AddOptions(Enum.GetNames(typeof(RoundTime)).Select(x => x + " (Mintus)").ToList());
        roundTimeDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(roundTimeDropdown); });
        roundTimeDropdown.RefreshShownValue();

        UpdateRoundTimeMessage();
    }
    void DropdownValueChanged(TMP_Dropdown change)
    {
        int ValueIndex = change.value;
        int CurrentRoundTimeValue = (int)Enum.GetValues(typeof(RoundTime)).GetValue(ValueIndex);
        SelectedRoundTime = (RoundTime)CurrentRoundTimeValue;
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ExitGames.Client.Photon.Hashtable roundTimeProperty = new ExitGames.Client.Photon.Hashtable();
                roundTimeProperty.Add(RoomProperty.RoundTime, CurrentRoundTimeValue);
                PhotonNetwork.CurrentRoom.SetCustomProperties(roundTimeProperty);
            }
            else
            {
                UpdateRoundTimeMessage();
            }
        }
    }

    public void SetAllOptionsStatus(bool b)
    {
        if (roundTimeDropdown != null)
        {
            var options = roundTimeDropdown.GetComponentsInChildren<Toggle>();
            if (options.Length == 0)
            {
                return;
            }
            _options = options;
            Debug.Log("Optione length: " + options.Length);

            foreach (var option in _options)
            {
                option.interactable = b;
            }
        }
    }

    public void UpdateRoundTimeMessage()
    {
        RoundTime t = (RoundTime)getRoundTimeInRoom();
        roundTimeDropdown.captionText.text = t.ToString() + " (Minutes)";
        roundTimeDropdown.SetValueWithoutNotify((int)t - 1);
    }
    public int getRoundTimeInRoom() => (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.RoundTime];

}
