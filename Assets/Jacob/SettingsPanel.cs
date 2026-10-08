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

        closeSettingsButton.onClick.AddListener(CloseSettingsMenu);
    }

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


    private void OnDestroy()
    {
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
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