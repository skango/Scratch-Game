using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScratchingArea : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public bool ScratchingEnabled = false;
    public Image ScratchImage;
    public Color BGColor;

    private void Awake()
    {
        ScratchImage = GetComponent<Image>();
    }

    public void ResetScratchArea()
    {
        gameObject.SetActive(true);
        ScratchingEnabled = false;
    }

    //coroutine which enable scrathing and run animation which blends disabled color to enabled one
    public IEnumerator enableScratching()
    {
        ScratchingEnabled = true;
        yield return Utility.lerpBetweenTwoColors(ScratchImage, BGColor, 0.5f);
    }

    // coroutine which finalize scratching
    public IEnumerator FinishScratching(List<BoardImage> BoardImageList)
    {
        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, BoardImageList.Select(a => a.ScratchFinalizeAnim.Play()).ToList());

        DrawingManager.instance.ClearInstances();

        foreach (var item in BoardImageList)
        {
            item.ScratchFinalizeAnim.EvaluateAnimCurveBegining();
        }

        gameObject.SetActive(false);
    }

    // UI event which is triggered when you start mouse dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ScratchingEnabled == true) DrawingManager.instance.OnBeginDrag(eventData);
    }

    // UI event which is triggered when you drag with your mouse
    public void OnDrag(PointerEventData eventData)
    {
        if (ScratchingEnabled == true)
        {
            ScratchImage.raycastTarget = false;
            DrawingManager.instance.Draw(eventData);
            ScratchImage.raycastTarget = true;
        }
    }
}
