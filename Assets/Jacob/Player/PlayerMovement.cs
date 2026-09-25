using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private int maxJumps = 1;

    private Vector2 moveInput;
    private int jumpsRemaining;
    private Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    

    void Update()
    {
        OnMove();
        OnJump();
    }

    void OnMove()
    {
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
}
