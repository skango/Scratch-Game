using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    private Image myImage;
    private BoardImage myBoardImage;

    private void Awake()
    {
        myImage = GetComponent<Image>();
        myImage.color = Color.clear;
        EnableSample(false);
    }

    public void EnableSample(bool value)
    {
        myImage.raycastTarget = value;
    }

    public void VisitSample()
    {
        EnableSample(false);
        
        myBoardImage.CalculateProgress();
    }

    public void SetBoardImage(BoardImage boardImage)
    {
        myBoardImage = boardImage;
    }
}
