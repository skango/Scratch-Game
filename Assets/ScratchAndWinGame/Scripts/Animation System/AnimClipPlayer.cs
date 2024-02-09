using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;

public enum loopType
{
    PlayOnce,
    PingPong,
    Repeat
}

[System.Serializable]
public class AnimClipSettings
{
    [HideInInspector]
    public string name;
    public GameObject Object;
    public AnimationClip Animation;
    public loopType loopType;
    public bool EvaluateOnEnable = false;
    public float speed = 1;

    internal float animTime;
    internal float animTimeNormalized;
}


public class AnimClipPlayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float TestProgress;
    public List<AnimClipSettings> AnimClips = new List<AnimClipSettings>() { new AnimClipSettings ()};
    [Space]
    public float WaitInBetween = 0f;
    public bool PlayOnEnable = false;

    private void OnValidate()
    {
        foreach (var animClip in AnimClips)
        {
            if (animClip.Object != null) animClip.name = animClip.Object.name; 
        }

        //Editor Animation Preview
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            AnimationMode.StartAnimationMode();

            foreach (var AnimClip in AnimClips)
            {
                if (AnimClip.Animation != null) AnimationMode.SampleAnimationClip(AnimClip.Object, AnimClip.Animation, ProcessProgress(TestProgress, AnimClip));
            }

            if (TestProgress == 0)
            {
                AnimationMode.BeginSampling();
                AnimationMode.EndSampling();
                AnimationMode.StopAnimationMode();
            }
        }
#endif
    }

    //Support mwethod for Animation Editor Preview
    private float ProcessProgress(float testProgress, AnimClipSettings animClip)
    {
        animClip.animTime = testProgress * animClip.Animation.length;
        return animClip.animTime;
    }

    private void OnEnable()
    {
        TestProgress = 0f;

        foreach (var AnimClip in AnimClips)
        {
            if (AnimClip.EvaluateOnEnable == true) EvaluateAnimCurveBegining();
        }

        if (PlayOnEnable == true) StartCoroutine(Play());
    }

    //Evaluate first frame of animation
    public void EvaluateAnimCurveBegining()
    {
        foreach (var AnimClip in AnimClips)
        {
            AnimClip.Animation.SampleAnimation(AnimClip.Object, ProcessTime(0, AnimClip));
        }
    }

    //Private Play Animation Coroutine with skip if already played option
    private IEnumerator PlayClipWaitToFinish(AnimClipSettings clipSettings, bool SkipIfDone = false)
    {
        if (SkipIfDone == true)
        {
            if(clipSettings.loopType == loopType.PlayOnce)
            {
                if (clipSettings.animTimeNormalized >= 1) yield break;
            }
        }

        float currentTime = 0;

        while (true)
        {
            currentTime += Time.deltaTime;
            clipSettings.Animation.SampleAnimation(clipSettings.Object, ProcessTime(currentTime, clipSettings));

            if (clipSettings.loopType == loopType.PlayOnce)
            {
                if (clipSettings.animTimeNormalized >= 1) yield break;
            }

            yield return null;
        }
    }

    // Process Time to calculate looping options
    public float ProcessTime(float currentTime, AnimClipSettings clipSettings)
    {
        if (clipSettings.loopType == loopType.PlayOnce) clipSettings.animTime = currentTime * clipSettings.speed;
        if (clipSettings.loopType == loopType.Repeat) clipSettings.animTime = Mathf.Repeat(currentTime * clipSettings.speed, clipSettings.Animation.length);
        if (clipSettings.loopType == loopType.PingPong) clipSettings.animTime = Mathf.PingPong(currentTime * clipSettings.speed, clipSettings.Animation.length);

        clipSettings.animTimeNormalized = clipSettings.animTime / clipSettings.Animation.length;

        return clipSettings.animTime;
    }

    //Public Play coroutine supporting to play multiple objects
    public IEnumerator Play(bool SkipIfDone = false)
    {
        List<IEnumerator> AnimRoutines = new List<IEnumerator>();

        foreach (var AnimClip in AnimClips)
        {
            AnimRoutines.Add(PlayClipWaitToFinish(AnimClip, SkipIfDone));
        }

        yield return Utility.runCoroutinesAllAtOnceWaitToFinish(this, AnimRoutines, WaitInBetween);
    }

    [ContextMenu("Start Playback")]
    void StartPlayback()
    {
        StartCoroutine(Play());
    }

    [ContextMenu("Start Playback Skip If Done")]
    void StartPlaybackSkipIfDone()
    {
        StartCoroutine(Play(true));
    }

    [ContextMenu("Stop Playback")]
    public void StopPlayback()
    {
        StopAllCoroutines();
    }
}
