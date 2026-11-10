using System;
using UnityEngine;

public enum eAchievement
{
    FirstItem,
    OneHundredItems,
    FiveHundredItems,
    OneThousandItems,
    ThreeThousandItems,
    FirstShelf,
    TenShelves,
    FiftyShelves,
    OneHundredShelves,
    TwoHundredShelves,
    LittleBuddy,
    PlayBuddy,
    TeaseBuddy,
    BullyBuddy,
    NoUpgrades,
    MaxUpgrades,
    MaxSkills,
    CompleteStageOne,
    StageOneNoSkillUsed,
    CompleteStageTwo,
    StageTwoNoSkillUsed,
    CompleteStageThree,
    StageThreeNoSkillUsed,
}

public class SteamManager : MonoBehaviour
{
    public static SteamManager instance;
    private uint appID = 00000;
    private int totalNumberOfAchievements = 23;
    private bool connectedToSteam = false;

    [Header("Items")]
    [SerializeField] private int itemsShelved; // global

    [Header("Shelves")]
    [SerializeField] private int shelvesCompleted; // global

    [Header("Upgrades")]
    [SerializeField] private bool boughtUpgrade = false;
    [SerializeField] private bool maxCapacity = false;
    [SerializeField] private bool maxSprint = false;
    [SerializeField] private bool maxReach = false;
    [SerializeField] private bool maxJump = false;
    [SerializeField] private bool maxRobotBuddy = false;
    [SerializeField] private int stolenFromRobotBuddy; // global

    [Header("Skills")]
    [SerializeField] private bool skillUsed = false;
    [SerializeField] private bool maxSkill1 = false;
    [SerializeField] private bool maxSkill2 = false;
    [SerializeField] private bool maxSkill3 = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        try
        {
            Steamworks.SteamClient.Init(appID);
            connectedToSteam = true;
        }
        catch(System.Exception exception)
        {
            connectedToSteam = false;
            Debug.LogWarning("Not connected to Steam");
        }
    }

    private void Start()
    {
        itemsShelved = ES3.Load("itemsShelved", 0);
        shelvesCompleted = ES3.Load("shelvesCompleted", 0);
        stolenFromRobotBuddy = ES3.Load("stolenFromRobotBuddy", 0);
    }

    private void OnApplicationQuit()
    {
        ES3.Save("itemsShelved", itemsShelved);
        ES3.Save("shelvesCompleted", shelvesCompleted);
        ES3.Save("stolenFromRobotBuddy", stolenFromRobotBuddy);
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamClient.RunCallbacks();
        }
    }

    public void DisconnectFromSteam()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamClient.Shutdown();
        }
    }

    public void UnlockAchievement(eAchievement _AchievementToUnlock)
    {
        if (connectedToSteam)
        {
            var ach = new Steamworks.Data.Achievement("Achievement_" + (int)_AchievementToUnlock);
            if (ach.State == true) return;            
            ach.Trigger();
        }

        Debug.Log("Achievement Unlocked: " + _AchievementToUnlock);
    }

    private void ResetAllAchievements()
    {
        for (int i = 0; i < totalNumberOfAchievements; i++)
        {
            var ach = new Steamworks.Data.Achievement("Achievement_" + i);
            ach.Clear();
        }
    }

    public void ItemShelvedCounter()
    {
        itemsShelved++;

        switch (itemsShelved)
        {
            case 1:
                UnlockAchievement(eAchievement.FirstItem);
                break;
            case 100:
                UnlockAchievement(eAchievement.OneHundredItems);
                break;
            case 500:
                UnlockAchievement(eAchievement.FiveHundredItems);
                break;
            case 1000:
                UnlockAchievement(eAchievement.OneThousandItems);
                break;
            case 3000:
                UnlockAchievement(eAchievement.ThreeThousandItems);
                break;
            default:
                break;
        }
    }

    public void CompletedShelvesCounter()
    {
        shelvesCompleted++;

        switch (shelvesCompleted)
        {
            case 1:
                UnlockAchievement(eAchievement.FirstShelf);
                break;
            case 10:
                UnlockAchievement(eAchievement.TenShelves);
                break;
            case 50:
                UnlockAchievement(eAchievement.FiftyShelves);
                break;
            case 100:
                UnlockAchievement(eAchievement.OneHundredShelves);
                break;
            case 200:
                UnlockAchievement(eAchievement.TwoHundredShelves);
                break;
            default:
                break;
        }
    }

    public void CompleteStage(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1:
                UnlockAchievement(eAchievement.CompleteStageOne);
                break;
            case 2:
                UnlockAchievement(eAchievement.CompleteStageTwo);
                break;
            case 3:
                UnlockAchievement(eAchievement.CompleteStageThree);
                break;
            default:
                break;
        }

        CheckIfUpgradeWasPurchased();
        CheckIfSkillWasUsed(stageNumber);
    }

    public void UpgradePurchased(int upgradeIndex, int currentLevel, int maxLevel)
    {
        boughtUpgrade = true;

        if (currentLevel < maxLevel) return;

        switch (upgradeIndex)
        {
            case 0:
                maxCapacity = true;
                break;
            case 1:
                maxSprint = true;
                break;
            case 2:
                maxReach = true;
                break;
            case 3:
                maxJump = true;
                break;
            case 5:
                maxRobotBuddy = true;
                break;
            default:
                break;
        }

        if (maxCapacity && maxSprint && maxReach && maxJump && maxRobotBuddy)
        {
            UnlockAchievement(eAchievement.MaxUpgrades);
        }
    }

    public void CheckIfUpgradeWasPurchased()
    {
        if (boughtUpgrade) return;

        UnlockAchievement(eAchievement.NoUpgrades);
    }

    public void SkillPurchased(int skillNumber, int currentLevel, int maxLevel)
    {
        if (currentLevel < maxLevel) return;

        switch (skillNumber)
        {
            case 1:
                maxSkill1 = true;
                break;
            case 2:
                maxSkill2 = true;
                break;
            case 3:
                maxSkill3 = true;
                break;
            default:
                break;
        }

        if (maxSkill1 && maxSkill2 && maxSkill3)
        {
            UnlockAchievement(eAchievement.MaxSkills);
        }
    }

    public void SkillWasUsed()
    {
        skillUsed = true;
    }

    public void CheckIfSkillWasUsed(int stageNumber)
    {
        if (skillUsed) return;

        switch (stageNumber)
        {
            case 1:
                UnlockAchievement(eAchievement.StageOneNoSkillUsed);
                break;
            case 2:
                UnlockAchievement(eAchievement.StageTwoNoSkillUsed);
                break;
            case 3:
                UnlockAchievement(eAchievement.StageThreeNoSkillUsed);
                break;
            default:
                break;
        }
    }

    public void StealFromRobotBuddy()
    {
        stolenFromRobotBuddy++;

        switch (stolenFromRobotBuddy)
        {
            case 1:
                UnlockAchievement(eAchievement.PlayBuddy);
                break;
            case 5:
                UnlockAchievement(eAchievement.TeaseBuddy);
                break;
            case 20:
                UnlockAchievement(eAchievement.BullyBuddy);
                break;
            default:
                break;
        }

    }
}
