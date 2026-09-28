using UnityEngine;
using UnityEngine.EventSystems;

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
