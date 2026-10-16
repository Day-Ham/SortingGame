using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private List<GameObject> prompts = new List<GameObject>();


    private int stageIndex;
    [SerializeField] private bool stage1NewGameCreated;
    [SerializeField] private bool stage2NewGameCreated;
    [SerializeField] private bool stage3NewGameCreated;

    UserInput input;

    private void Awake()
    {
        playButton.onClick.AddListener(OpenPlayMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        quitButton.onClick.AddListener(Quit); 
        stage1Btn.onClick.AddListener(SetStage1);
        stage2Btn.onClick.AddListener(SetStage2);
        stage3Btn.onClick.AddListener(SetStage3);
        newGameBtn.onClick.AddListener(NewGame);
        resumeGameBtn.onClick.AddListener(LoadStage);
        playBackBtn.onClick.AddListener(OpenMainMenu);
        settingsBackButton.onClick.AddListener(OpenMainMenu);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = UserInput.instance;
        OpenMainMenu();
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        AddPromptsToLists();
    }

    private void Update()
    {
        if (input.EscapeInput)
        {
            if (settingsMenu.activeInHierarchy)
            {
                foreach (GameObject prompt in prompts)
                {
                    if (prompt.activeInHierarchy)
                    {
                        prompt.SetActive(false);
                        return;
                    }
                }
            }
            OpenMainMenu();
        }
    }

    private void AddPromptsToLists()
    {
        int promptLayer = LayerMask.NameToLayer("Prompt");
        GameObject[] promptUI = FindObjectsByType<GameObject>(FindObjectsInactive.Include)
            .Where(obj => obj.layer == promptLayer)
            .ToArray();
        foreach (GameObject prompt in promptUI)
        {
            prompts.Add(prompt);
        }
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);       
    }

    public void OpenPlayMenu()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        settingsMenu.SetActive(false);

        //default stage is 1;
        SetStage1();
    }

    public void OpenSettingsMenu()
    {
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
    }

    public void SetStage2()
    {
        stageIndex = 2;
        stageImg.sprite = stage2Sprite;
        stageNameTxt.text = stage2Name;
        stageDescriptionTxt.text = stage2Description;
        resumeGameBtn.gameObject.SetActive(stage2NewGameCreated);
    }

    public void SetStage3()
    {
        stageIndex = 3;
        stageImg.sprite = stage3Sprite;
        stageNameTxt.text = stage3Name;
        stageDescriptionTxt.text = stage3Description;
        resumeGameBtn.gameObject.SetActive(stage3NewGameCreated);
    }

    public void NewGame()
    {
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
        

        SaveManager.instance.SaveGame();
        SceneLoader.Instance.LoadSceneByIndex(stageIndex);
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
