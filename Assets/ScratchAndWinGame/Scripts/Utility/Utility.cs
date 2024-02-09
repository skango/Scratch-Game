using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Utility
{
    //Random bool method
    public static bool GetRandomBool()
    {
        return System.Convert.ToBoolean(Random.Range(0,2));
    }

    //Generic method which return random member from list
    public static T RandomFromList<T>(List<T> GenericList)
    {
        return GenericList[Random.Range(0, GenericList.Count)];
    }

    //Generic method which return random member from list and removes it aswell from the list
    public static T RandomFromListRemove<T>(List<T> GenericList)
    {
        int RandomIndex = Random.Range(0, GenericList.Count);
        T RandomMember = GenericList[RandomIndex];
        GenericList.RemoveAt(RandomIndex);
        return RandomMember;
    }

    // Generic method which shuffle the list
    public static List<T> ShuffleList<T>(List<T> GenericList)
    {
        List<T> sorted = GenericList.OrderBy(a => Random.Range(0, 100)).ToList();
        return sorted;
    }

    // Coroutine which can run list of coroutines and wait untill all of them will finish
    public static IEnumerator runCoroutinesAllAtOnceWaitToFinish(MonoBehaviour monoGameObject, List<IEnumerator> myCorouList, float WaitInBetween = 0)
    {
        List<IEnumerator> newRoutineList = new List<IEnumerator>();

        foreach (var item in myCorouList)
        {
            monoGameObject.StartCoroutine(CheckIfRunning(item, newRoutineList));
            if (WaitInBetween != 0) yield return new WaitForSeconds(WaitInBetween);
        }

        while(newRoutineList.Count != 0)
        {
            yield return null;
        }
    }

    // Support method for runCoroutinesAllAtOnceWaitToFinish which add coroutine to the list and after it finish it removes it from that list
    private static IEnumerator CheckIfRunning(IEnumerator routine, List<IEnumerator> newRoutineList)
    {
        newRoutineList.Add(routine);
        yield return routine;
        newRoutineList.Remove(routine);
    }

    // Coroutine which blend between two colors over time
    public static IEnumerator lerpBetweenTwoColors(Image myImage, Color toColor, float inTime)
    {
        float elapsedTime = 0;
        Color fromColor = myImage.color;

        while (elapsedTime < inTime)
        {
            Color newColor = Color.Lerp(fromColor, toColor, elapsedTime / inTime);
            myImage.color = newColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        myImage.color = toColor;
    }

    //Coroutine which change number string gradually over time
    public static IEnumerator ChangeNumbersGradually(float from, float to, TextMeshProUGUI myText, float inTime)
    {
        float elapsedTime = 0;

        while (elapsedTime <= inTime)
        {
            elapsedTime += Time.deltaTime;

            myText.text = Mathf.FloorToInt(Mathf.Lerp(from,to, elapsedTime / inTime)).ToString();

            yield return null;
        }

        myText.text = to.ToString();
    }

    //Coroutine which move gameobject over time
    public static IEnumerator MoveTo(Vector3 from, Vector3 to, Transform transform, float inTime)
    {
        float elapsedTime = 0;

        while (elapsedTime<= inTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(from,to, elapsedTime/ inTime);
            yield return null;
        }

        transform.position = to;
    }
}
