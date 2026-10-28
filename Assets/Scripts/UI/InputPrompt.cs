using UnityEngine;
using UnityEngine.UI;

public class InputPrompt : MonoBehaviour
{
    [SerializeField] private Image promptImage;
    [SerializeField] private Sprite keyboardMouseSprite;
    [SerializeField] private Sprite gamepadSprite;

    private void OnEnable()
    {
        if (InputDeviceDetector.Instance == null)
            return;

        InputDeviceDetector.Instance.OnInputModeChanged += UpdateSprite;
        UpdateSprite(InputDeviceDetector.Instance.CurrentMode);
    }

    private void OnDisable()
    {
        if (InputDeviceDetector.Instance != null)
            InputDeviceDetector.Instance.OnInputModeChanged -= UpdateSprite;
    }

    private void UpdateSprite(InputDeviceDetector.InputMode mode)
    {
        promptImage.sprite = mode ==
            InputDeviceDetector.InputMode.Gamepad
            ? gamepadSprite
            : keyboardMouseSprite;
    }
}
