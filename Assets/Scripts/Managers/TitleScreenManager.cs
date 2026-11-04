using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    [SerializeField] private Button stage1Btn;
    [SerializeField] private Button stage2Btn;
    [SerializeField] private Button stage3Btn;
    [SerializeField] private Button resumeGameBtn;
    [SerializeField] private Button newGameBtn;
    [SerializeField] private GameObject newGamePrompt;
    [SerializeField] private Button confirmNewGameButton;
    [SerializeField] private Button cancelNewGameButton;
    [SerializeField] private Button playBackBtn;
    [SerializeField] private Image stageImg;
    [SerializeField] private Sprite stage1Sprite;
    [SerializeField] private Sprite stage2Sprite;
    [SerializeField] private Sprite stage3Sprite;
    [SerializeField] private TMP_Text stageNameTxt;
    [SerializeField] private string stage1Name;
    [SerializeField] private string stage2Name;
    [SerializeField] private string stage3Name;
    [SerializeField] private TMP_Text stageDescriptionTxt;

    [SerializeField] private string stage1Description;
    [SerializeField] private string stage2Description;
    [SerializeField] private string stage3Description;

    [Header("Settings")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject duplicatePrompt;
    [SerializeField] private GameObject defaultPrompt;
    [SerializeField] private Button settingsBackButton;


    private int stageIndex;
    [SerializeField] private bool stage1NewGameCreated;
    [SerializeField] private bool stage2NewGameCreated;
    [SerializeField] private bool stage3NewGameCreated;

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
        stage1Btn.onClick.AddListener(SetStage1);
        stage2Btn.onClick.AddListener(SetStage2);
        stage3Btn.onClick.AddListener(SetStage3);
        newGameBtn.onClick.AddListener(NewGame);
        confirmNewGameButton.onClick.AddListener(StartNewGame);
        cancelNewGameButton.onClick.AddListener(CancelNewGame);
        resumeGameBtn.onClick.AddListener(LoadStage);
        playBackBtn.onClick.AddListener(OpenMainMenu);
        settingsBackButton.onClick.AddListener(OpenMainMenu);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputDetector = InputDeviceDetector.Instance;
        input = UserInput.instance;
        OpenMainMenu();
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);

        playerInput = FindAnyObjectByType<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");

        isSecondarySelected = false;

        playMenuSelectUI = playMenu.GetComponent<SetsUIElementToSelectOnInteraction>();
    }

    private void Update()
    {
        if (input.EscapeInput)
        {
            if (duplicatePrompt.activeInHierarchy) return;
            if (defaultPrompt.activeInHierarchy)
            {
                defaultPrompt.SetActive(false);
                return;
            }

            if (newGamePrompt.activeInHierarchy)
            {
                CancelNewGame();
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
    }

    public void OpenPlayMenu()
    {
        if (isSecondarySelected) return;

        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        settingsMenu.SetActive(false);

        //default stage is 1;
        SetStage1();
    }

    public void OpenSettingsMenu()
    {
        if (isSecondarySelected) return;

        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void SetStage1()
    {
        stageIndex = 1;
        stageImg.sprite = stage1Sprite;
        stageNameTxt.text = stage1Name;
        stageDescriptionTxt.text = stage1Description;
        resumeGameBtn.gameObject.SetActive(stage1NewGameCreated);
        newGamePrompt.gameObject.SetActive(false);

        Image image1 = stage1Btn.targetGraphic as Image;
        if (image1 != null)
        {
            SpriteState state = stage1Btn.spriteState;
            image1.sprite = state.pressedSprite;
        }

        Image image2 = stage2Btn.targetGraphic as Image;
        if (image2 != null)
        {
            SpriteState state = stage2Btn.spriteState;
            image2.sprite = playMenuSelectUI.normalSprites[stage2Btn];
        }

        Image image3 = stage3Btn.targetGraphic as Image;
        if (image3 != null)
        {
            SpriteState state = stage3Btn.spriteState;
            image3.sprite = playMenuSelectUI.normalSprites[stage3Btn];
        }

        stage1Btn.transform.GetChild(0).gameObject.SetActive(true);
        stage2Btn.transform.GetChild(0).gameObject.SetActive(false);
        stage3Btn.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetStage2()
    {
        stageIndex = 2;
        stageImg.sprite = stage2Sprite;
        stageNameTxt.text = stage2Name;
        stageDescriptionTxt.text = stage2Description;
        resumeGameBtn.gameObject.SetActive(stage2NewGameCreated);
        newGamePrompt.gameObject.SetActive(false);

        Image image1 = stage1Btn.targetGraphic as Image;
        if (image1 != null)
        {
            SpriteState state = stage1Btn.spriteState;
            image1.sprite = playMenuSelectUI.normalSprites[stage1Btn];
        }

        Image image2 = stage2Btn.targetGraphic as Image;
        if (image2 != null)
        {
            SpriteState state = stage2Btn.spriteState;
            image2.sprite = state.pressedSprite;
        }

        Image image3 = stage3Btn.targetGraphic as Image;
        if (image3 != null)
        {
            SpriteState state = stage3Btn.spriteState;
            image3.sprite = playMenuSelectUI.normalSprites[stage3Btn];
        }

        stage1Btn.transform.GetChild(0).gameObject.SetActive(false);
        stage2Btn.transform.GetChild(0).gameObject.SetActive(true);
        stage3Btn.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetStage3()
    {
        stageIndex = 3;
        stageImg.sprite = stage3Sprite;
        stageNameTxt.text = stage3Name;
        stageDescriptionTxt.text = stage3Description;
        resumeGameBtn.gameObject.SetActive(stage3NewGameCreated);
        newGamePrompt.gameObject.SetActive(false);

        Image image1 = stage1Btn.targetGraphic as Image;
        if (image1 != null)
        {
            SpriteState state = stage1Btn.spriteState;
            image1.sprite = playMenuSelectUI.normalSprites[stage1Btn];
        }

        Image image2 = stage2Btn.targetGraphic as Image;
        if (image2 != null)
        {
            SpriteState state = stage2Btn.spriteState;
            image2.sprite = playMenuSelectUI.normalSprites[stage2Btn];
        }

        Image image3 = stage3Btn.targetGraphic as Image;
        if (image3 != null)
        {
            SpriteState state = stage3Btn.spriteState;
            image3.sprite = state.pressedSprite;
        }

        stage1Btn.transform.GetChild(0).gameObject.SetActive(false);
        stage2Btn.transform.GetChild(0).gameObject.SetActive(false);
        stage3Btn.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void NewGame()
    {
        if (resumeGameBtn.gameObject.activeInHierarchy)
        {
            newGamePrompt.SetActive(true);
            return;
        }

        switch (stageIndex)
        {
            case 1:
                stage1NewGameCreated = true;
                break;

            case 2:
                stage2NewGameCreated = true;
                break;

            case 3:
                stage3NewGameCreated = true;
                break;
        }

        if (inputDetector.CurrentMode == InputDeviceDetector.InputMode.Gamepad)
        {
            if (playMenuSelectUI != null)
            {
                playMenuSelectUI.ReturnToParent();
            }
        }
        

        StartNewGame();
    }

    public void StartNewGame()
    {
        SaveManager.instance.SaveGame();
        SceneLoader.Instance.LoadSceneByIndex(stageIndex);
    }

    public void CancelNewGame()
    {
        newGamePrompt.SetActive(false);

        if (inputDetector.CurrentMode == InputDeviceDetector.InputMode.Gamepad)
        {
            if (playMenuSelectUI == null) return;

            playMenuSelectUI.SelectSecondaryElement();
        }
    }

    public void LoadStage()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader.Instance is NULL!");
            return;
        }

        SaveManager.instance.LoadGame(stageIndex);
    }

    public void Quit()
    {
        SceneLoader.Instance.QuitGame();
    }
}
