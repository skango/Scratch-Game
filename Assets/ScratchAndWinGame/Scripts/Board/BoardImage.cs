using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BoardImageSettings
{
    public Sprite mySprite;
    public bool isWinning = false;

    public BoardImageSettings(Sprite _mySprite, bool _isWinning)
    {
        mySprite = _mySprite;
        isWinning = _isWinning;
    }
}


public class BoardImage : MonoBehaviour
{
    public Transform SamplesParent;
    public bool isFinished = false;
    [HideInInspector]
    public TicketSection myTicketSection;
    private int VisitedSamples = 0;
    private float currentProgress;
    private List<Sample> AllSamples = new List<Sample>();

    public Image boardImage;
    public bool isWinning = false;
    public GameObject WinningGameobject;
    [Header("Animations")]
    public AnimClipPlayer WinAnimIn;
    public AnimClipPlayer WinAnimOut;
    public AnimClipPlayer ScratchFinalizeAnim;

    private void Awake()
    {
        WinningGameobject.SetActive(false);
        AllSamples = SamplesParent.GetComponentsInChildren<Sample>().ToList();
        foreach (var item in AllSamples)item.SetBoardImage(this);
    }

    public void ResetBoardImage()
    {
        WinningGameobject.SetActive(false);

        VisitedSamples = 0;
        isFinished = false;
        currentProgress = 0;
    }

    // Coroutine which check if this board image was finished and if it is winning
    public IEnumerator CheckIfFinishedAndShowWining()
    {
        while(isFinished == false)
        {
            yield return null;
        }

        if (isWinning == true)
        {
            WinningGameobject.SetActive(true);
            yield return WinAnimIn.Play();
        }
    }

    // method for setting board image sprite and winning option
    public void initialize(BoardImageSettings settings)
    {
        boardImage.sprite = settings.mySprite;
        Winning(settings.isWinning);
    }

    public void Winning(bool _isWinning)
    {
        isWinning = _isWinning;
    }


    // method for calculating progress of already scratched samples to deside if whole board image was revealed
    public void CalculateProgress()
    {
        if(isFinished == false)
        {
            VisitedSamples += 1;
            currentProgress = (float)VisitedSamples / AllSamples.Count();

            if (currentProgress >= 0.6f)
            {
                isFinished = true;
                VisitAllSamples();
                myTicketSection.UpdateAllCount(this);
            }
        }
    }

    public void VisitAllSamples()
    {
        foreach (var item in AllSamples)item.VisitSample();
    }

    public void EnableSamples(bool value)
    {
        foreach (var item in AllSamples) item.EnableSample(value);
    }
}
