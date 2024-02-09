using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

[System.Serializable]
public class OnClick
{
    public string name;
    public Button myButton;
    public ButtonClickedEvent ButtonClick;

    private void OnValidate()
    {
        if (myButton != null) ButtonClick = myButton.onClick;
    }
}
// centralised Button Manager
[ExecuteInEditMode]
public class ButtonManager : MonoBehaviour
{
    public List<OnClick> myButtons = new List<OnClick>();

    private void OnEnable()
    {
        foreach (var button in myButtons)
        {
            button.myButton.onClick = button.ButtonClick;
        }
    }

    private void OnDisable()
    {
        foreach (var button in myButtons)
        {
            button.myButton.onClick = null;
        }
    }
}
