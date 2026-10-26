using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [Header("Currency")]
    public int skillPoints;
    public int upgradePoints;

    [Header("UI")]
    [SerializeField] private TMP_Text skillTxt;
    [SerializeField] private TMP_Text upgradeTxt;

    private void Start()
    {
        UpdateUI();
    }

    public void ChangeSkillPoints(int value)
    {
        skillPoints += value;
        UpdateUI();
    }

    public void ChangeUpgradePoints(int value)
    {
        upgradePoints += value;
        UpdateUI();
    }

    public void UpdateUI()
    {
        skillTxt.text = skillPoints.ToString("F0");
        upgradeTxt.text = upgradePoints.ToString("F0");
    }
}
