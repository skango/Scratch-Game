using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Settings database class for changing winning sprites count, winning prices, sprites and colors

public class SettingsDatabase : MonoBehaviour
{
    public static SettingsDatabase instance;

    [Header("Win BoardimagesCount Default")]
    public List<int> WinBoardImagesCountDefault;
    [Header("Win OneByOneTicketCount")]
    public List<int> WinCountOneByOneTicket;
    [Header("Win AllAtOnceTicketCount")]
    public List<int> WinCountAllAtOnceTicket;
    [Header("Money Prices")]
    public List<SectionPrice> MoneyPriceList;
    [Header("Bonus Prices")]
    public List<SectionPrice> BonusPriceList;
    [Header("Bonus Sprite")]
    public Sprite BonusSprite;
    [Header("Money Sprite")]
    public Sprite MoneySprite;
    [Header("Any Sprite")]
    public Sprite AnySprite;
    [Header("All Board Sprites")]
    public List<Sprite> AllBoardSprites;
    [Header("Section Enabled Color")]
    public List<Color> SectionEnabledColorList;
    [Header("Section Disabled Color")]
    public Color SectionDisabledColor;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
}
