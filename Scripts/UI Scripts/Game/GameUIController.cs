using System;
using UnityEngine;


public class GameUIController : MonoBehaviour
{
    [SerializeField] private Animator menuAnimator;
    public static GameUIController Instance { set; get; }

    public Action<bool> SetLocalGame;

    private void Awake()
    {
        Instance = this;
    }

    // buttons
    public void OnLocalGameButton()
    {
        SetLocalGame?.Invoke(true);
        CameraController.Instance.ChangeCamera(CameraAngel.whiteTeam);
        menuAnimator.SetTrigger("InGameMenu");
    }

    // networking UI
    public void OnStartGame(CameraAngel team)
    {
        SetLocalGame?.Invoke(true);
        CameraController.Instance.ChangeCamera(team + 1);
        menuAnimator.SetTrigger("InGameMenu");
    }

    public void OnLeaveFromGameMenu()
    {
        CameraController.Instance.ChangeCamera(CameraAngel.menu);
        menuAnimator.SetTrigger("StartMenu");


    }
}
