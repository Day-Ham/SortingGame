using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class InputDeviceDetector : MonoBehaviour
{
    public static InputDeviceDetector Instance { get; private set; }
    public enum InputMode
    {
        KeyboardMouse,
        Gamepad
    }

    private PauseManager pause;

    public InputMode CurrentMode { get; private set; }
        = InputMode.KeyboardMouse;

    public event Action<InputMode> OnInputModeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        pause = PauseManager.instance;
    }

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(
        UnityEngine.InputSystem.LowLevel.InputEventPtr eventPtr,
        InputDevice device)
    {
        if (device is Keyboard || device is Mouse)
        {
            if (eventPtr.HasButtonPress() || device is Mouse)
                SetInputMode(InputMode.KeyboardMouse);

            return;
        }

        if (device is not Gamepad gamepad)
            return;

        foreach (var control in gamepad.allControls)
        {
            if (eventPtr.EnumerateChangedControls().Contains(control))
            {
                if (control is UnityEngine.InputSystem.Controls.ButtonControl button &&
                    button.isPressed)
                {
                    SetInputMode(InputMode.Gamepad);
                    return;
                }

                if (control is UnityEngine.InputSystem.Controls.AxisControl axis &&
                    Mathf.Abs(axis.ReadValue()) > 0.2f)
                {
                    SetInputMode(InputMode.Gamepad);
                    return;
                }

                if (control is UnityEngine.InputSystem.Controls.StickControl stick &&
                    stick.ReadValue().magnitude > 0.2f)
                {
                    SetInputMode(InputMode.Gamepad);
                    return;
                }
            }
        }
    }


    private void SetInputMode(InputMode mode)
    {
        if (CurrentMode == mode) return;

        CurrentMode = mode;
        
        Debug.Log("Input mode changed to: " + CurrentMode);

        OnInputModeChanged?.Invoke(mode);

        if (mode == InputMode.Gamepad)
        {
            Cursor.visible = false;
        }
        else if (mode == InputMode.KeyboardMouse)
        {
            Cursor.visible = true;
        }
    }
}
