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
    BullyBuddy,
    NoUpgrades,
    MaxUpgrades,
    UnlockAllSkills,
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
    private int totalNumberOfAchievements = 21;
    private bool connectedToSteam = false;

    [Header("Stats")]
    [SerializeField] private int itemsShelved;
    [SerializeField] private int shelvesCompleted;
    [SerializeField] private bool stage1Complete;
    [SerializeField] private bool stage2Complete;
    [SerializeField] private bool stage3Complete;

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
                SteamManager.instance.UnlockAchievement(eAchievement.FirstItem);
                break;
            case 100:
                SteamManager.instance.UnlockAchievement(eAchievement.OneHundredItems);
                break;
            case 500:
                SteamManager.instance.UnlockAchievement(eAchievement.FiveHundredItems);
                break;
            case 1000:
                SteamManager.instance.UnlockAchievement(eAchievement.OneThousandItems);
                break;
            case 3000:
                SteamManager.instance.UnlockAchievement(eAchievement.ThreeThousandItems);
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
                SteamManager.instance.UnlockAchievement(eAchievement.FirstShelf);
                break;
            case 10:
                SteamManager.instance.UnlockAchievement(eAchievement.TenShelves);
                break;
            case 50:
                SteamManager.instance.UnlockAchievement(eAchievement.FiftyShelves);
                break;
            case 100:
                SteamManager.instance.UnlockAchievement(eAchievement.OneHundredShelves);
                break;
            case 200:
                SteamManager.instance.UnlockAchievement(eAchievement.TwoHundredShelves);
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
                stage1Complete = true;
                SteamManager.instance.UnlockAchievement(eAchievement.CompleteStageOne);
                break;
            case 2:
                stage2Complete = true;
                SteamManager.instance.UnlockAchievement(eAchievement.CompleteStageTwo);
                break;
            case 3:
                stage3Complete = true;
                SteamManager.instance.UnlockAchievement(eAchievement.CompleteStageThree);
                break;
            default:
                break;
        }
    }
}
