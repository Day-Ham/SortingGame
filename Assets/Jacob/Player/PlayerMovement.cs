using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;

    CharacterController controller;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        sprintAction = playerInput.actions.FindAction("Sprint");
        jumpAction = playerInput.actions.FindAction("Jump");
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    

    void Update()
    { 
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 dir = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * dir.x + transform.forward * dir.y;

        float currentSpeed = moveSpeed;

        if (sprintAction.IsPressed() && dir.y > 0)
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
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
