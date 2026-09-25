using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

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
    public GameObject upgradesMenu;

    [Header("Upgrades Buy Sliders")]
    public Slider upgradeCapacitySlider;
    public Slider upgradeSprintSlider;
    public Slider upgradeReachSlider;
    public Slider upgradeJumpSlider;

    [Header("Skills")]
    [Header("Skills Menu")]
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
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnToTitleScreenButton;
    [SerializeField] private Button quitButton;

    [Header("Settings Menu")]
    [SerializeField] private GameObject settingsMenu;

    //Checks
    [SerializeField] private List<GameObject> menus = new List<GameObject>();
    [SerializeField] private GameObject currentOpenMenu;

    UserInput input;
    PauseManager pause;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        input = UserInput.instance;
        pause = PauseManager.instance;

        settingsButton.onClick.AddListener(SetSettingsMenuAsCurrent);

        AddMenusToList();

        DisableUI(pauseMenu);
        DisableUI(settingsMenu);
        DisableUI(upgradesMenu);
        DisableUI(skillsMenu);
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
                SetAsCurrentOpenMenu(pauseMenu);
                DisableUI(settingsMenu);
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
    private void AddMenusToList()
    {
        menus.Add(pauseMenu);
        menus.Add(settingsMenu);
        menus.Add(upgradesMenu);
        menus.Add(skillsMenu);
    }

    private void DisableInactiveMenus()
    {
        if (currentOpenMenu == null) return;

        foreach (GameObject menu in menus)
        {
            if(menu != currentOpenMenu)
            {
                DisableUI(menu);
            }
        }
    }

    private void SetAsCurrentOpenMenu(GameObject menu)
    {
        currentOpenMenu = menu;
        EnableUI(currentOpenMenu);

        if (currentOpenMenu == pauseMenu || currentOpenMenu == settingsMenu)
        {
            pause.PauseGame();
            return;
        }

        pause.DisablePlayerControl();
    }

    private void CloseMenu(GameObject menu)
    {
        currentOpenMenu = null;
        DisableUI(menu);

        pause.ResumeGame();
    }

    private void SetSettingsMenuAsCurrent()
    {
        SetAsCurrentOpenMenu(settingsMenu);
    }
    #endregion
}