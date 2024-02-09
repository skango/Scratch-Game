using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TicketSection : MonoBehaviour
{
    public AreaType areaType;
    public ScratchingArea myScratchArea;
    public List<BoardImage> BoardImageList;
    public bool sectionFinished = false;
    private int FinishedBoardImagesCount = 0;
    public int FinishedWinningSprites = 0;
    public WinSettings winSettings;
    [Header("Money Part")]
    public int WinBoardImagesCount = 0;
    private Sprite WinningSprite;
    [Header("Bonus Part")]
    public TextMeshProUGUI BonusText;
    [Header("Optional Section Group")]
    public TicketSectionGroup sectionGroup;
    public bool ShowAnySprite = false;
    [Header("Animations")]
    public AnimClipPlayer AnimIn;
    public AnimClipPlayer AnimOut;

    private void Awake()
    {
        myScratchArea = GetComponentInChildren<ScratchingArea>(true);
        BoardImageList = GetComponentsInChildren<BoardImage>().ToList();
        foreach (var item in BoardImageList)item.myTicketSection = this;
    }

    public void ResetSection(TicketSection section)
    {
        FinishedBoardImagesCount = 0;
        FinishedWinningSprites = 0;

        sectionFinished = false;

        myScratchArea.ResetScratchArea();

        foreach (var boardImage in BoardImageList)
        {
            boardImage.ResetBoardImage();
        }
    }

    // Method for connecting Data to UI Elements
    public void ConnectToUIAddon(AreaType areatype)
    {
        if (areaType == AreaType.Money) ScoreBoardManager.instance.ConnectMoneyPartUI(ShowAnySprite, WinBoardImagesCount, winSettings, WinningSprite);
        if (areaType == AreaType.Bonus) ScoreBoardManager.instance.ConnectBonusPartUI(BonusText, winSettings);
    }


    // Method for picking random winning sprite count number from database
    public void FillWinBoardImagesCount()
    {
        WinBoardImagesCount = Utility.RandomFromList(SettingsDatabase.instance.WinBoardImagesCountDefault);
    }

    // Main Method for filling random board sprites for ticket
    public void FillBoardImages(AreaType areaType)
    {
        if (areaType == AreaType.Money)
        {
            List<BoardImageSettings> BoardImageSettingsList = MakeBoardImagesArray(WinBoardImagesCount, BoardImageList.Count, winSettings.DidIWon);
            ConnectBoardSettings(ShowAnySprite,BoardImageSettingsList, BoardImageList, WinBoardImagesCount, ref WinningSprite);
        }

        if(areaType == AreaType.Bonus)
        {
            BoardImageList[0].Winning(winSettings.DidIWon);
            BoardImageList[1].Winning(winSettings.DidIWon);
        }
    }

    // Method for connecting boar imnage settings to board images
    public static void ConnectBoardSettings(bool ShowAnySprite, List<BoardImageSettings> boardImageSettingsList, List<BoardImage> boardImageList, int winBoardImagesCount, ref Sprite winningSprite)
    {
        for (int i = 0; i < boardImageList.Count; i++)
        {
            boardImageList[i].initialize(boardImageSettingsList[i]);
        }

        if (ShowAnySprite == true && winBoardImagesCount != 1) winningSprite = SettingsDatabase.instance.AnySprite;
        else winningSprite = boardImageSettingsList.Last().mySprite;
    }

    // Method which makes BoardImageSettings List which is later connected to actual board images respecting the winning conditions
    public static List<BoardImageSettings> MakeBoardImagesArray(int winBoardImagesCount, int CurrentBoardImagesCount, bool didIWon)
    {
        List<Sprite> AllSpritesCopy = SettingsDatabase.instance.AllBoardSprites.ToList();
        Sprite LocalWiningSprite = Utility.RandomFromListRemove(AllSpritesCopy);
        List<BoardImageSettings> BoardImageSettingsList = new List<BoardImageSettings>();
        List<Sprite> NonWinningSprites = new List<Sprite>();

        if (didIWon == true)
        {
            for (int i = 0; i < winBoardImagesCount; i++)
            {
                BoardImageSettingsList.Add(new BoardImageSettings(LocalWiningSprite, true));
            }
        }
        else
        {
            for (int i = 0; i < winBoardImagesCount - 1; i++)
            {
                BoardImageSettingsList.Add(new BoardImageSettings(LocalWiningSprite, true));
            }
        }

        foreach (var item in AllSpritesCopy)
        {
            for (int i = 0; i < winBoardImagesCount - 1; i++)
            {
                NonWinningSprites.Add(item);
            }
        }

        if (NonWinningSprites.Count != 0)
        {
            for (int i = BoardImageSettingsList.Count; i < CurrentBoardImagesCount; i++)
            {
                BoardImageSettingsList.Add(new BoardImageSettings(Utility.RandomFromListRemove(NonWinningSprites), false));
            }
        }
        else
        {
            for (int i = BoardImageSettingsList.Count; i < CurrentBoardImagesCount; i++)
            {
                BoardImageSettingsList.Add(new BoardImageSettings(Utility.RandomFromListRemove(AllSpritesCopy), false));
            }
        }

        BoardImageSettingsList = Utility.ShuffleList(BoardImageSettingsList);
        BoardImageSettingsList.Add(new BoardImageSettings(LocalWiningSprite, true));
        return BoardImageSettingsList;
    }

    // Coroutine which enable scratching and wait until the scratching is finished
    public IEnumerator WaitUntilScratchingFinish(bool ShowAnySprite)
    {
        if (ShowAnySprite == false) ShowWinning();

        enableSamples(true);
        yield return myScratchArea.enableScratching();


        while(sectionFinished == false)
        {
            yield return null;
        }

        foreach (var item in BoardImageList)
        {
            item.isFinished = true;
        }

        if (sectionGroup != null) sectionGroup.FinishTicketArea();
    }

    // Method to show winning animation when the boarImage is reveal
    public void ShowWinning()
    {
        foreach (var item in BoardImageList)
        {
            StartCoroutine(item.CheckIfFinishedAndShowWining());
        }
    }

    // Method which updates finished board images and decide if whole section is finished
    public void UpdateAllCount(BoardImage myBoardImage)
    {
        if (myBoardImage.isWinning == true) UpdateWinningCount();
        FinishedBoardImagesCount++;
        if (FinishedBoardImagesCount == BoardImageList.Count) sectionFinished = true;
    }

    // Method which updates revealed winning board images and decide if they match winBoardImages cound and if yes section will be finished
    private void UpdateWinningCount()
    {
        FinishedWinningSprites++;
        if (sectionGroup != null) sectionGroup.UpdateWinningCount();
        if (FinishedWinningSprites == WinBoardImagesCount) sectionFinished = true;
    }

    private void enableSamples(bool value)
    {
        foreach (var item in BoardImageList)
        {
            item.EnableSamples(value);
        }
    }

    //Method for filling winning conditions to Money area type ticket and bonus area type ticket
    public void FillWinSettings(AreaType areaType)
    {
        if (areaType == AreaType.Money) winSettings = new WinSettings(Utility.RandomFromList(SettingsDatabase.instance.MoneyPriceList), AreaType.Money);
        if (areaType == AreaType.Bonus) winSettings = new WinSettings(Utility.RandomFromList(SettingsDatabase.instance.BonusPriceList), AreaType.Bonus);
    }
}
