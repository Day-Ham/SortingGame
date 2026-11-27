using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    //Checks
    [SerializeField] private List<GameObject> menus = new List<GameObject>();
    public bool isPauseMenu;
    public bool isSettingsMenu;

    public GameObject currentOpenMenu;
    private GameObject previousOpenMenu;
    private GameObject closingMenu;

    PlayerInput playerInput;
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
        playerInput = FindAnyObjectByType<PlayerInput>();
        
        input = UserInput.instance;
        pause = PauseManager.instance;

        AddMenusAndPromptsToLists();

        foreach (GameObject menu in menus)
        {
            if (menu.activeInHierarchy)
            {
                menu.SetActive(false);
            }
        }
    }

    private void Update()
    {
        //if (input.EscapeInput && !isSecondarySelected)
        //{
        //    if (keybindingOverlay.activeInHierarchy) return;

        //    if (currentOpenMenu == null)
        //    {
        //        SetAsCurrentOpenMenu(pauseMenu);
        //        return;
        //    }

        //    if (currentOpenMenu == settingsMenu)
        //    {
        //        if (duplicatePrompt.activeInHierarchy) return;
        //        if (defaultPrompt.activeInHierarchy)
        //        {
        //            TweenClose(defaultPrompt);
        //            return;
        //        }

        //        ReturnFromSettingsToPause();
        //        return;
        //    }

        //    CloseMenu(currentOpenMenu);
        //    return;
        //}
    }

    #region Menus
    private void AddMenusAndPromptsToLists()
    {
        int menuLayer = LayerMask.NameToLayer("Menu");
        GameObject[] menuUI = FindObjectsByType<GameObject>(FindObjectsInactive.Include)
            .Where(obj => obj.layer == menuLayer)
            .ToArray();
        foreach (GameObject menu in menuUI)
        {
            menus.Add(menu);
        }
    }

    private void DisableInactiveMenus()
    {
        if (currentOpenMenu == null) return;

        foreach (GameObject menu in menus)
        {
            if (menu != currentOpenMenu && menu != closingMenu)
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

        if (isSettingsMenu)
        {
            previousOpenMenu = currentOpenMenu;
        }

        currentOpenMenu = menu;
        TweenOpen(currentOpenMenu);

        if (isPauseMenu)
        {
            pause.PauseGame();
            isPauseMenu = false;
            return;
        }

        pause.DisablePlayerControl();
        pause.UnlockCursor();

        if (isSettingsMenu) 
        {
            isSettingsMenu = false;
            return;
        }

        DisableInactiveMenus();
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
                menu.transform.localScale = Vector3.one;
                menu.SetActive(false);

                if (isSettingsMenu)
                {
                    currentOpenMenu = previousOpenMenu;
                    previousOpenMenu = null;
                }
                else
                {
                    currentOpenMenu = null;
                }

                FinishCloseMenu(menu);
            });
    }
    private void FinishCloseMenu(GameObject menu)
    {
        if (isSettingsMenu)
        {
            isSettingsMenu = false;
            return;
        }

        playerInput.SwitchCurrentActionMap("Player");

        if (isPauseMenu && !isSettingsMenu)
        {
            pause.ResumeGame();
            isPauseMenu = false;
            return;
        }

        pause.EnablePlayerControl();
        pause.LockCursor();
    }

    public void TweenClose(GameObject menu)
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
    #endregion

    
}