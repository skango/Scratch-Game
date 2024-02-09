using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelManager : MonoBehaviour
{
    public static WinPanelManager instance;
    public GameObject WiningPanel;

    public Image WinTypeImage;
    public TextMeshProUGUI PriceText;

    public bool Clicked = false;

    public GameObject toInstatiate;

    private void Awake()
    {
        if (instance == null) instance = this;
        WiningPanel.SetActive(false);
    }

    public void WinPanelClicked()
    {
        Clicked = true;
    }

    //Main Coroutine to show winning panel
    public IEnumerator ShowWiningPanel(WinSettings winSettings)
    {
        yield return new WaitForSeconds(1f);

        if (winSettings.DidIWon == true)
        {
            SaveLoadManager.instance.increaseValues(winSettings.type, winSettings.PricePreset.Price);
            SaveLoadManager.instance.SaveDataToPlayerPrefs();

            WiningPanel.SetActive(true);

            SetWinningUI(winSettings);

            Clicked = false;

            while (Clicked == false)
            {
                yield return null;
            }

            WiningPanel.SetActive(false);

            if (winSettings.type == AreaType.Money) yield return PanelFinishAnimation(SaveLoadManager.instance.MoneyOldScore, SaveLoadManager.instance.MoneyScore, ScoreBoardManager.instance.MainPart);
            if (winSettings.type == AreaType.Bonus) yield return PanelFinishAnimation(SaveLoadManager.instance.BonusOldScore, SaveLoadManager.instance.BonusScore, ScoreBoardManager.instance.BonusPart);
        }
    }

    // Coroutine to play additional animations after we clicked on claim button inside winning panel
    private IEnumerator PanelFinishAnimation(int OldScore, int newScore, UIPart uiPart)
    {
        List<IEnumerator> ListOfAnimationRoutines = new List<IEnumerator>();

        ListOfAnimationRoutines.Add(uiPart.AnimationIn.Play());
        ListOfAnimationRoutines.Add(Utility.ChangeNumbersGradually(OldScore, newScore, uiPart.UICountText, 2f));

        List<GameObject> myGoList = InstantiateGameObject(4, toInstatiate, WinTypeImage.sprite);

        foreach (var item in myGoList)
        {
            ListOfAnimationRoutines.Add(Utility.MoveTo(item.transform.position, uiPart.UISpriteTransform.position, item.transform, 1.5f));
            ListOfAnimationRoutines.Add(item.GetComponent<AnimClipPlayer>().Play());
        }

        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, ListOfAnimationRoutines, 0.15f);

        foreach (var item in myGoList)
        {
            Destroy(item.gameObject);
        }
    }

    // Method to fill sprite for the WinTypeImage
    private void SetWinningUI(WinSettings winSettings)
    {
        PriceText.text = winSettings.PricePreset.Price.ToString();

        if (winSettings.type == AreaType.Money) WinTypeImage.sprite = SettingsDatabase.instance.MoneySprite;
        if (winSettings.type == AreaType.Bonus) WinTypeImage.sprite = SettingsDatabase.instance.BonusSprite;
    }


    // Method to insantiate multiple copies of gameobject
    public List<GameObject> InstantiateGameObject(int howMuch, GameObject toInstatiate, Sprite mySprite)
    {
        List<GameObject> myGameObjectList = new List<GameObject>();

        for (int i = 0; i < howMuch; i++)
        {
            GameObject myGO = Instantiate(toInstatiate);
            myGO.GetComponent<SpriteRenderer>().sprite = mySprite;
            myGameObjectList.Add(myGO);
        }

        return myGameObjectList;
    }
}
