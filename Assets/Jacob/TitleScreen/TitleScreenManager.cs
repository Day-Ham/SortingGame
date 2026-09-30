using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("First Selected Option")]
    [SerializeField] private GameObject mainFirstSelected;
    [SerializeField] private GameObject playFirstSelected;
    [SerializeField] private GameObject settingsFirstSelected;

    [Header("Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Play Menu Buttons")]
    [SerializeField] private Button stage1ResumeGameBtn;
    [SerializeField] private Button stage1NewGameBtn;
    [SerializeField] private Button stage1ResumeTimedChallengeBtn;
    [SerializeField] private Button stage1TimedChallengeBtn;
    [SerializeField] private Button stage2ResumeGameBtn;
    [SerializeField] private Button stage2NewGameBtn;
    [SerializeField] private Button stage2ResumeTimedChallengeBtn;
    [SerializeField] private Button stage2TimedChallengeBtn;
    [SerializeField] private Button stage3ResumeGameBtn;
    [SerializeField] private Button stage3NewGameBtn;
    [SerializeField] private Button stage3ResumeTimedChallengeBtn;
    [SerializeField] private Button stage3TimedChallengeBtn;

    SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = SceneLoader.Instance;

        playButton.onClick.AddListener(OpenPlayMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        quitButton.onClick.AddListener(sceneLoader.QuitGame);

        //stage 1
        //stage1ResumeGameBtn.onClick.AddListener();
        stage1NewGameBtn.onClick.AddListener(sceneLoader.LoadGameScene);
        //stage1ResumeTimedChallengeBtn.onClick.AddListener();
        //stage1TimedChallengeBtn.onClick.AddListener();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(mainFirstSelected);
    }

    public void OpenPlayMenu()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        settingsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(playFirstSelected);
    }

    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        settingsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(settingsFirstSelected);
    }
}
