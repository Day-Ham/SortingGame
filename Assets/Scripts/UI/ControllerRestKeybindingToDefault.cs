using System;
using UnityEngine;
using UnityEngine.UI;

public class ControllerRestKeybindingToDefault : MonoBehaviour
{
    [SerializeField] private GameObject resetToDefaultPrompt;

    // Update is called once per frame
    void Update()
    {
        if (UserInput.instance == null) return;

        if (UserInput.instance.ResetBindingsInput)
        {
            OpenDefaultPrompt();
        }
    }

    private void OpenDefaultPrompt()
    {
        resetToDefaultPrompt.SetActive(true);
    }
}
