using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData Data => itemData;

    private Rigidbody rb;

    [SerializeField] private float stopSpeed = 0.05f;
    [SerializeField] private float stopTime = 0.2f;

    private float stoppedTimer;

    public bool IsHeld { get; private set; }
    public bool IsPlaced;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (IsHeld)
        {
            stoppedTimer = 0f;
            return;
        }

        if (rb.linearVelocity.magnitude < stopSpeed)
        {
            stoppedTimer += Time.fixedDeltaTime;
            if (stoppedTimer >= stopTime && !rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
        else
        {
            stoppedTimer = 0f;
        }
    }

    public void Held(bool held)
    {
        IsHeld = held;

        if (held)
        {
            stoppedTimer = 0f;
            rb.isKinematic = true;
        }
    }
}