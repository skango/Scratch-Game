using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to drive initial scrathing when our game starts
public class StartSequenceManager : MonoBehaviour
{
    public static StartSequenceManager instance;

    public AnimClipPlayer AnimIn;
    public AnimClipPlayer AnimOut;
    public AnimClipPlayer AnimIdle;

    public GameObject InitialBG;
    public TrailRenderer trail;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public IEnumerator StartSequence()
    {
        InitialBG.SetActive(true);
        trail.emitting = true;

        yield return AnimIn.Play();

        AnimIdle.StopPlayback();

        yield return AnimOut.Play();

        trail.emitting = false;
        trail.Clear();
        InitialBG.SetActive(false);
    }
}
