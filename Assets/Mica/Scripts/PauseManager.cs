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
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        EnablePlayerControl();
    }

    public void DisablePlayerControl()
    {
        UserInput.instance.SetPlayerInput(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnablePlayerControl()
    {
        UserInput.instance.SetPlayerInput(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
