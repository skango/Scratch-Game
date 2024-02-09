using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Advanced scratching options types
public enum ScratchBy
{
    oneByOneTicket,
    AllAtOnceTicket
}

// Class for advanced Scratching options
public class TicketSectionGroup : MonoBehaviour
{
    [Header("User Settings")]
    public ScratchBy ScratchBy;

    [Header("Filled By Ticket")]
    public WinSettings winSettings;
    public int WinBoardImagesCount = 0;
    public Sprite WinningSprite;
    public bool ShowAnySprite = false;

    public int FinishedWinningBoardImages = 0;
    public int FinishedTicketAreas = 0;
    public int AreasCount = 0;

    public bool TicketFinished = false;

    public void ResetSettings(Ticket ticket)
    {
        AreasCount = ticket.TicketAreas.Count;
        TicketFinished = false;
        FinishedWinningBoardImages = 0;
        FinishedTicketAreas = 0;
    }

    // Method for filling win board images count
    public void FillWinBoardImagesCount()
    {
        if(ScratchBy == ScratchBy.oneByOneTicket) WinBoardImagesCount = Utility.RandomFromList(SettingsDatabase.instance.WinCountOneByOneTicket);
        if (ScratchBy == ScratchBy.AllAtOnceTicket) WinBoardImagesCount = Utility.RandomFromList(SettingsDatabase.instance.WinCountAllAtOnceTicket);
    }

    // Method for filling win Settings
    public void FillWinSettings()
    {
        winSettings = new WinSettings(Utility.RandomFromList(SettingsDatabase.instance.MoneyPriceList), AreaType.Money);
    }

    // Method for filling Board Images
    public void FillBoardImages(Ticket ticket)
    {
        List<BoardImage> AllBoardImages = ticket.TicketAreas.SelectMany(a => a.BoardImageList).ToList();
        List<BoardImageSettings> BoardImageSettingsList = TicketSection.MakeBoardImagesArray(WinBoardImagesCount, AllBoardImages.Count, winSettings.DidIWon);

        TicketSection.ConnectBoardSettings(ShowAnySprite, BoardImageSettingsList, AllBoardImages, WinBoardImagesCount, ref WinningSprite);
    }

    // Method for Connecting UI Addon
    public void ConnectToUIAddon()
    {
        ScoreBoardManager.instance.ConnectMoneyPartUI(ShowAnySprite, WinBoardImagesCount, winSettings, WinningSprite);
    }

    // Coroutine for Processing oneByOneTicket ScratchBy option
    public IEnumerator ProcessOneByOneTicket(Ticket ticket)
    {
        yield return BeginSectionGroupProcessing(ticket);

        foreach (var item in ticket.TicketAreas)
        {
            yield return item.WaitUntilScratchingFinish(ShowAnySprite);
            if (TicketFinished == false) yield return item.myScratchArea.FinishScratching(item.BoardImageList);
        }

        yield return ticket.FinishProcessing(ShowAnySprite, ticket.TicketAreas,winSettings);
    }

    // Coroutine for Processing AllAtOnceTicket ScratchBy option
    public IEnumerator ProcessAllAtOnceTicket(Ticket ticket)
    {
        yield return BeginSectionGroupProcessing(ticket);

        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, ticket.TicketAreas.Select(a => a.WaitUntilScratchingFinish(ShowAnySprite)).ToList(), 0.1f);

        yield return ticket.FinishProcessing(ShowAnySprite,ticket.TicketAreas, winSettings);
    }


    // Shared Coroutine for begining of the processing
    private IEnumerator BeginSectionGroupProcessing(Ticket ticket)
    {
        yield return ScoreBoardManager.instance.AnimAddonIn.Play();
        if (winSettings.type == AreaType.Money) yield return ScoreBoardManager.instance.AnimNumberIn.Play();

        StartCoroutine(WaitUntilTicketFinish(ticket));
    }

    // Method which updates revealed winning board images and decide if they match winBoardImages cound and if yes whole Ticket will be finished
    public void UpdateWinningCount()
    {
        FinishedWinningBoardImages++;
        if (FinishedWinningBoardImages == WinBoardImagesCount) TicketFinished = true;
    }

    // Method which updates finished ticket areass and decide if whole Ticket is finished
    public void FinishTicketArea()
    {
        FinishedTicketAreas++;
        if (FinishedTicketAreas == AreasCount) TicketFinished = true;
    }

    // Coroutine which wait untill we scratch whole ticket
    public IEnumerator WaitUntilTicketFinish(Ticket ticket)
    {
        while(TicketFinished == false)
        {
            yield return null;
        }

        foreach (var item in ticket.TicketAreas)
        {
            item.sectionFinished = true;
        }
    }
}
