using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputIconManager : MonoBehaviour
{
    public static InputIconManager Instance { get; private set; }
    [SerializeField] private PlayerInput playerInput;

    [Header("Keyboard and Mouse")]
    [SerializeField] private KeyboardMouseIcons keyboardMouse;
   
    [Header("Gamepad")]
    [SerializeField] private GamepadIcons xbox;
    [SerializeField] private GamepadIcons ps4;
    private readonly HashSet<InputPrompt> prompts = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (playerInput == null && UserInput.instance != null)
            playerInput = UserInput.instance.GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
                
        if (InputDeviceDetector.Instance != null)
            InputDeviceDetector.Instance.OnInputModeChanged += OnInputModeChanged;
    }

    private void Start()
    {
        if (InputDeviceDetector.Instance != null) 
            InputDeviceDetector.Instance.OnInputModeChanged += OnInputModeChanged;

        foreach (var prompt in FindObjectsByType<InputPrompt>())
            Register(prompt);

        RefreshAll();
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;

        if (InputDeviceDetector.Instance != null)
            InputDeviceDetector.Instance.OnInputModeChanged -= OnInputModeChanged;
    }
    private void OnInputModeChanged(InputDeviceDetector.InputMode mode)
    {
        RefreshAllForMode(mode);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Register(InputPrompt prompt)
    {
        if (prompt == null) return;

        prompts.Add(prompt);
        Refresh(prompt);
    }

    public void Unregister(InputPrompt prompt)
    {
        prompts.Remove(prompt);
    }

    private void RefreshAllForMode(InputDeviceDetector.InputMode mode)
    {
        foreach (var prompt in prompts)
            RefreshForMode(prompt, mode);
    }

    private void RefreshForMode(InputPrompt prompt, InputDeviceDetector.InputMode mode)
    {
        if (playerInput == null || prompt == null) return;

        InputAction action = playerInput.actions.FindAction(prompt.ActionName, false);

        if (action == null) return;

        string path = GetBindingPath(action, mode);

        if (string.IsNullOrEmpty(path))
        {
            prompt.SetIcon(null);
            return;
        }

        Sprite sprite;

        if (IsGamepadPath(path))
        {
            Gamepad gamepad = Gamepad.current;

            bool isPS4 = gamepad != null &&
                InputSystem.IsFirstLayoutBasedOnSecond(gamepad.layout, "DualShockGamepad");

            sprite = isPS4 ? ps4.GetSprite(path) : xbox.GetSprite(path);
        }
        else
        {
            sprite = keyboardMouse.GetSprite(path);
        }

        prompt.SetIcon(sprite);
    }


    public void RefreshAll()
    {
        foreach (var prompt in prompts)
            Refresh(prompt);
    }

    public void Refresh(InputPrompt prompt)
    {
        if (playerInput == null || prompt == null) return;

        InputAction action = playerInput.actions.FindAction(prompt.ActionName, false);

        if (action == null) return;

        string path = GetBindingPath(action, InputDeviceDetector.Instance.CurrentMode);

        if (string.IsNullOrEmpty(path))
        {
            prompt.SetIcon(null);
            return;
        }

        Sprite sprite = null;

        if (IsGamepadPath(path))
        {
            Gamepad gamepad = Gamepad.current;

            bool isPS4 = gamepad != null &&
                InputSystem.IsFirstLayoutBasedOnSecond(
                    gamepad.layout, "DualShockGamepad");

            sprite = isPS4
                ? ps4.GetSprite(path)
                : xbox.GetSprite(path);
        }
        else if (IsKeyboardMousePath(path))
        {
            sprite = keyboardMouse.GetSprite(path);
        }

        prompt.SetIcon(sprite);
    }


    private string GetBindingPath(InputAction action, InputDeviceDetector.InputMode mode)
    {
        foreach (var binding in action.bindings)
        {
            if (binding.isComposite) continue;

            string path = binding.effectivePath;

            if (string.IsNullOrEmpty(path)) continue;

            bool matchesMode = mode == InputDeviceDetector.InputMode.Gamepad
                ? IsGamepadPath(path)
                : IsKeyboardMousePath(path);

            if (matchesMode)
                return path;
        }

        return null;
    }

    private bool IsGamepadPath(string path)
    {
        string layout = GetDeviceLayout(path);

        return !string.IsNullOrEmpty(layout) && InputSystem.IsFirstLayoutBasedOnSecond(layout, "Gamepad");
    }

    private bool IsKeyboardMousePath(string path)
    {
        string layout = GetDeviceLayout(path);

        return !string.IsNullOrEmpty(layout) && (InputSystem.IsFirstLayoutBasedOnSecond(layout, "Keyboard") || InputSystem.IsFirstLayoutBasedOnSecond(layout, "Mouse"));
    }

    private string GetDeviceLayout(string path)
    {
        if (string.IsNullOrEmpty(path) || !path.StartsWith("<"))
            return null;

        int end = path.IndexOf('>');

        return end > 1 ? path.Substring(1, end - 1) : null;
    }

    private void OnActionChange(object actionOrMap, InputActionChange change)
    {
        if (change == InputActionChange.BoundControlsChanged)
            RefreshAll();
    }
}
