using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UIPart
{
    public TextMeshProUGUI UICountText;
    public Transform UISpriteTransform;
    public AnimClipPlayer AnimationIn;
}

// class for Managing Scoreboard Panel
public class ScoreBoardManager : MonoBehaviour
{
    public static ScoreBoardManager instance;

    public UIPart MainPart;
    public UIPart BonusPart;

    [Space]
    public TextMeshProUGUI LeftText;
    public TextMeshProUGUI RightText;
    public TextMeshProUGUI NumberText;

    public GameObject NumberPanel;
    public Image CenterImage;

    [Header("Animations")]
    public AnimClipPlayer AnimMainIn;
    public AnimClipPlayer AnimAddonIn;
    public AnimClipPlayer AnimAddonOut;
    public AnimClipPlayer AnimNumberIn;
    public AnimClipPlayer AnimNumberOut;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void UpdateMoneyAndScore(int money, int bonus)
    {
        MainPart.UICountText.text = money.ToString();
        BonusPart.UICountText.text = bonus.ToString();
    }

    public void ConnectMoneyPartUI(bool showAnySprite, int winningSpriteCount, WinSettings winSettings, Sprite winSprite)
    {
        NumberPanel.SetActive(true);

        string LeftText = "MATCH ";
        if (showAnySprite == true && winningSpriteCount != 1) LeftText = "ANY ";
        string RightText = "WIN " + winSettings.PricePreset.Price.ToString();
        string Number = winningSpriteCount.ToString();

        FillAddonUI(LeftText, winSprite, RightText, Number);
    }

    public void ConnectBonusPartUI(TextMeshProUGUI BonusBoardText, WinSettings winSettings)
    {
        NumberPanel.SetActive(false);

        string LeftText = "TRY";
        string RightText = "BONUS";

        Sprite winSprite = SettingsDatabase.instance.BonusSprite;

        FillAddonUI(LeftText, winSprite, RightText, "");

        if (winSettings.DidIWon == true) BonusBoardText.text = winSettings.PricePreset.Price.ToString();
        else BonusBoardText.text = "No Luck!";
    }

    private void FillAddonUI(string leftText, Sprite winSprite, string rightText, string number)
    {
        LeftText.text = leftText;
        RightText.text = rightText;
        NumberText.text = number;
        CenterImage.sprite = winSprite;
    }
}
