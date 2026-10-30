using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;
    
    // Game
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool CrouchInput { get; private set; }
    public bool InteractionInput { get; private set; }
    public bool ThrowInput { get; private set; }
    public bool UpgradeInput { get; private set; }
    public bool SkillInput { get; private set; }
    public bool Skill1Input { get; private set; }
    public bool Skill2Input { get; private set; }
    public bool Skill3Input { get; private set; }
    public bool ScrollUpInput { get; private set; }
    public bool ScrollDownInput { get; private set; }
    public bool PlayerInputEnabled { get; private set; } = true;

    //UI
    public bool ResetBindingsInput { get; private set; }
    public bool PrimaryTabLeftInput { get; private set; }
    public bool PrimaryTabRightInput { get; private set; }
    public bool SecondaryTabLeftInput { get; private set; }
    public bool SecondaryTabRightInput { get; private set; }

    //Player and UI
    public bool EscapeInput { get; private set; }

    private PlayerInput playerInput;

    //Game
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction interactAction;
    private InputAction throwAction;
    private InputAction upgradeAction;
    private InputAction skillsAction;
    private InputAction playerEscapeAction;
    private InputAction skill1Action;
    private InputAction skill2Action;
    private InputAction skill3Action;
    private InputAction scrollUpAction;
    private InputAction scrollDownAction;

    //UI
    private InputAction uiEscapeAction;
    private InputAction resetBindingsAction;
    private InputAction primaryTabLeftAction;
    private InputAction primaryTabRightAction;
    private InputAction secondaryTabLeftAction;
    private InputAction secondaryTabRightAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        SetUpInputAction();
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void SetUpInputAction()
    {
        // Game
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        sprintAction = playerInput.actions.FindAction("Sprint");
        jumpAction = playerInput.actions.FindAction("Jump");
        crouchAction = playerInput.actions.FindAction("Crouch");
        interactAction = playerInput.actions.FindAction("Interact");
        throwAction = playerInput.actions.FindAction("Throw");
        upgradeAction = playerInput.actions.FindAction("UpgradeMenu");
        skillsAction = playerInput.actions.FindAction("SkillsMenu");
        playerEscapeAction = playerInput.actions.FindAction("Player/Escape");
        skill1Action = playerInput.actions.FindAction("Skill1");
        skill2Action = playerInput.actions.FindAction("Skill2");
        skill3Action = playerInput.actions.FindAction("Skill3");
        scrollUpAction = playerInput.actions.FindAction("ScrollUp");
        scrollDownAction = playerInput.actions.FindAction("ScrollDown");

        // UI
        uiEscapeAction = playerInput.actions.FindAction("UI/Escape");
        resetBindingsAction = playerInput.actions.FindAction("ResetBindings");
        primaryTabLeftAction = playerInput.actions.FindAction("PrimaryTabLeft");
        primaryTabRightAction = playerInput.actions.FindAction("PrimaryTabRight");
        secondaryTabLeftAction = playerInput.actions.FindAction("SecondaryTabLeft");
        secondaryTabRightAction = playerInput.actions.FindAction("SecondaryTabRight");
    }

    private void UpdateInputs()
    {
        // Game
        MoveInput = PlayerInputEnabled ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        LookInput = PlayerInputEnabled ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        SprintInput = PlayerInputEnabled && sprintAction.IsPressed();
        JumpInput = PlayerInputEnabled && jumpAction.WasPressedThisFrame();
        CrouchInput = PlayerInputEnabled && crouchAction.WasPressedThisFrame();
        InteractionInput = PlayerInputEnabled && interactAction.WasPressedThisFrame();
        ThrowInput = PlayerInputEnabled && throwAction.WasPressedThisFrame();
        UpgradeInput = upgradeAction.WasPressedThisFrame();
        SkillInput = skillsAction.WasPressedThisFrame();
        Skill1Input = PlayerInputEnabled && skill1Action.WasPressedThisFrame();
        Skill2Input = PlayerInputEnabled && skill2Action.WasPressedThisFrame();
        Skill3Input = PlayerInputEnabled && skill3Action.WasPressedThisFrame();
        ScrollUpInput = PlayerInputEnabled && scrollUpAction.WasPressedThisFrame();
        ScrollDownInput = PlayerInputEnabled && scrollDownAction.WasPressedThisFrame();

        // UI
        ResetBindingsInput = resetBindingsAction.WasPressedThisFrame();
        PrimaryTabLeftInput = primaryTabLeftAction.WasPressedThisFrame();
        PrimaryTabRightInput = primaryTabRightAction.WasPressedThisFrame();
        SecondaryTabLeftInput = secondaryTabLeftAction.WasPressedThisFrame();
        SecondaryTabRightInput = secondaryTabRightAction.WasPressedThisFrame();

        // Game & UI
        EscapeInput = playerEscapeAction.WasPressedThisFrame() || uiEscapeAction.WasPressedThisFrame();

    }

    public void SetPlayerInput(bool enabled)
    {
        PlayerInputEnabled = enabled;
    }
}
