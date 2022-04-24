using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PromotionWindow : SelectionWindows
{
    public static PromotionWindow instance { get; set; }
    [SerializeField] private SelectionWindows promotionWindow;
    [SerializeField] private Board chessboard;

    string TypeOfMessage;
    Button[] buttons = null;
    int typeNum = (int)Enum.Parse(typeof(ChessPieceType), "Queen");
    public void Awake()
    {
        instance = this;
        buttons = promotionWindow.options;
        promotionWindow.HiddenWindow();
    }
    public SelectionWindows getPromotionWindow()
    {
        return promotionWindow;
    }
    public void OpenWindow(string title)
    {

        promotionWindow.ShowWindow();
        promotionWindow.titleText.text = title; // Pawn's Promotion
        promotionWindow.setMessageText("Your selection:		" + (ChessPieceType)typeNum); // Your selection (default is Queen):		(TypeOfPieces)
        AddButtonsEvent();
    }
    private void initSelecttionButtons()
    {
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int buttonIndex = i;
                promotionWindow.options[buttonIndex].onClick.AddListener(() => onSelectButton(buttonIndex));
            }
        }
    }
    private void onSelectButton(int buttonIndex)
    {
        TypeOfMessage = buttons[buttonIndex].tag; // get the piece tag for the piece types
        typeNum = (int)Enum.Parse(typeof(ChessPieceType), TypeOfMessage);
        //print(typeNum.ToString());

        promotionWindow.setMessageText("Your selection:\t" + (ChessPieceType)typeNum);
    }


    private void confirmClicked()
    {
        chessboard.pawnPromotion((ChessPieceType)typeNum);
        NetworkTurnManager.Instance.OnPromotionPiece(typeNum.ToString());

        promotionWindow.HiddenWindow();
    }

    public void closeClicked()
    {
        promotionWindow.HiddenWindow();
        RemoveButtonsEvent();
    }

    private void AddButtonsEvent()
    {
        initSelecttionButtons();
        promotionWindow.confirmButton.onClick.AddListener(confirmClicked);
    }
    private void RemoveButtonsEvent()
    {
        foreach (var btn in promotionWindow.options)
            btn.onClick.RemoveAllListeners();
        promotionWindow.confirmButton.onClick.RemoveAllListeners();
    }

}
