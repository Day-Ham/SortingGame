using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Button returnToTitleScreenButton;
    [SerializeField] private Button quitButton;

    [Header("Status")]
    [SerializeField] private bool isPaused;

    [Header("Input Action")]
    private PlayerInput playerInput;
    private InputAction escapeAction;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interactAction;
    private InputAction throwAction;
    private InputAction skill1Action;
    private InputAction skill2Action;
    private InputAction skill3Action;

    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private PlayerInteraction playerInteraction;

    [Header("SceneLoader")]
    private SceneLoader sceneLoader;

    private void Awake()
    {
        playerInput = FindAnyObjectByType<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerLook = FindAnyObjectByType<PlayerLook>();
        playerInteraction = GetComponent<PlayerInteraction>();

        escapeAction = playerInput.actions.FindAction("Escape");
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        interactAction = playerInput.actions.FindAction("Interact");
        throwAction = playerInput.actions.FindAction("Throw");
        skill1Action = playerInput.actions.FindAction("Skill1");
        skill2Action = playerInput.actions.FindAction("Skill2");
        skill3Action = playerInput.actions.FindAction("Skill3");

        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        returnToTitleScreenButton.onClick.AddListener(ReturnToTitleScreen);
        quitButton.onClick.AddListener(QuitGame);

        //sceneLoader = FindAnyObjectByType<SceneLoader>();
        //if (sceneLoader == null) Debug.Log("Scene Loader not found");

        //deactivate pause 
        ResumeGame();
        CloseSettings();
    }

    private void OnEnable()
    {
        escapeAction.performed += OnEscapePressed;
    }

    private void OnDisable()
    {
        escapeAction.performed -= OnEscapePressed;
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        if (isPaused && settingsMenu.activeInHierarchy)
        {
            CloseSettings();
        }
        else if (isPaused && !settingsMenu.activeInHierarchy)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        OpenPauseMenu();
        UnlockCursor();
        DisablePlayerControl();
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    
        ClosePauseMenu();
        LockCursor();
        EnablePlayerControl();
    }

    private void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    private void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
    }

    private void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }

    private void CloseSettings()
    {
        settingsMenu.SetActive(false);
    }

    private void DisablePlayerControl()
    {
        moveAction.Disable();
        lookAction.Disable();

        interactAction.Disable();
        throwAction.Disable();

        skill1Action.Disable();
        skill2Action.Disable();
        skill3Action.Disable();
    }

    private void EnablePlayerControl()
    {
        moveAction.Enable();
        lookAction.Enable();

        interactAction.Enable();
        throwAction.Enable();

        skill1Action.Enable();
        skill2Action.Enable();
        skill3Action.Enable();
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

    private void ReturnToTitleScreen()
    {
        Debug.Log("Returned to title screen");
    }

    private void QuitGame()
    {
        Debug.Log("Quit and saved game");
    }
}
