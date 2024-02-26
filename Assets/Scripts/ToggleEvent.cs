using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arttteo.UI
{
    public class ToggleEvent : MonoBehaviour
    {
        public UnityEvent FunctionOne, FunctionTwo;
        bool pressed = false;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                ToggleSwitch();
            });
        }

        public void ToggleSwitch()
        {
            if (!pressed)
            {
                FunctionOne.Invoke();
            }else
            {
                FunctionTwo.Invoke();
            }
            pressed = !pressed;
        }
    }
}
