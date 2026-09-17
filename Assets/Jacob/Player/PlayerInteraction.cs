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

    public GameObject heldObject;
    private Rigidbody heldRigidbody;

    // Update is called once per frame
    void Update()
    {
        
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            Item item = hit.collider.GetComponent<Item>();

            if (rb != null && item != null)
            {
                heldObject = rb.gameObject;
                heldRigidbody = rb;

                heldRigidbody.isKinematic = true;
                heldObject.transform.SetParent(holdPoint);

                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;

                item.Held(true);
            }
        }
    }


    void ThrowObject()
    {
        Item item = heldObject.GetComponent<Item>();

        if (item != null)
        {
            item.Held(false);
        }

        heldObject.transform.SetParent(null);

        heldRigidbody.isKinematic = false;

        heldRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);

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
