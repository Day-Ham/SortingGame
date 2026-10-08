using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData Data => itemData;

    private Rigidbody rb;

    [SerializeField] private float stopSpeed = 0.05f;
    [SerializeField] private float stopTime = 0.2f;

    [SerializeField] private float supportCheckDistance = 0.1f;
    [SerializeField] private LayerMask supportLayers;

    private float stoppedTimer;

    public bool IsHeld { get; private set; }
    public Transform HeldBy { get; private set; }

    public bool IsPlaced;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (IsHeld || IsPlaced)
        {
            stoppedTimer = 0f;
            return;
        }

        if (rb.isKinematic)
        {
            if (!HasSupport())
            {
                rb.isKinematic = false;
                stoppedTimer = 0f;
            }

            return;
        }

        if (rb.linearVelocity.magnitude < stopSpeed)
        {
            stoppedTimer += Time.fixedDeltaTime;

            if (stoppedTimer >= stopTime)
            {
                rb.isKinematic = true;
                stoppedTimer = 0f;
            }
        }
        else
        {
            stoppedTimer = 0f;
        }
    }

    private bool HasSupport()
    {
        float rayDistance = GetComponent<Collider>().bounds.extents.y + supportCheckDistance;
        return Physics.Raycast(transform.position, Vector3.down, rayDistance, supportLayers);
    }

    public void Held(bool held, Transform holder = null)
    {
        IsHeld = held;
        if (held)
        {
            HeldBy = holder;
            stoppedTimer = 0f;
            rb.isKinematic = true;
        }
        else
        {
            HeldBy = null;
        }

        if (held)
        {
            stoppedTimer = 0f;
            rb.isKinematic = true;
        }
    }
}