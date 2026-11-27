using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PlayerUpgrade : MonoBehaviour
{
    [Header("Capacity")]
    [SerializeField] private int capacityInitialPrice;
    [SerializeField] private int capacityCurrentPrice;
    [SerializeField] private int upgradeCapacity = 0;
    [SerializeField] private TMP_Text capacityPriceTxt;
    [SerializeField] private Slider upgradeCapacitySlider;

    [Header("Sprint")]
    [SerializeField] private int sprintInitialPrice;
    [SerializeField] private int sprintCurrentPrice;
    [SerializeField] private bool canSprint = false;
    public bool CanSprint => canSprint;
    [SerializeField] private TMP_Text sprintPriceTxt;
    [SerializeField] private Slider upgradeSprintSlider;

    [Header("Reach")]
    [SerializeField] private int reachInitialPrice;
    [SerializeField] private int reachCurrentPrice;
    [SerializeField] private int upgradeReach = 0;
    [SerializeField] private TMP_Text reachPriceTxt;
    [SerializeField] private Slider upgradeReachSlider;

    [Header("Jump")]
    [SerializeField] private int jumpInitialPrice;
    [SerializeField] private int jumpCurrentPrice;
    [SerializeField] private int upgradeJump = 0;
    [SerializeField] private TMP_Text jumpPriceTxt;
    [SerializeField] private Slider upgradeJumpSlider;

    [Header("RobotBuddy")]
    [SerializeField] private int robotBuddyInitialPrice;
    [SerializeField] private int robotBuddyCurrentPrice;
    [SerializeField] private int boughtRobotBuddy = 0;
    [SerializeField] private TMP_Text robotBuddyPriceTxt;
    [SerializeField] private Slider upgradeRobotBuddySlider;

    [Header("Points")]
    [SerializeField] private int totalPointsSpent;

    [Header("Reset")]
    [SerializeField] private Button resetButton;
    public GameObject resetPrompt;
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;

    [Header("Other UI Elements")]
    public GameObject upgradesMenu;
    [SerializeField] private Button closeButton;

    //Upgrade References
    private PlayerMovement playerMovement;
    private PlayerInteraction playerInteraction;
    private RobotBuddySpawner robotBuddySpawner;
    private CurrencyAndShelvedCountManager currencyAndShelvedManager;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponent<PlayerInteraction>();
        robotBuddySpawner = FindAnyObjectByType<RobotBuddySpawner>();
        currencyAndShelvedManager = FindAnyObjectByType<CurrencyAndShelvedCountManager>();
    }

    private void Start()
    {
        resetButton.onClick.AddListener(OpenResetPrompt);
        confirmResetButton.onClick.AddListener(ResetUpgrades);
        cancelResetButton.onClick.AddListener(CloseResetPrompt);
        closeButton.onClick.AddListener(CloseUpgradesMenu);

        OnLoading();
        UpdateUIPrices();
    }

    private void Update()
    {
        if (UserInput.instance == null && PauseManager.instance == null) return;

        if (UserInput.instance.ResetBindingsInput)
        {
            if (upgradesMenu.activeInHierarchy && !resetPrompt.activeInHierarchy)
                OpenResetPrompt();
        }

        if (UserInput.instance.EscapeInput)
        {
            if (resetPrompt.activeInHierarchy)
            {
                CloseResetPrompt();
                return;
            }
            else if (upgradesMenu.activeInHierarchy)
            {
                CloseUpgradesMenu();
            }
        }

        if (UserInput.instance.UpgradeInput && !PauseManager.instance.isPaused)
        {
            if (!upgradesMenu.activeInHierarchy)
            {
                OpenUpgradesMenu();
            }
            else if (upgradesMenu.activeInHierarchy && !resetPrompt.activeInHierarchy)
            {
                CloseUpgradesMenu();
            }
        }
    }

    #region GeneralUse
    private bool TryBuyUpgrade(ref int price)
    {
        if (currencyAndShelvedManager.upgradePoints < price)
            return false;

        currencyAndShelvedManager.ChangeUpgradePoints(-price);

        totalPointsSpent += price;

        price += Mathf.CeilToInt(price * 0.5f);

        return true;
    }

    public void UpdateCooldownSlider(Slider slider, float cooldownTime, float cooldownTimer)
    {
        slider.value = 1f - (cooldownTimer / cooldownTime);
    }

    public void FillBuySlider(Slider slider, int maxValue)
    {
        slider.value += 1f / maxValue;
    }
    #endregion

    #region Buy Upgrade
    public void BuyUpgradeCapacity()
    {
        if (upgradeCapacity >= 10)
            return;

        if (!TryBuyUpgrade(ref capacityCurrentPrice))
            return;

        UpdateUIPrices();

        upgradeCapacity += 2;

        playerInteraction.UpgradeCapacity(2);

        FillBuySlider(upgradeCapacitySlider, 5);

        if(SteamManager.instance != null)
            SteamManager.instance.UpgradePurchased(0, upgradeCapacity, 10);

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
    }

    public void BuyUpgradeSprint()
    {
        if (canSprint)
            return;

        if (!TryBuyUpgrade(ref sprintCurrentPrice))
            return;

        UpdateUIPrices();

        canSprint = true;
        playerMovement.UnlockSprint();

        FillBuySlider(upgradeSprintSlider, 1);

        if (SteamManager.instance != null)
            SteamManager.instance.UpgradePurchased(1, 1, 1);
        
        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
    }

    public void BuyUpgradeReach()
    {
        if (upgradeReach >= 10f)
            return;

        if (!TryBuyUpgrade(ref reachCurrentPrice))
            return;

        UpdateUIPrices();

        upgradeReach += 2;

        playerInteraction.UpgradeReach(2);

        FillBuySlider(upgradeReachSlider, 5);

        if (SteamManager.instance != null)
            SteamManager.instance.UpgradePurchased(2, upgradeReach, 10);

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
    }

    public void BuyUpgradeJump()
    {
        if (upgradeJump >= 3)
            return;

        if (!TryBuyUpgrade(ref jumpCurrentPrice))
            return;

        UpdateUIPrices();

        upgradeJump++;

        playerMovement.UpgradeJump();

        FillBuySlider(upgradeJumpSlider, 3);

        if (SteamManager.instance != null)
            SteamManager.instance.UpgradePurchased(3, upgradeJump, 3);

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
    }

    public void BuyARobotBuddy()
    {
        if(boughtRobotBuddy >= 3) return;

        if (!TryBuyUpgrade(ref robotBuddyCurrentPrice)) return;

        UpdateUIPrices();

        boughtRobotBuddy++;
       
        if (SteamManager.instance != null)
            SteamManager.instance.UnlockAchievement(eAchievement.LittleBuddy);

        robotBuddySpawner.SpawnRobotBuddy();

        FillBuySlider(upgradeRobotBuddySlider, 3);
        
        if (SteamManager.instance != null)
            SteamManager.instance.UpgradePurchased(5, boughtRobotBuddy, 3);

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
    }

    private void UpdateUIPrices()
    {
        capacityPriceTxt.text = capacityCurrentPrice.ToString("F0");
        sprintPriceTxt.text = sprintCurrentPrice.ToString("F0");
        reachPriceTxt.text = reachCurrentPrice.ToString("F0");
        jumpPriceTxt.text = jumpCurrentPrice.ToString("F0");
        robotBuddyPriceTxt.text = robotBuddyCurrentPrice.ToString("F0");
    }
    #endregion

    public void OnLoading()
    {
        for (int i = 0; i < upgradeCapacity; i += 2)
        {
            FillBuySlider(upgradeCapacitySlider, 5);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(0, upgradeCapacity, 10);
        }

        if (canSprint)
        {
            playerMovement.UnlockSprint();
            FillBuySlider(upgradeSprintSlider, 1);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(1, 1, 1);
        }

        for (int i = 0; i < upgradeReach; i += 2)
        {
            FillBuySlider(upgradeReachSlider, 5);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(2, upgradeReach, 10);
        }

        for (int i = 0; i < upgradeJump; i++)
        {
            FillBuySlider(upgradeJumpSlider, 3);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(3, upgradeJump, 3);
        }

        for (int i = 0; i < boughtRobotBuddy; i++)
        {
            FillBuySlider(upgradeRobotBuddySlider, 3);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(5, boughtRobotBuddy, 3);
        }

        UpdateUIPrices();
    }

    #region Menu and Prompt Handling
    private void OpenUpgradesMenu()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.SetAsCurrentOpenMenu(upgradesMenu);
        }
    }

    private void CloseUpgradesMenu()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.CloseMenu(upgradesMenu);
        }
    }

    private void OpenResetPrompt()
    {
        if (GameUIManager.instance != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameUIManager.instance.TweenOpen(resetPrompt);
        }
    }

    private void CloseResetPrompt()
    {
        if(GameUIManager.instance != null)
        {
            GameUIManager.instance.TweenClose(resetPrompt);
        }
    }
    #endregion

    public void ResetUpgrades()
    {
        //Capacity & Reach
        upgradeCapacity = 0;
        upgradeCapacitySlider.value = 0f;
        capacityCurrentPrice = capacityInitialPrice;

        upgradeReach = 0;
        upgradeReachSlider.value = 0f;
        reachCurrentPrice = reachInitialPrice;

        playerInteraction.ResetCapacityAndReach();


        //Sprint & Jump
        canSprint = false;
        upgradeSprintSlider.value = 0f;
        sprintCurrentPrice = sprintInitialPrice;

        upgradeJump = 0;
        upgradeJumpSlider.value = 0f;
        reachCurrentPrice = reachInitialPrice;

        playerMovement.ResetSprintAndJump();


        //RobotBuddy
        ResetRobotBuddy();

        currencyAndShelvedManager.ChangeUpgradePoints(totalPointsSpent);
        totalPointsSpent = 0;

        UpdateUIPrices();
        CloseResetPrompt();
    }

    public void ResetRobotBuddy()
    {
        RobotBuddy[] robots = Object.FindObjectsByType<RobotBuddy>();

        if (robots.Length == 0) return;

        foreach (RobotBuddy robot in robots)
        {
            Item item = robot.targetItem;
            if (item != null)
            {
                robot.targetItem = null;

                if (item.HeldBy == robot.transform)
                {
                    item.Held(false);
                    item.transform.SetParent(FindAnyObjectByType<ItemSpawner>().transform);
                }
            }
            Destroy(robot.gameObject);
        }

        upgradeRobotBuddySlider.value = 0f;
        robotBuddyCurrentPrice = robotBuddyInitialPrice;
    }

    // QA EDITOR
    public void BuyMaxUpgradess()
    {
        //Capacity
        for (int i = 0; i<5; i++)
        {
            if (upgradeCapacity >= 10)
                break;

            upgradeCapacity += 2;

            playerInteraction.UpgradeCapacity(2);

            FillBuySlider(upgradeCapacitySlider, 5);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(0, upgradeCapacity, 10);
        }

        //Sprint
        if (!canSprint)
        {
            canSprint = true;
            playerMovement.UnlockSprint();
            FillBuySlider(upgradeSprintSlider, 1);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(1, 1, 1);
        }

        //Reach
        for (int i = 0; i < 5; i++)
        {
            if (upgradeReach >= 10f)
                break;

            upgradeReach += 2;

            playerInteraction.UpgradeReach(2);

            FillBuySlider(upgradeReachSlider, 5);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(2, upgradeReach, 10);
        }

        //Jump
        for (int i = 0; i < 3; i++)
        {
            if (upgradeJump >= 3) break;

            upgradeJump++;

            playerMovement.UpgradeJump();

            FillBuySlider(upgradeJumpSlider, 3);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(3, upgradeJump, 3);
        }
    }

    public void BuyAllRobotBuddies()
    {
        for (int i = 0; i < 3; i++)
        {
            if (boughtRobotBuddy >= 3) return;

            boughtRobotBuddy++;

            robotBuddySpawner.SpawnRobotBuddy();

            FillBuySlider(upgradeRobotBuddySlider, 3);
            if (SteamManager.instance != null)
                SteamManager.instance.UpgradePurchased(5, boughtRobotBuddy, 3);
        }
    }

    
}
