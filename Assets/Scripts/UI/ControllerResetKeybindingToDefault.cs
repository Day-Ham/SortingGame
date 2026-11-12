using DG.Tweening;
using UnityEngine;

public class ControllerResetKeybindingToDefault : MonoBehaviour
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
        TweenMenuOpen(resetToDefaultPrompt);
    }

    private void TweenMenuOpen(GameObject menu)
    {
        menu.SetActive(true);
        Transform menuTransform = menu.transform;
        menuTransform.DOKill();
        menuTransform.localScale = Vector3.zero;
        menuTransform.DOScale(Vector3.one, 0.25f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }
}
