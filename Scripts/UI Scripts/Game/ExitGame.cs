using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : UIWindows
{
    [SerializeField] private ConfirmationWindow ExitWindow;
    [SerializeField] private Board chessboard;
    public void onExitButton()
    {
        OpenConfirmationWindow("Oops... \nExit the game?");
    }

    private void OpenConfirmationWindow(string message)
    {
        ExitWindow.ShowWindow();
        ExitWindow.yesButton.onClick.AddListener(yesClicked);
        ExitWindow.noButton.onClick.AddListener(noClicked);
        ExitWindow.messageText.text = message;
    }

    private void yesClicked()
    {
        ExitWindow.HiddenWindow();
        PromotionWindow.instance.closeClicked();
        GameUIController.Instance.OnLeaveFromGameMenu();
    }

    private void noClicked()
    {
        ExitWindow.HiddenWindow();
        ExitWindow.yesButton.onClick.RemoveAllListeners();
        ExitWindow.noButton.onClick.RemoveAllListeners();
    }

}

