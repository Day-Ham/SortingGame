using UnityEngine;

public class ControllerResetKeybindingToDefault : MonoBehaviour
{
    [SerializeField] private GameObject resetToDefaultPrompt;
    [SerializeField] private SetsUIElementToSelectOnInteraction selectable;
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

        if (selectable == null) return;

        selectable.JumpToElement();
    }
}
