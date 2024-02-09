using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    public GameObject MainMenuParent;
    public GameObject GamePlayParent;

    public AnimClipPlayer AnimIn;
    public AnimClipPlayer AnimOut;

    bool menuClicked = false;

    private void Awake()
    {
        MainMenuParent.SetActive(true);
        GamePlayParent.SetActive(false);

        if (instance == null) instance = this;
    }

    // Coroutine which waits untill we click on the play button
    public IEnumerator waitUntilClicked()
    {
        yield return AnimIn.Play();

        menuClicked = false;

        while (menuClicked == false)
        {
            yield return null;
        }

        yield return AnimOut.Play();

        MainMenuParent.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        GamePlayParent.SetActive(true);

        yield return null;
    }

    public void Clicked()
    {
        menuClicked = true;
    }
}
