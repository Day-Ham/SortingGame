using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUpgrade : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private GameObject skillsMenu;

    PlayerInput playerInput;

    InputAction upgradeAction;
    InputAction skillsAction;
    InputAction escapeAction;

    InputAction moveAction;
    InputAction lookAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        upgradeAction = playerInput.actions.FindAction("UpgradeMenu");
        skillsAction = playerInput.actions.FindAction("SkillsMenu");
        escapeAction = playerInput.actions.FindAction("Escape");
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
    }

    private void Start()
    {
        CloseMenus();
    }

    private void OnEnable()
    {
        upgradeAction.performed += OnUpgradePressed;
        skillsAction.performed += OnSkillsPressed;
        escapeAction.performed += OnEscapePressed;
    }

    private void OnDisable()
    {
        upgradeAction.performed -= OnUpgradePressed;
        skillsAction.performed -= OnSkillsPressed;
        escapeAction.performed -= OnEscapePressed;
    }

    private void OnUpgradePressed(InputAction.CallbackContext context)
    {
        OpenUpgradeMenu();
    }

    private void OnSkillsPressed(InputAction.CallbackContext context)
    {
        OpenSkillsMenu();
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        CloseMenus();
    }

    public void OpenUpgradeMenu()
    {
        upgradeMenu.SetActive(true);
        skillsMenu.SetActive(false);

        DisablePlayerControl();
        UnlockCursor();
    }

    public void OpenSkillsMenu()
    {
        skillsMenu.SetActive(true);
        upgradeMenu.SetActive(false);

        DisablePlayerControl();
        UnlockCursor();
    }

    public void CloseMenus()
    {
        upgradeMenu.SetActive(false);
        skillsMenu.SetActive(false);

        EnablePlayerControl();
        LockCursor();
    }

    private void DisablePlayerControl()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    private void EnablePlayerControl()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}