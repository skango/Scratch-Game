using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ticket : MonoBehaviour
{
    public List<TicketSection> TicketAreas;
    private List<Color> ColorPalette;

    [Header("Optional Section Group")]
    public TicketSectionGroup sectionGroup;

    private void Awake()
    {
        TicketAreas = GetComponentsInChildren<TicketSection>().ToList();

        if (sectionGroup != null)
        {
            foreach (var item in TicketAreas)
            {
                item.sectionGroup = sectionGroup;
            }
        }
    }

    // Starting method for ticked preparation
    public void PrepareTicket()
    {
        ResetTicket();
        FillSectionsColor();

        if (sectionGroup == null)
        {
            foreach (var item in TicketAreas)
            {
                item.ShowAnySprite = Utility.GetRandomBool();
                item.FillWinBoardImagesCount();
                item.FillWinSettings(item.areaType);
                item.FillBoardImages(item.areaType);
                if (TicketAreas.First() == item) item.ConnectToUIAddon(item.areaType);
            }
        }
        else
        {
            sectionGroup.ShowAnySprite = Utility.GetRandomBool();
            sectionGroup.FillWinBoardImagesCount();
            sectionGroup.FillWinSettings();
            sectionGroup.FillBoardImages(this);
            sectionGroup.ConnectToUIAddon();
        }
    }

    // Main method for processing ticket
    public IEnumerator ProcessTicket()
    {
        List<IEnumerator> TicketAnimationsRoutines = new List<IEnumerator> { ScoreBoardManager.instance.AnimMainIn.Play(true) };
        TicketAnimationsRoutines.AddRange(TicketAreas.Select(a => a.AnimIn.Play()).ToList());
        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, TicketAnimationsRoutines, 0.1f);

        if (sectionGroup == null) yield return ProcessOneByOneSection();
        else
        {
            if (sectionGroup.ScratchBy == ScratchBy.oneByOneTicket) yield return sectionGroup.ProcessOneByOneTicket(this);
            if (sectionGroup.ScratchBy == ScratchBy.AllAtOnceTicket) yield return sectionGroup.ProcessAllAtOnceTicket(this);
        }

        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, TicketAreas.Select(a => a.AnimOut.Play()).Reverse().ToList(), 0.1f);
    }

    private void ResetTicket()
    {
        ColorPalette = SettingsDatabase.instance.SectionEnabledColorList.ToList();

        if (sectionGroup != null) sectionGroup.ResetSettings(this);

        foreach (var item in TicketAreas)
        {
            item.ResetSection(item);
        }
    }

    private void FillSectionsColor()
    {
        foreach (var item in TicketAreas)
        {
            item.myScratchArea.ScratchImage.color = SettingsDatabase.instance.SectionDisabledColor;
            item.myScratchArea.BGColor = Utility.RandomFromListRemove(ColorPalette);
        }
    }

    // Basic Scratching option by process section one by one
    public IEnumerator ProcessOneByOneSection()
    {
        foreach (var item in TicketAreas)
        {
            if (TicketAreas.First() != item) item.ConnectToUIAddon(item.areaType);

            yield return ScoreBoardManager.instance.AnimAddonIn.Play();
            if (item.winSettings.type == AreaType.Money) yield return ScoreBoardManager.instance.AnimNumberIn.Play();

            yield return item.WaitUntilScratchingFinish(item.ShowAnySprite);
            yield return FinishProcessing(item.ShowAnySprite, new List<TicketSection> { item }, item.winSettings);
        }
    }

    // method after we process our ticket 
    public IEnumerator FinishProcessing(bool ShowAnySprite, List<TicketSection> SectionList, WinSettings winSettings)
    {
        if (ShowAnySprite == true)
        {
            if(winSettings.DidIWon == true)
            {
                yield return PlayBoardImagesWinAnimationIn(SectionList);
            }
        }

        List<TicketSection> SectionsWithEnablesScrathingArea = SectionList.Where(a => a.myScratchArea.gameObject.activeSelf == true).ToList();
        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, SectionsWithEnablesScrathingArea.Select(a => a.myScratchArea.FinishScratching(a.BoardImageList)).ToList());

        yield return WinPanelManager.instance.ShowWiningPanel(winSettings);

        if (ShowAnySprite == false || winSettings.DidIWon == true) yield return PlayBoardImagesWinAnimationOut(SectionList);

        if (winSettings.type == AreaType.Money) yield return ScoreBoardManager.instance.AnimNumberOut.Play();
        yield return ScoreBoardManager.instance.AnimAddonOut.Play();
    }

    //Coroutine to play win Out Animation
    private IEnumerator PlayBoardImagesWinAnimationOut(List<TicketSection> sectionList)
    {
        List<BoardImage> OnlyWinningBoardImages = sectionList.SelectMany(a => a.BoardImageList).ToList().Where(b => b.isWinning == true).ToList();
        List<IEnumerator> BoardImagesAnimOutList = OnlyWinningBoardImages.Select(a => a.WinAnimOut.Play()).ToList();

        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, BoardImagesAnimOutList);

        foreach (var item in OnlyWinningBoardImages)
        {
            item.WinningGameobject.SetActive(false);
        }
    }

    //Coroutine to play win In Animation
    private IEnumerator PlayBoardImagesWinAnimationIn(List<TicketSection> sectionList)
    {
        List<IEnumerator> BoardImagesAnimInList = sectionList.SelectMany(a => a.BoardImageList).ToList().Select(b => b.CheckIfFinishedAndShowWining()).ToList();
        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, BoardImagesAnimInList);
    }
}
