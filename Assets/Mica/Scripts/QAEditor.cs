using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QAEditor : MonoBehaviour
{
    [SerializeField] private GameObject editorPanel;
    private bool openPanel;

    [Header("Currency")]
    [SerializeField] private TMP_InputField skillPointsInputField;
    [SerializeField] private int skillPoints;
    [SerializeField] private TMP_InputField upgradePointsInputField;
    [SerializeField] private int upgradePoints;
    [SerializeField] private Button applyPointsButton;
    [SerializeField] private Button resetPointsButton;

    [Header("Skills")]
    [SerializeField] private Button maxLevelSkills;
    [SerializeField] private Button resetLevelSkills;

    [Header("Upgrades")]
    [SerializeField] private Button maxLevelUpgrades;
    [SerializeField] private Button resetLevelUpgrades;
    [SerializeField] private Toggle includeRobotBuddy;
    [SerializeField] private Toggle includeThrowUpgrade;

    [Header("Scene")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadSaveGameButton;

    [Header("Player")]
    [SerializeField] private GameObject player;
    private PlayerSkills skills;
    private PlayerUpgrade upgrades;

    [Header("Other Scripts")]
    [SerializeField] private CurrencyManager currency;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skills = player.GetComponent<PlayerSkills>();
        upgrades = player.GetComponent<PlayerUpgrade>();

        openPanel = false;
        editorPanel.SetActive(false);

        applyPointsButton.onClick.AddListener(ApplyPoints);
        resetPointsButton.onClick.AddListener(ResetPoints);

        maxLevelSkills.onClick.AddListener(MaxLevelSkills);
        resetLevelSkills.onClick.AddListener(ResetLevelSkills);

        maxLevelUpgrades.onClick.AddListener(MaxLevelUpgrades);
        resetLevelUpgrades.onClick.AddListener(ResetLevelUpgrades);

        newGameButton.onClick.AddListener(NewGame);
        loadSaveGameButton.onClick.AddListener(LoadStage);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Debug.Log("F1 pressed!");

            openPanel = !openPanel;
            OpenEditorPanel(openPanel);
        }
    }

    private void OpenEditorPanel(bool openPanel)
    {
        editorPanel.SetActive(openPanel);
        if (openPanel)
        {
            PauseManager.instance.PauseGame();
        }
        else
        {
            PauseManager.instance.ResumeGame();
        }
    }

    private void ApplyPoints()
    {
        if (int.TryParse(skillPointsInputField.text, out int skillValue) && int.TryParse(upgradePointsInputField.text, out int upgradeValue))
        {
            skillPoints = skillValue;
            upgradePoints = upgradeValue;
        }
        else
        {
            if (!int.TryParse(skillPointsInputField.text, out _))
            {
                skillPointsInputField.placeholder.GetComponent<TMP_Text>().text = "Enter integer...";
            }

            if (!int.TryParse(upgradePointsInputField.text, out _))
            {
                upgradePointsInputField.placeholder.GetComponent<TMP_Text>().text = "Enter integer...";
            }
        }

        currency.skillPoints = skillPoints;
        currency.upgradePoints = upgradePoints;

        currency.UpdateUI(); // return to private on final build
    }

    private void ResetPoints()
    {
        currency.skillPoints = 0;
        currency.upgradePoints = 0;

        skillPointsInputField.text = "";
        upgradePointsInputField.text = "";

        skillPointsInputField.placeholder.GetComponent<TMP_Text>().text = "Enter integer...";
        upgradePointsInputField.placeholder.GetComponent<TMP_Text>().text = "Enter integer...";

        currency.UpdateUI(); // return to private on final build
    }

    private void MaxLevelSkills()
    {
        skills.BuyMaxSkills();
    }

    private void ResetLevelSkills()
    {
        skills.ResetSkills();
    }

    private void MaxLevelUpgrades()
    {
        upgrades.BuyMaxUpgradess();

        if (includeRobotBuddy.isOn)
        {
            upgrades.BuyAllRobotBuddies();
        }

        if (includeThrowUpgrade.isOn)
        {
            upgrades.MaxThrow();
        }
    }

    private void ResetLevelUpgrades()
    {
        upgrades.ResetUpgrades();
        upgrades.ResetRobotBuddy();
    }

    private void NewGame()
    {
        PauseManager.instance.ResumeGame();
        SaveManager.instance.SaveGame();
        SceneLoader.Instance.LoadSceneByIndex(1);
    }

    public void LoadStage()
    {
        PauseManager.instance.ResumeGame();
        SaveManager.instance.LoadGame(1);
    }
}
