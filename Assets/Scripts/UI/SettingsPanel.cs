using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public static SettingsPanel instance;
    
    //For player prefs
    
    

    [Header("Audio")]






    [Header("Mouse Sensitivity")]


    [Header("Other UI")]
    [SerializeField] private Button closeSettingsButton;

    private GameUIManager gameUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        gameUI = GameUIManager.instance;

        

        closeSettingsButton.onClick.AddListener(CloseSettingsMenu);
    }


    #region Mouse Sensitivity
    

    

    
    #endregion

    private void OnDestroy()
    {
        //if (masterVolumeSlider != null)
        //    masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);

        //if (musicVolumeSlider != null)
        //    musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);

        //if (sfxVolumeSlider != null)
        //    sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);

        //if (sensitivitySlider != null)
        //    sensitivitySlider.onValueChanged.RemoveListener(SetSensitivity);

        //if (closeSettingsButton != null)
        //    closeSettingsButton.onClick.RemoveListener(CloseSettingsMenu);

        //PlayerPrefs.Save();

        //if (instance == this)
        //    instance = null;
    }

    public void CloseSettingsMenu()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.buildIndex == 0)
        {
            TitleScreenManager title = FindAnyObjectByType<TitleScreenManager>();
            title.OpenMainMenu();
            return;
        }
        
        if (gameUI == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //gameUI.ReturnFromSettingsToPause();
        }
    }
}