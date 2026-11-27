using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public bool isPaused;

    [Header("UI")]
    public GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button returnToTitleScreenButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (UserInput.instance == null || SettingsManager.Instance == null || GameUIManager.instance == null) return;

        if (UserInput.instance.EscapeInput)
        {
            if (pauseMenu.activeInHierarchy)
            {
                if (SettingsManager.Instance.settingsMenu.activeInHierarchy)
                {
                    return;
                }
                else
                {
                    ClosePauseMenu();
                }
            }
            else if (GameUIManager.instance.currentOpenMenu == null)
            {
                OpenPauseMenu();
            }
        }
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(ClosePauseMenu);
        returnToTitleScreenButton.onClick.AddListener(LoadTitleScreen);
        quitButton.onClick.AddListener(QuitGame);

        ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        DisablePlayerControl();
        UnlockCursor();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        EnablePlayerControl();
        LockCursor();
    }

    public void DisablePlayerControl()
    {
        UserInput.instance.SetPlayerInput(false);  
    }

    public void EnablePlayerControl()
    {
        UserInput.instance.SetPlayerInput(true);   
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;

        if (InputDeviceDetector.Instance.CurrentMode == InputDeviceDetector.InputMode.Gamepad)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    private void OpenPauseMenu()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.isPauseMenu = true;
            GameUIManager.instance.SetAsCurrentOpenMenu(pauseMenu);
        }
    }

    private void ClosePauseMenu()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.isPauseMenu = true;
            GameUIManager.instance.CloseMenu(pauseMenu);
        }  
    }

    private void LoadTitleScreen()
    {
        if (GameUIManager.instance != null)
        {
            Debug.Log("game manager ok");
            ClosePauseMenu();
        }

        if (SaveManager.instance != null)
        {
            Debug.Log("save manager ok");
            SaveManager.instance.SaveGame();
        } 

        if (SceneLoader.Instance != null)
        {
            Debug.Log("scene loader ok");
            SceneLoader.Instance.LoadSceneByIndex(0);
        }      
    }

    private void QuitGame()
    {
        if (GameUIManager.instance != null)
            GameUIManager.instance.CloseMenu(pauseMenu);
        
        DisablePlayerControl();
        UnlockCursor();

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.QuitGame();
    }
}
