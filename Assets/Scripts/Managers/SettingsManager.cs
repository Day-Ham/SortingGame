using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Mouse Sensitivity")]
    [SerializeField] private float mouseSensitivity = 5f;
    private const string MouseSensitivityKey = "MouseSensitivity";
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityText;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    //Master volume
    [SerializeField] private float masterVolume = 1f;
    private const string MasterVolumeKey = "MasterVolume";
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeText;

    //Music volume
    [SerializeField] private float musicVolume = 1f;
    private const string MusicVolumeKey = "MusicVolume";
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Text musicVolumeText;

    //SFX volume
    [SerializeField] private float sfxVolume = 1f;
    private const string SFXVolumeKey = "SFXVolume";
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text sfxVolumeText;

    [Header("Settings Menu")]
    public GameObject settingsMenu;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private GameObject keybindingOverlay;
    [SerializeField] private GameObject duplicatePrompt;
    [SerializeField] private GameObject defaultPrompt;
    [SerializeField] private Button cancelDefaultButton;
    public bool isSecondarySelected;

    private SetsUIElementToSelectOnInteraction pauseMenuSetUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        settingsBackButton.onClick.AddListener(CloseSettingsMenu);
        cancelDefaultButton.onClick.AddListener(CancelDefault);

        InitializeAudioSettings();
        InitializeMouseSensitivitySettings();

        isSecondarySelected = false;

        pauseMenuSetUI = PauseManager.instance.pauseMenu.GetComponent<SetsUIElementToSelectOnInteraction>();
    }

    private void Update()
    {
        if (UserInput.instance == null) return;

        if (UserInput.instance.EscapeInput && !isSecondarySelected)
        {
            if (keybindingOverlay.activeInHierarchy || duplicatePrompt.activeInHierarchy) return;

            if (defaultPrompt.activeInHierarchy)
            {
                CancelDefault();
                return;
            }

            if (settingsMenu.activeInHierarchy)
            {
                CloseSettingsMenu();
            }
        }
    }

    #region Audio
    private void InitializeAudioSettings()
    {
        // Load saved values
        float masterVolumePref = PlayerPrefs.GetFloat(MasterVolumeKey, masterVolume);
        float musicVolumePref = PlayerPrefs.GetFloat(MusicVolumeKey, musicVolume);
        float sfxVolumePref = PlayerPrefs.GetFloat(SFXVolumeKey, sfxVolume);

        //Update sliders
        masterVolumeSlider.value = masterVolumePref;
        musicVolumeSlider.value = musicVolumePref;
        sfxVolumeSlider.value = sfxVolumePref;

        //Apply to settings
        SetMasterVolume(masterVolumePref);
        SetMusicVolume(musicVolumePref);
        SetSFXVolume(sfxVolumePref);

        UpdateVolumeTexts();

        // Add listeners
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    private void SetMasterVolume(float value)
    {
        masterVolume = value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
        masterVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
        musicVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
        sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    private void UpdateVolumeTexts()
    {
        masterVolumeText.text = $"{Mathf.RoundToInt(masterVolumeSlider.value * 100)}%";
        musicVolumeText.text = $"{Mathf.RoundToInt(musicVolumeSlider.value * 100)}%";
        sfxVolumeText.text = $"{Mathf.RoundToInt(sfxVolumeSlider.value * 100)}%";
    }
    #endregion

    #region Sensitivity
    private void InitializeMouseSensitivitySettings()
    {
        sensitivitySlider.minValue = 1f;
        sensitivitySlider.maxValue = 10f;

        float sensitivityPref = PlayerPrefs.GetFloat(MouseSensitivityKey, mouseSensitivity);

        sensitivitySlider.value = sensitivityPref;

        //Apply to settings
        SetMouseSensitivity(sensitivityPref);
        UpdateSensitivityText(sensitivityPref);

        sensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
    }

    private void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
        PlayerPrefs.SetFloat(MouseSensitivityKey, value);
        UpdateSensitivityText(value);
    }

    private void UpdateSensitivityText(float value)
    {
        sensitivityText.text = $"{value:F1}";
    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }
    #endregion

    #region Settings Menu
    private void OpenSettingsMenu()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.isSettingsMenu = true;
            GameUIManager.instance.SetAsCurrentOpenMenu(settingsMenu);

            if (pauseMenuSetUI != null)
                pauseMenuSetUI.enabled = false;
        }
    }

    private void CloseSettingsMenu()
    {
        if (GameUIManager.instance != null && PauseManager.instance != null)
        {
            GameUIManager.instance.isSettingsMenu = true;
            GameUIManager.instance.CloseMenu(settingsMenu);

            if (pauseMenuSetUI != null)
                pauseMenuSetUI.enabled = true;

            return;
        }
        else
        {
            TitleScreenManager title = FindAnyObjectByType<TitleScreenManager>();
            if (title != null)
                title.OpenMainMenu();
        }
    }

    private void CancelDefault()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.TweenClose(defaultPrompt);
        }
    }
    #endregion
}
