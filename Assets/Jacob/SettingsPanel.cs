using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public static SettingsPanel instance;

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

    GameUIManager gameUI;

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
        masterVolumeSlider.value = SettingsManager.Instance.GetMasterVolume();
        musicVolumeSlider.value = SettingsManager.Instance.GetMusicVolume();
        sfxVolumeSlider.value = SettingsManager.Instance.GetSFXVolume();

        UpdateVolumeTexts();

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMasterVolume(float value)
    {
        masterVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SettingsManager.Instance.SetMasterVolume(value);
    }

    private void SetMusicVolume(float value)
    {
        musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SettingsManager.Instance.SetMusicVolume(value);
    }

    private void SetSFXVolume(float value)
    {
        sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        SettingsManager.Instance.SetSFXVolume(value);
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

        sensitivitySlider.value = SettingsManager.Instance.GetMouseSensitivity();
        UpdateSensitivityText(sensitivitySlider.value);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    private void SetSensitivity(float value)
    {
        SettingsManager.Instance.SetMouseSensitivity(value);
        UpdateSensitivityText(value);
    }

    private void UpdateSensitivityText(float value)
    {
        sensitivityText.text = $"{value:F1}";
    }
    #endregion

    private void OnDestroy()
    {
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        sensitivitySlider.onValueChanged.RemoveListener(SetSensitivity);
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