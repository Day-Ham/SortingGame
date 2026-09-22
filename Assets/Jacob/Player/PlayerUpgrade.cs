using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUpgrade : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private GameObject skillsMenu;

    [Header("Cost")]
    [Header("Upgrade Prices")]
    [SerializeField] private int capacityPrice = 50;
    [SerializeField] private int sprintPrice = 50;
    [SerializeField] private int reachPrice = 50;
    [SerializeField] private int jumpPrice = 50;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Upgrade Sliders")]
    [SerializeField] private Slider upgradeCapacitySlider;
    [SerializeField] private Slider upgradeSprintSlider;
    [SerializeField] private Slider upgradeReachSlider;
    [SerializeField] private Slider upgradeJumpSlider;

    [Header("Upgrade Values")]
    [SerializeField] private int upgradeCapacity = 0;
    [SerializeField] private float upgradeSprint = 0f;
    [SerializeField] private float upgradeReach = 0f;
    [SerializeField] private int upgradeJump = 0;

    [Header("Skill Sliders")]
    [SerializeField] private Slider skill1Slider;
    [SerializeField] private Slider skill2Slider;
    [SerializeField] private Slider skill3Slider;
    [SerializeField] private Slider skill4Slider;


    //Upgrade References
    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private PlayerInteraction playerInteraction;

    PlayerInput playerInput;

    InputAction upgradeAction;
    InputAction skillsAction;
    InputAction escapeAction;

    InputAction moveAction;
    InputAction lookAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerLook = GetComponent<PlayerLook>();
        playerInteraction = GetComponent<PlayerInteraction>();

        upgradeAction = playerInput.actions.FindAction("UpgradeMenu");
        skillsAction = playerInput.actions.FindAction("SkillsMenu");
        escapeAction = playerInput.actions.FindAction("Escape");
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
    }

    private void Start()
    {
        CloseMenus();
    }

    private void OnEnable()
    {
        upgradeAction.performed += OnUpgradePressed;
        skillsAction.performed += OnSkillsPressed;
        escapeAction.performed += OnEscapePressed;
    }

    private void OnDisable()
    {
        upgradeAction.performed -= OnUpgradePressed;
        skillsAction.performed -= OnSkillsPressed;
        escapeAction.performed -= OnEscapePressed;
    }

    private void OnUpgradePressed(InputAction.CallbackContext context)
    {
        OpenUpgradeMenu();
    }

    private void OnSkillsPressed(InputAction.CallbackContext context)
    {
        OpenSkillsMenu();
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        CloseMenus();
    }


    #region GeneralUse
    public void OpenUpgradeMenu()
    {
        upgradeMenu.SetActive(true);
        skillsMenu.SetActive(false);

        DisablePlayerControl();
        UnlockCursor();
    }

    public void OpenSkillsMenu()
    {
        skillsMenu.SetActive(true);
        upgradeMenu.SetActive(false);

        DisablePlayerControl();
        UnlockCursor();
    }

    public void CloseMenus()
    {
        upgradeMenu.SetActive(false);
        skillsMenu.SetActive(false);

        EnablePlayerControl();
        LockCursor();
    }

    private void DisablePlayerControl()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    private void EnablePlayerControl()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FillSlider(Slider slider, int maxLevels)
    {
        slider.value += 1f / maxLevels;
    }

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

        FillSlider(upgradeCapacitySlider, 5);
    }

    public void BuyUpgradeSprint()
    {
        if (upgradeSprint >= 10f)
            return;

        if (!TryBuyUpgrade(ref sprintPrice))
            return;

        upgradeSprint += 2f;

        playerMovement.UpgradeSprint(2f);

        FillSlider(upgradeSprintSlider, 5);
    }

    public void BuyUpgradeReach()
    {
        if (upgradeReach >= 10f)
            return;

        if (!TryBuyUpgrade(ref reachPrice))
            return;

        upgradeReach += 2f;

        playerInteraction.UpgradeReach(2);

        FillSlider(upgradeReachSlider, 5);
    }

    public void BuyUpgradeJump()
    {
        if (upgradeJump >= 3)
            return;

        if (!TryBuyUpgrade(ref jumpPrice))
            return;

        upgradeJump++;

        playerMovement.UpgradeJump();

        FillSlider(upgradeJumpSlider, 3);
    }
    #endregion
}
