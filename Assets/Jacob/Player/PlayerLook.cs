using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int lookSensitivity = 5;
    [SerializeField] private float maxLookAngle = 80f;
    Vector2 lookInput;
    private float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    void Look()
    {
        lookInput = UserInput.instance.LookInput;

        float mouseX = lookInput.x * lookSensitivity * 0.01f;
        float mouseY = lookInput.y * lookSensitivity * 0.01f;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}