using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask pickupLayer;

    [Header("Throw")]
    [SerializeField] private float throwForce = 5f;

    private GameObject heldObject;
    private Rigidbody heldRigidbody;

    // Update is called once per frame
    void Update()
    {
        if (heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward);
        if (Physics.Raycast( ray,out RaycastHit hit,pickupRange,pickupLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            Item item = hit.collider.GetComponent<Item>();

            if (rb != null && item != null)
            {
                heldObject = rb.gameObject;
                heldRigidbody = rb;

                item.Held(true);

                heldObject.transform.position = holdPoint.position;
                heldObject.transform.rotation = holdPoint.rotation;
            }
        }
    }

    void MoveHeldObject()
    {
        heldObject.transform.position = holdPoint.position;
        heldObject.transform.rotation = holdPoint.rotation;
    }

    void DropObject()
    {
        heldRigidbody.isKinematic = false;
        heldObject = null;
        heldRigidbody = null;
    }

    void ThrowObject()
    {
        Item item = heldObject.GetComponent<Item>();
        if (item != null)
        {
            item.Held(false);
        }

        heldRigidbody.isKinematic = false;
        heldRigidbody.AddForce(playerCamera.transform.forward * throwForce,ForceMode.Impulse);

        heldObject = null;
        heldRigidbody = null;
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (heldObject == null)
        {
            TryPickup();
        }
        else
        {
            DropObject();
        }
    }

    public void OnThrow(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (heldObject != null)
        {
            ThrowObject();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.transform.position,playerCamera.transform.forward * pickupRange);

        if (holdPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere( holdPoint.position,0.15f);
            Gizmos.DrawLine(playerCamera.transform.position,holdPoint.position);
        }
    }
}
