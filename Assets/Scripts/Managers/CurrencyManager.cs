using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrencyAndShelvedCountManager : MonoBehaviour
{
    [Header("Currency")]
    public int skillPoints;
    public int upgradePoints;
    [SerializeField] private TMP_Text skillTxt;
    [SerializeField] private TMP_Text upgradeTxt;

    [Header("Shelved Count")]
    [SerializeField] private TMP_Text shelvedItemText;
    private int shelvedItemCount;
    [SerializeField] private TMP_Text maxItemText;
    private int maxItemCount;

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
