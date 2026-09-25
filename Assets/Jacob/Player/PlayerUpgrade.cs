using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUpgrade : MonoBehaviour
{
    [Header("Cost")]
    [Header("Upgrade Prices")]
    [SerializeField] private int capacityPrice = 50;
    [SerializeField] private int sprintPrice = 50;
    [SerializeField] private int reachPrice = 50;
    [SerializeField] private int jumpPrice = 50;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Upgrade Values")]
    [SerializeField] private int upgradeCapacity = 0;
    [SerializeField] private float upgradeSprint = 0f;
    [SerializeField] private float upgradeReach = 0f;
    [SerializeField] private int upgradeJump = 0;

    //Upgrade References
    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private PlayerInteraction playerInteraction;

    UIManager ui;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerLook = GetComponent<PlayerLook>();
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    private void Start()
    {
        ui = UIManager.instance;
    }

    #region GeneralUse
    private bool TryBuyUpgrade(ref int price)
    {
        if (currencyManager.coin < price)
            return false;

        currencyManager.ChangeCoin(-price);

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
    }

    public void BuyUpgradeSprint()
    {
        if (upgradeSprint >= 10f)
            return;

        if (!TryBuyUpgrade(ref sprintPrice))
            return;

        upgradeSprint += 2f;

        playerMovement.UpgradeSprint(2f);

        ui.FillSlider(ui.upgradeSprintSlider, 5);
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
    }
    #endregion
}
