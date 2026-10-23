using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAEditor : MonoBehaviour
{
    [SerializeField] private GameObject editorPanel;

    [Header("Currency")]
    [SerializeField] private TMP_InputField skillPointsInputField;
    [SerializeField] private float skillPoints;
    [SerializeField] private TMP_InputField upgradePointsInputField;
    [SerializeField] private float upgradePoints;
    [SerializeField] private Button applyPointsButton;
    [SerializeField] private Button resetPointsButton;

    [Header("Skills")]
    [SerializeField] private Button maxLevelSkills;
    [SerializeField] private Button resetLevelSkills;

    [Header("Upgrades")]
    [SerializeField] private Button maxLevelUpgrades;
    [SerializeField] private Button resetLevelUpgrades;
    [SerializeField] private Toggle includeRobotBuddy;

    [Header("Scene")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadSaveGameButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
