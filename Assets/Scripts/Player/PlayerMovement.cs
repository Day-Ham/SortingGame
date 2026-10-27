using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;

    [SerializeField] private GameObject camera;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchSpeed;

    private Vector3 targetCameraPosition;
    private bool isCrouching;
    private Vector2 moveInput;
    private int jumpsRemaining;
    private Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        isCrouching = false;
        targetCameraPosition = new Vector3(0f, 1f, 0f);
        camera.transform.localPosition = targetCameraPosition;
    }

    // Update is called once per frame
    void Update()
    {
        OnMove();
        OnJump();
        OnCrouch();

        camera.transform.localPosition = Vector3.Lerp( camera.transform.localPosition, targetCameraPosition, crouchSpeed * Time.deltaTime);
    }

    void OnMove()
    {
        if (UserInput.instance == null) return;
        moveInput = UserInput.instance.MoveInput;
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        float currentSpeed = moveSpeed;

        if (UserInput.instance.SprintInput && moveInput.y > 0)
        {
            currentSpeed = sprintSpeed;
        }

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

            jumpsRemaining = maxJumps;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnJump()
    {
        if (UserInput.instance == null) return;

        if (!UserInput.instance.JumpInput)
            return;

        if (controller.isGrounded)
        {
            jumpsRemaining = maxJumps - 1;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            return;
        }

        if (jumpsRemaining > 0)
        {
            jumpsRemaining--;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnCrouch()
    {
        if (UserInput.instance == null) return;

        if (!UserInput.instance.CrouchInput)
            return;

        isCrouching = !isCrouching;

        if (isCrouching)
        {
            targetCameraPosition = new Vector3(0f, crouchHeight, 0f);
        }
        else
        {
            targetCameraPosition = new Vector3(0f, 1f, 0f);
        }
    }

    public void UpgradeSprint(float amount)
    {
        sprintSpeed += amount;
    }

    public void UpgradeJump()
    {
        if (maxJumps >= 4)
            return;

        maxJumps++;
    }

    public void ResetSprintAndJump()
    {
        sprintSpeed = 10;
        maxJumps = 1;
    }
}
