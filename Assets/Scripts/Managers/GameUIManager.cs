using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [Header("Player HUD")]
    [Header("Crosshair")]
    [SerializeField] private Image crosshairImage;
    public Sprite normalCrosshair;
    public Sprite oCrosshair;
    public Sprite xCrosshair;

    [Header("Looked at Object")]
    public TMP_Text lookedAtItemNameText;

    [Header("Held Item")]
    public TMP_Text amountHeldText;
    public TMP_Text maxHeldItemsText;
    public TMP_Text heldItemNameText;
    public TMP_Text[] backpackItemNames;

    [Header("Upgrades")]
    [Header("Upgrades Menu")]
    [SerializeField] private Button upgradesCloseButton;
    public GameObject upgradesMenu;

    [Header("Shelved Count")]
    [SerializeField] private TMP_Text shelvedItemText;
    private int shelvedItemCount;
    [SerializeField] private TMP_Text maxItemText;
    private int maxItemCount;

    [Header("Upgrades Buy Sliders")]
    public Slider upgradeCapacitySlider;
    public Slider upgradeSprintSlider;
    public Slider upgradeReachSlider;
    public Slider upgradeJumpSlider;
    public Slider upgradeThrowSlider;
    public Slider upgradeRobotBuddySlider;

    [Header("Skills")]
    [Header("Skills Menu")]
    [SerializeField] private Button skillsCloseButton;

    public GameObject skillsMenu;

    [Header("Skills Buy Sliders")]
    public Slider skill1Slider;
    public Slider skill2Slider;
    public Slider skill3Slider;

    [Header("Skill 1")]
    public GameObject skill1Icon;
    public Slider skill1CooldownSlider;
    
    [Header("Skill 2")]
    public GameObject skill2Icon;
    public Slider skill2CooldownSlider;
    
    [Header("Skill 3")]
    public GameObject skill3Icon;
    public Slider skill3CooldownSlider;

    [Header("Menu")]
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnToTitleScreenButton;
    [SerializeField] private Button quitButton;

    [Header("Settings Menu")]
    public GameObject settingsMenu;
    [SerializeField] private GameObject duplicatePrompt;
    [SerializeField] private GameObject defaultPrompt;
    [SerializeField] private Button settingsBackButton;


    //Checks
    [SerializeField] private List<GameObject> menus = new List<GameObject>();

    [SerializeField] private GameObject currentOpenMenu;
    private GameObject closingMenu;

    PlayerInput playerInput;
    UserInput input;
    PauseManager pause;
    SceneLoader sceneLoader;    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerInput = FindAnyObjectByType<PlayerInput>();
        input = UserInput.instance;
        pause = PauseManager.instance;
        sceneLoader = SceneLoader.Instance;

        resumeButton.onClick.AddListener(ResumeButtonAction);
        settingsButton.onClick.AddListener(SetSettingsMenuAsCurrent);
        returnToTitleScreenButton.onClick.AddListener(LoadTitleScreen);
        quitButton.onClick.AddListener(QuitGame);
        settingsBackButton.onClick.AddListener(SetPauseMenuAsCurrent);
        skillsCloseButton.onClick.AddListener(CloseSkills);
        upgradesCloseButton.onClick.AddListener(CloseUpgrades);

        AddMenusAndPromptsToLists();

        foreach (GameObject menu in menus)
        {
            if (menu.activeInHierarchy)
            {
                DisableUI(menu);
            }
        }
    }

    private void Update()
    {
        DisableInactiveMenus();

        if (input.UpgradeInput && !pause.isPaused)
        {
            if (currentOpenMenu == upgradesMenu)
            {
                CloseMenu(currentOpenMenu);
                return;
            }

            SetAsCurrentOpenMenu(upgradesMenu);
        }

        if (input.SkillInput && !pause.isPaused)
        {
            if (currentOpenMenu == skillsMenu)
            {
                CloseMenu(currentOpenMenu);
                return;
            }

            SetAsCurrentOpenMenu(skillsMenu);
        }

        if (input.EscapeInput)
        {
            if (currentOpenMenu == null)
            {
                SetAsCurrentOpenMenu(pauseMenu);
                return;
            }

            if (currentOpenMenu == settingsMenu)
            {
                if (duplicatePrompt.activeInHierarchy) return;
                if (defaultPrompt.activeInHierarchy)
                {
                    DisableUI(defaultPrompt);
                    return;
                }

                ReturnFromSettingsToPause();
                return;
            }

            CloseMenu(currentOpenMenu);
            return;
        }
    }

    #region Set UIs
    public void SetCrosshair(Sprite sprite)
    {
        crosshairImage.sprite = sprite;
    }

    public void SetUIText(TMP_Text tmpText, string text)
    {
        tmpText.text = text;
    }

    public void FillSlider(Slider slider, int maxValue)
    {
        slider.value += 1f / maxValue;
    }

    public void UpdateTimerSlider(Slider slider, float cooldownTime, float cooldownTimer)
    {
        slider.value = 1f - (cooldownTimer / cooldownTime);
    }

    public void EnableUI(GameObject UI)
    {
        UI.SetActive(true);
    }

    public void DisableUI(GameObject UI)
    {
        UI.SetActive(false);
    }
    #endregion

    #region Menus
    private void AddMenusAndPromptsToLists()
    {
        int menuLayer = LayerMask.NameToLayer("Menu");
        GameObject[] menuUI = FindObjectsByType<GameObject>(FindObjectsInactive.Include)
            .Where(obj => obj.layer == menuLayer)
            .ToArray();
        foreach(GameObject menu in menuUI)
        {
            menus.Add(menu);
        }
    }

    private void DisableInactiveMenus()
    {
        if (currentOpenMenu == null) return;

        foreach (GameObject menu in menus)
        {
            if(menu != currentOpenMenu && menu != closingMenu)
            {
                DisableUI(menu);
            }
        }
    }

    public void SetAsCurrentOpenMenu(GameObject menu)
    {
        if (closingMenu != null)
        {
            closingMenu.transform.DOKill();
            closingMenu.transform.localScale = Vector3.one;
            if (closingMenu != menu)
                DisableUI(closingMenu);
            closingMenu = null;
        }

        playerInput.SwitchCurrentActionMap("UI");

        currentOpenMenu = menu;
        EnableUI(currentOpenMenu);
        TweenMenuOpen(currentOpenMenu);

        if (currentOpenMenu == pauseMenu || currentOpenMenu == settingsMenu)
        {
            pause.PauseGame();
            return;
        }

        pause.DisablePlayerControl();
        pause.UnlockCursor();
    }

    private void TweenMenuOpen(GameObject menu)
    {
        if (menu != pauseMenu && menu != upgradesMenu && menu != skillsMenu && menu != settingsMenu)
            return;

        Transform menuTransform = menu.transform;
        menuTransform.DOKill();
        menuTransform.localScale = Vector3.zero;
        menuTransform.DOScale(Vector3.one, 0.25f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void CloseMenu(GameObject menu)
    {
        if (menu == null || menu == closingMenu)
            return;

        bool tweenClose = menu == pauseMenu || menu == upgradesMenu || menu == skillsMenu || menu == settingsMenu;
        if (!tweenClose)
        {
            currentOpenMenu = null;
            DisableUI(menu);
            FinishCloseMenu(menu);
            return;
        }

        closingMenu = menu;
        menu.transform.DOKill();
        menu.transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (closingMenu != menu)
                    return;

                closingMenu = null;
                currentOpenMenu = null;
                menu.transform.localScale = Vector3.one;
                DisableUI(menu);
                FinishCloseMenu(menu);
            });
    }

    private void FinishCloseMenu(GameObject menu)
    {
        playerInput.SwitchCurrentActionMap("Player");

        if (menu == pauseMenu || menu == settingsMenu)
        {
            pause.ResumeGame();
            return;
        }

        pause.EnablePlayerControl();
        pause.LockCursor();
    }

    private void SetSettingsMenuAsCurrent()
    {
        SetAsCurrentOpenMenu(settingsMenu);
    }

    private void SetPauseMenuAsCurrent()
    {
        ReturnFromSettingsToPause();
    }

    public void ReturnFromSettingsToPause()
    {
        SetAsCurrentOpenMenu(pauseMenu);
        ShrinkAway(settingsMenu);
    }

    private void ShrinkAway(GameObject menu)
    {
        if (menu == null)
            return;

        closingMenu = menu;
        menu.transform.DOKill();
        menu.transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (closingMenu != menu)
                    return;

                closingMenu = null;
                menu.transform.localScale = Vector3.one;
                DisableUI(menu);
            });
    }
    private void CloseUpgrades()
    {
        CloseMenu(upgradesMenu);
    }
    private void CloseSkills()
    {
        CloseMenu(skillsMenu);
    }
    #endregion

    private void ResumeButtonAction()
    {
        CloseMenu(pauseMenu);
    }

    private void LoadTitleScreen()
    {
        ResumeButtonAction();
        pause.DisablePlayerControl();
        pause.UnlockCursor();
        SaveManager.instance.SaveGame();
        sceneLoader.LoadSceneByIndex(0);
    }

    private void QuitGame()
    {
        ResumeButtonAction();
        pause.DisablePlayerControl();
        pause.UnlockCursor();
        SaveManager.instance.SaveGame();
        sceneLoader.QuitGame();
    }

    public void UpdateShelvedCount(int count)
    {
        shelvedItemCount += count;
        shelvedItemText.text = shelvedItemCount.ToString("F0"); 
    }

    public void SetMaxNumberItems(int maxCount)
    {
        maxItemCount = maxCount;
        maxItemText.text = maxItemCount.ToString("F0");
    }
}