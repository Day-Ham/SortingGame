using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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

    [Header("Shelved Count")]
    [SerializeField] private TMP_Text shelvedItemText;
    private int shelvedItemCount;
    [SerializeField] private TMP_Text maxItemText;
    private int maxItemCount;

    [Header("Skills Menu")]
    public GameObject skillsMenu;
    [SerializeField] private Button skillsCloseButton;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnToTitleScreenButton;
    [SerializeField] private Button quitButton;

    [Header("Settings Menu")]
    public GameObject settingsMenu;
    [SerializeField] private GameObject keybindingOverlay;
    [SerializeField] private GameObject duplicatePrompt;
    [SerializeField] private GameObject defaultPrompt;
    [SerializeField] private Button cancelDefaultButton;
    [SerializeField] private Button settingsBackButton;


    //Checks
    [SerializeField] private List<GameObject> menus = new List<GameObject>();

    public GameObject currentOpenMenu;
    private GameObject closingMenu;
    public bool isSecondarySelected;

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

        resumeButton.onClick.AddListener(() => CloseMenu(pauseMenu));
        settingsButton.onClick.AddListener(()=> SetAsCurrentOpenMenu(settingsMenu));
        returnToTitleScreenButton.onClick.AddListener(LoadTitleScreen);
        quitButton.onClick.AddListener(QuitGame);
        settingsBackButton.onClick.AddListener(ReturnFromSettingsToPause);
        skillsCloseButton.onClick.AddListener(() => CloseMenu(skillsMenu));
        cancelDefaultButton.onClick.AddListener(() => ClosePrompt(defaultPrompt));

        AddMenusAndPromptsToLists();

        foreach (GameObject menu in menus)
        {
            if (menu.activeInHierarchy)
            {
                menu.SetActive(false);
            }
        }

        isSecondarySelected = false;
    }

    private void Update()
    {
        if (input.EscapeInput && !isSecondarySelected)
        {
            if (keybindingOverlay.activeInHierarchy) return;

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
                    ClosePrompt(defaultPrompt);
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
                menu.SetActive(false);
            }
        }
    }

    public void SetAsCurrentOpenMenu(GameObject menu)
    {
        playerInput.SwitchCurrentActionMap("UI");

        if (closingMenu != null)
        {
            closingMenu.transform.DOKill();
            closingMenu.transform.localScale = Vector3.one;
            if (closingMenu != menu)
                closingMenu.SetActive(false);
            closingMenu = null;
        }

        currentOpenMenu = menu;
        TweenOpen(currentOpenMenu);

        if (currentOpenMenu == pauseMenu || currentOpenMenu == settingsMenu)
        {
            pause.PauseGame();
            return;
        }

        DisableInactiveMenus();

        pause.DisablePlayerControl();
        pause.UnlockCursor();
    }

    public void TweenOpen(GameObject menu)
    {
        menu.SetActive(true);
        Transform menuTransform = menu.transform;
        menuTransform.DOKill();
        menuTransform.localScale = Vector3.zero;
        menuTransform.DOScale(Vector3.one, 0.25f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void CloseMenu(GameObject menu)
    {
        if (menu == null || menu == closingMenu) return;

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
                menu.SetActive(false);
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

    public void ClosePrompt(GameObject menu)
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
                menu.SetActive(false);
            });
    }

    public void ReturnFromSettingsToPause()
    {
        SetAsCurrentOpenMenu(pauseMenu);
        ClosePrompt(settingsMenu);
    }

    
    #endregion

    private void LoadTitleScreen()
    {
        CloseMenu(pauseMenu);
        pause.DisablePlayerControl();
        pause.UnlockCursor();
        SaveManager.instance.SaveGame();
        sceneLoader.LoadSceneByIndex(0);
    }

    private void QuitGame()
    {
        CloseMenu(pauseMenu);
        pause.DisablePlayerControl();
        pause.UnlockCursor();
        SaveManager.instance.SaveGame();
        sceneLoader.QuitGame();
    }

    public void UpdateShelvedCount(int count)
    {
        shelvedItemCount += count;
        shelvedItemText.text = shelvedItemCount.ToString("F0");

        if (shelvedItemCount == maxItemCount)
        {
            SteamManager.instance.CompleteStage(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SetMaxNumberItems(int maxCount)
    {
        maxItemCount = maxCount;
        maxItemText.text = maxItemCount.ToString("F0");
    }
}