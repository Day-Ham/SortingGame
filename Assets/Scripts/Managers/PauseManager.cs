using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public bool isPaused;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        isPaused = false;
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
        Cursor.visible = true;
    }
}
