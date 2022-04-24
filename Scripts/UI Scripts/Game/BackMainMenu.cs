using UnityEngine;

public class BackMainMenu : UIWindows
{
    // this windows for Gaming to go back lobby
    // IN GAME -> LOBBY

    [SerializeField] private ConfirmationWindow BackMainMenuWindow;
    public void onMenuButton()
    {
        OpenWindow("Back to Main Menu?");
    }

    private void OpenWindow(string message)
    {
        AddButtonsEvent(); // Add the Button events when windows opened.
        BackMainMenuWindow.ShowWindow();
        BackMainMenuWindow.messageText.text = message;
    }

    private void yesClicked()
    {
        // NetworkManager; // IN GAME -> LOBBY
        if (BackMainMenuWindow.gameObject != null || BackMainMenuWindow.gameObject.activeSelf)
            BackMainMenuWindow.HiddenWindow();
        if (PromotionWindow.instance.getPromotionWindow().gameObject.activeSelf)
            PromotionWindow.instance.closeClicked();

        GameUIController.Instance.OnLeaveFromGameMenu();
        NetworkTurnManager.Instance.OnClickExitButton();
        RemoveButtonsEvent();
    }

    private void noClicked()
    {
        BackMainMenuWindow.HiddenWindow();
        RemoveButtonsEvent();
    }

    private void AddButtonsEvent()
    {
        BackMainMenuWindow.yesButton.onClick.AddListener(yesClicked);
        BackMainMenuWindow.noButton.onClick.AddListener(noClicked);
    }
    private void RemoveButtonsEvent()
    {
        BackMainMenuWindow.yesButton.onClick.RemoveAllListeners();
        BackMainMenuWindow.noButton.onClick.RemoveAllListeners();
    }

}

