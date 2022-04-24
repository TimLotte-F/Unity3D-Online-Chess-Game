using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWindows : UIWindows
{
    [SerializeField] public Button[] options;
    public Button confirmButton;
    public Button closeButton;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;

    public void setMessageText(string text)
    {
        messageText.text = text;
    }

    public void setTitleText(string text)
    {
        titleText.text = text;
    }
}
