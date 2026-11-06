using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Play")]
    [SerializeField] private GameObject playMenu;
    [SerializeField] private Button resumeGameBtn;
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button playBackBtn;
    [SerializeField] private Image stageImg;
    [SerializeField] private TMP_Text stageNameTxt;
    [SerializeField] private TMP_Text stageDescriptionTxt;

    [Header("New Game Prompt")]
    [SerializeField] private GameObject newGamePrompt;
    [SerializeField] private Button confirmNewGameButton;
    [SerializeField] private Button cancelNewGameButton;

    [Header("Stage Data")]
    [SerializeField] private List<StageData> stageData;

    [Header("Settings")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject keybindingOverlay;
    [SerializeField] private GameObject duplicatePrompt;
    [SerializeField] private GameObject defaultPrompt;
    [SerializeField] private Button cancelDefaultButton;
    [SerializeField] private Button settingsBackButton;

    private int stageIndex;

    UserInput input;
    PlayerInput playerInput;
    InputDeviceDetector inputDetector;

    SetsUIElementToSelectOnInteraction playMenuSelectUI;
    public bool isSecondarySelected;

    private void Awake()
    {
        playButton.onClick.AddListener(OpenPlayMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        quitButton.onClick.AddListener(Quit); 
        newGameBtn.onClick.AddListener(NewGame);
        confirmNewGameButton.onClick.AddListener(StartNewGame);
        cancelNewGameButton.onClick.AddListener(CancelNewGame);
        resumeGameBtn.onClick.AddListener(LoadStage);
        playBackBtn.onClick.AddListener(OpenMainMenu);
        settingsBackButton.onClick.AddListener(OpenMainMenu);
        cancelDefaultButton.onClick.AddListener(CancelDefault);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputDetector = InputDeviceDetector.Instance;
        input = UserInput.instance;
        playMenuSelectUI = playMenu.GetComponent<SetsUIElementToSelectOnInteraction>();
        
        playerInput = FindAnyObjectByType<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");

        isSecondarySelected = false;

        OpenMainMenu();
    }

    private void Update()
    {
        if (input.EscapeInput)
        {
            if (keybindingOverlay.activeInHierarchy) return;
            if (duplicatePrompt.activeInHierarchy)
            {
                Debug.Log("TitleScreenManager: Duplicate prompt is open, do nothing");
                return;
            }

            if (defaultPrompt.activeInHierarchy)
            {
                defaultPrompt.SetActive(false);
                Debug.Log("TitleScreenManager closed default prompt: Update");
                return;
            }

            if (newGamePrompt.activeInHierarchy)
            {
                CancelNewGame();
                return;
            }

            if (playMenuSelectUI.GetSelectedUI() == resumeGameBtn.gameObject || playMenuSelectUI.GetSelectedUI() == newGameBtn.gameObject)
            {
                playMenuSelectUI.ReturnToPreviousSelection();
                return;
            }

            OpenMainMenu();
        }
    }

    public void OpenMainMenu()
    {
        if (isSecondarySelected) return;

        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Debug.Log("TitleScreenManager: Opened Main Menu");
    }

    public void OpenPlayMenu()
    {
        if (isSecondarySelected) return;

        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        settingsMenu.SetActive(false);

        //default stage is 1;
        InitializeStage();
    }

    public void OpenSettingsMenu()
    {
        if (isSecondarySelected) return;

        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    private void InitializeStage()
    {
        stageIndex = 1;
        stageImg.sprite = stageData[0].stageSprite;
        stageNameTxt.text = stageData[0].stageName;
        stageDescriptionTxt.text = stageData[0].stageDescription;

        resumeGameBtn.gameObject.SetActive(stageData[0].hasSavedData);
    }

    public void SetStage(int index)
    {
        // Stage information
        stageIndex = index;
        stageImg.sprite = stageData[index - 1].stageSprite;
        stageNameTxt.text = stageData[index - 1].stageName;
        stageDescriptionTxt.text = stageData[index - 1].stageDescription;

        resumeGameBtn.gameObject.SetActive(stageData[index - 1].hasSavedData);

        Button nextButton = stageData[index - 1].hasSavedData ? resumeGameBtn : newGameBtn;
        playMenuSelectUI.ClearSelectionHistory();
        playMenuSelectUI.SelectAndRemember(nextButton.gameObject);
    }

    public void NewGame()
    {
        if (resumeGameBtn.gameObject.activeInHierarchy)
        {
            newGamePrompt.SetActive(true);
            if (inputDetector.CurrentMode == InputDeviceDetector.InputMode.Gamepad)
            {
                playMenuSelectUI.SelectAndRemember(cancelNewGameButton.gameObject);
            }
            return;
        }

        if (string.IsNullOrEmpty(SceneUtility.GetScenePathByBuildIndex(stageIndex)))
        {
            return;
        }

        stageData[stageIndex - 1].hasSavedData = true;
        StartNewGame();
    }

    public void StartNewGame()
    {
        playMenuSelectUI.ClearSelectionHistory();
        playerInput.SwitchCurrentActionMap("Player");
        SaveManager.instance.SaveGame();
        SceneLoader.Instance.LoadSceneByIndex(stageIndex);
    }

    public void CancelNewGame()
    {
        if (inputDetector.CurrentMode == InputDeviceDetector.InputMode.Gamepad)
        {
            playMenuSelectUI.ReturnToPreviousSelection();
        }

        newGamePrompt.SetActive(false);
    }

    public void CancelDefault()
    {
        defaultPrompt.SetActive(false);
        Debug.Log("TitleScreenManager closed default prompt: CancelDefault");
    }

    public void LoadStage()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader.Instance is NULL!");
            return;
        }

        playerInput.SwitchCurrentActionMap("Player");
        SaveManager.instance.LoadGame(stageIndex);
    }

    public void Quit()
    {
        SceneLoader.Instance.QuitGame();
    }
}

[Serializable]
public class StageData
{
    public Button stageButton;
    public Sprite stageSprite;
    public string stageName;
    public string stageDescription;
    public bool hasSavedData;
}