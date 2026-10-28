using UnityEngine;
using UnityEngine.UI;

public class InputPrompt : MonoBehaviour
{
    [SerializeField] private string actionName;
    [SerializeField] private Image promptImage;

    public string ActionName => actionName;

    private void OnEnable()
    {
        if (InputIconManager.Instance != null)
            InputIconManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (InputIconManager.Instance != null)
            InputIconManager.Instance.Unregister(this);
    }

    public void SetIcon(Sprite sprite)
    {
        if (promptImage == null) return;

        promptImage.sprite = sprite;
        promptImage.enabled = sprite != null;
    }
}
