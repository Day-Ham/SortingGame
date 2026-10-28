using UnityEngine;

public class PlayerUpgrade : MonoBehaviour
{
    [Header("Cost")]
    [Header("Upgrade Prices")]
    [SerializeField] private int capacityPrice = 50;
    [SerializeField] private int sprintPrice = 50;
    [SerializeField] private int reachPrice = 50;
    [SerializeField] private int jumpPrice = 50;
    [SerializeField] private int throwToShelfPrice = 500;
    [SerializeField] private int robotBuddyPrice = 1000;
    CurrencyManager currencyManager;

    [Header("Upgrade Values")]
    [SerializeField] private int upgradeCapacity = 0;
    [SerializeField] private bool canSprint = false;
    public bool CanSprint => canSprint;
    [SerializeField] private float upgradeReach = 0f;
    [SerializeField] private int upgradeJump = 0;
    [SerializeField] private bool canThrowToShelf = false;
    public bool CanThrowToShelf => canThrowToShelf;
    [SerializeField] private float boughtRobotBuddy = 0f;

    //Upgrade References
    private PlayerMovement playerMovement;
    private PlayerInteraction playerInteraction;
    private RobotBuddySpawner robotBuddySpawner;

    private SaveManager saveManager;

    private GameUIManager ui;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInteraction = GetComponent<PlayerInteraction>();
        robotBuddySpawner = FindAnyObjectByType<RobotBuddySpawner>();
        currencyManager = FindAnyObjectByType<CurrencyManager>();
    }

    private void Start()
    {
        saveManager = SaveManager.instance;
        ui = GameUIManager.instance;

        OnLoading();
    }

    #region GeneralUse
    private bool TryBuyUpgrade(ref int price)
    {
        if (currencyManager.upgradePoints < price)
            return false;

        currencyManager.ChangeUpgradePoints(-price);

        price += 50;

        return true;
    }
    #endregion

    #region BuyUpgradeButtons
    public void BuyUpgradeCapacity()
    {
        if (upgradeCapacity >= 10)
            return;

        if (!TryBuyUpgrade(ref capacityPrice))
            return;

        upgradeCapacity += 2;

        playerInteraction.UpgradeCapacity(2);

        ui.FillSlider(ui.upgradeCapacitySlider, 5);

        if(saveManager != null)
            saveManager.SaveGame();
    }

    public void BuyUpgradeSprint()
    {
        if (canSprint)
            return;

        if (!TryBuyUpgrade(ref sprintPrice))
            return;

        canSprint = true;
        playerMovement.UnlockSprint();

        ui.FillSlider(ui.upgradeSprintSlider, 1);

        if (saveManager != null)
            saveManager.SaveGame();
    }

    public void BuyUpgradeReach()
    {
        if (upgradeReach >= 10f)
            return;

        if (!TryBuyUpgrade(ref reachPrice))
            return;

        upgradeReach += 2f;

        playerInteraction.UpgradeReach(2);

        ui.FillSlider(ui.upgradeReachSlider, 5);

        if (saveManager != null)
            saveManager.SaveGame();
    }

    public void BuyUpgradeJump()
    {
        if (upgradeJump >= 3)
            return;

        if (!TryBuyUpgrade(ref jumpPrice))
            return;

        upgradeJump++;

        playerMovement.UpgradeJump();

        ui.FillSlider(ui.upgradeJumpSlider, 3);

        if (saveManager != null)
            saveManager.SaveGame();
    }

    public void BuyUpgradeThrowToShelf()
    {
        if (canThrowToShelf) return;

        if (!TryBuyUpgrade(ref throwToShelfPrice)) return;

        canThrowToShelf = true;

        ui.FillSlider(ui.upgradeThrowSlider, 1);

        if (saveManager != null)
            saveManager.SaveGame();
    }

    public void BuyARobotBuddy()
    {
        if(boughtRobotBuddy >= 3) return;

        if (!TryBuyUpgrade(ref robotBuddyPrice)) return;

        boughtRobotBuddy++;

        robotBuddySpawner.SpawnRobotBuddy();

        ui.FillSlider(ui.upgradeRobotBuddySlider, 3);

        if (saveManager != null)
            saveManager.SaveGame();
    }
    #endregion

    public void OnLoading()
    {
        if (ui == null) return;

        for (int i = 0; i < upgradeCapacity; i += 2)
        {
            ui.FillSlider(ui.upgradeCapacitySlider, 5);
        }

        if (canSprint)
        {
            playerMovement.UnlockSprint();
            ui.FillSlider(ui.upgradeSprintSlider, 1);
        }

        for (int i = 0; i < upgradeReach; i += 2)
        {
            ui.FillSlider(ui.upgradeReachSlider, 5);
        }

        for (int i = 0; i < upgradeJump; i++)
        {
            ui.FillSlider(ui.upgradeJumpSlider, 3);
        }

        if (canThrowToShelf)
            ui.FillSlider(ui.upgradeThrowSlider, 1);

        for (int i = 0; i < boughtRobotBuddy; i++)
        {
            ui.FillSlider(ui.upgradeRobotBuddySlider, 3);
        }
    }

    public void BuyMaxUpgradess()
    {
        //Capacity
        for (int i = 0; i<5; i++)
        {
            if (upgradeCapacity >= 10)
                break;

            upgradeCapacity += 2;

            playerInteraction.UpgradeCapacity(2);

            ui.FillSlider(ui.upgradeCapacitySlider, 5);  
        }

        //Sprint
        if (!canSprint)
        {
            canSprint = true;
            playerMovement.UnlockSprint();
            ui.FillSlider(ui.upgradeSprintSlider, 1);
        }

        //Reach
        for (int i = 0; i < 5; i++)
        {
            if (upgradeReach >= 10f)
                break;

            upgradeReach += 2f;

            playerInteraction.UpgradeReach(2);

            ui.FillSlider(ui.upgradeReachSlider, 5);
        }

        //Jump
        for (int i = 0; i < 3; i++)
        {
            if (upgradeJump >= 3) break;

            upgradeJump++;

            playerMovement.UpgradeJump();

            ui.FillSlider(ui.upgradeJumpSlider, 3);
        }
    }

    public void BuyAllRobotBuddies()
    {
        for (int i = 0; i < 3; i++)
        {
            if (boughtRobotBuddy >= 3) return;

            boughtRobotBuddy++;

            robotBuddySpawner.SpawnRobotBuddy();

            ui.FillSlider(ui.upgradeRobotBuddySlider, 3);
        }
    }

    public void MaxThrow()
    {
        //Throw Shelf
        if (canThrowToShelf) return;

        canThrowToShelf = true;

        ui.FillSlider(ui.upgradeThrowSlider, 1);
    }

    public void ResetUpgrades()
    {
        //Capacity & Reach
        upgradeCapacity = 0;
        upgradeReach = 0;
        playerInteraction.ResetCapacityAndReach();
        ui.upgradeCapacitySlider.value = ui.upgradeCapacitySlider.minValue;
        ui.upgradeReachSlider.value = ui.upgradeReachSlider.minValue;

        //Sprint & Jump
        canSprint = false;
        upgradeJump = 0;
        playerMovement.ResetSprintAndJump();
        ui.upgradeSprintSlider.value = ui.upgradeSprintSlider.minValue;
        ui.upgradeJumpSlider.value = ui.upgradeJumpSlider.minValue;

        //Throw Shelf
        canThrowToShelf = false;
        ui.upgradeThrowSlider.value = ui.upgradeThrowSlider.minValue;
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

        ui.upgradeRobotBuddySlider.value = ui.upgradeRobotBuddySlider.minValue;
    }
}
