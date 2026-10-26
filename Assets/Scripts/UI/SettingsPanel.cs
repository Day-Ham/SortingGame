using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public static SettingsPanel instance;
    
    //For player prefs
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MouseSensitivityKey = "MouseSensitivity";

    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeText;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeText;

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeText;

    [Header("Mouse Sensitivity")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityText;

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

        InitializeAudioSettings();
        InitializeSensitivitySettings();

        closeSettingsButton.onClick.AddListener(CloseSettingsMenu);
    }

    #region Audio
    private void InitializeAudioSettings()
    {
        SettingsManager settings = SettingsManager.Instance;

        // Load saved values, using SettingsManager values as defaults.
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, settings.GetMasterVolume());
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, settings.GetMusicVolume());
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, settings.GetSFXVolume());

        //Update sliders
        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;

        //Apply to settings
        settings.SetMasterVolume(masterVolume);
        settings.SetMusicVolume(musicVolume);
        settings.SetSFXVolume(sfxVolume);

        UpdateVolumeTexts();

        // Add listeners
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMasterVolume(float value)
    {
        masterVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SettingsManager.Instance.SetMasterVolume(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }

    private void SetMusicVolume(float value)
    {
        musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SettingsManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    private void SetSFXVolume(float value)
    {
        sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SettingsManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }
    private void UpdateVolumeTexts()
    {
        masterVolumeText.text = $"{Mathf.RoundToInt(masterVolumeSlider.value * 100)}%";
        musicVolumeText.text = $"{Mathf.RoundToInt(musicVolumeSlider.value * 100)}%";
        sfxVolumeText.text = $"{Mathf.RoundToInt(sfxVolumeSlider.value * 100)}%";
    }
    #endregion

    #region Mouse Sensitivity
    private void InitializeSensitivitySettings()
    {
        sensitivitySlider.minValue = 1f;
        sensitivitySlider.maxValue = 10f;

        float sensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey,SettingsManager.Instance.GetMouseSensitivity());

        sensitivitySlider.value = sensitivity;

        //Apply to settings
        SettingsManager.Instance.SetMouseSensitivity(sensitivitySlider.value);
        UpdateSensitivityText(sensitivitySlider.value);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    private void SetSensitivity(float value)
    {
        SettingsManager.Instance.SetMouseSensitivity(value);
        PlayerPrefs.SetFloat(MouseSensitivityKey, value);
        UpdateSensitivityText(value);
    }

    private void UpdateSensitivityText(float value)
    {
        sensitivityText.text = $"{value:F1}";
    }
    #endregion

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveListener(SetSensitivity);

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.RemoveListener(CloseSettingsMenu);

        PlayerPrefs.Save();

        if (instance == this)
            instance = null;
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
            gameUI.SetAsCurrentOpenMenu(gameUI.pauseMenu);
            gameUI.DisableUI(gameUI.settingsMenu);
        }
    }
}