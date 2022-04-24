using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonGameUIManager : MonoBehaviour
{

    public static NonGameUIManager Instance { get; set; }

    public GameObject loginPanel;
    public GameObject lobbyPanel;
    public GameObject createRoomPanel;
    public GameObject roomPanel;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

}
