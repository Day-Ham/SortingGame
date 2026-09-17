using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float pickupSpeed = 12f;

    [Header("Throw")]
    [SerializeField] private float throwForce = 5f;

    public GameObject heldObject;
    private Rigidbody heldRigidbody;

    public List<GameObject> heldItems = new List<GameObject>();
    private int activeItemIndex = -1;

    private bool isPickingUp;

    void Update()
    {
        if (heldObject != null && isPickingUp)
        {
            PickupLerp();
        }

        float scroll = Mouse.current.scroll.y.ReadValue();

        if (scroll != 0 && heldItems.Count > 1)
        {
            SwitchItem(scroll > 0 ? 1 : -1);
        }
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
                if (heldObject != null)
                {
                    heldObject.SetActive(false);
                }

                heldObject = rb.gameObject;
                heldRigidbody = rb;

                heldItems.Add(heldObject);
                activeItemIndex = heldItems.Count - 1;

                heldRigidbody.isKinematic = true;
                item.Held(true);
                heldObject.transform.SetParent(holdPoint, true);

                isPickingUp = true;
            }
        }
    }

    void SwitchItem(int direction)
    {
        if (heldObject != null)
        {
            heldObject.SetActive(false);
        }

        activeItemIndex += direction;

        if (activeItemIndex >= heldItems.Count)
        {
            activeItemIndex = 0;
        }
        else if (activeItemIndex < 0)
        {
            activeItemIndex = heldItems.Count - 1;
        }

        heldObject = heldItems[activeItemIndex];
        heldRigidbody = heldObject.GetComponent<Rigidbody>();

        heldObject.SetActive(true);
        heldObject.transform.SetParent(holdPoint, true);

        isPickingUp = true;
    }

    void PickupLerp()
    {
        Transform obj = heldObject.transform;
        obj.localPosition = Vector3.Lerp(obj.localPosition,Vector3.zero, pickupSpeed * Time.deltaTime);
        obj.localRotation = Quaternion.Lerp(obj.localRotation, Quaternion.identity,pickupSpeed * Time.deltaTime);

        if (Vector3.Distance(obj.localPosition, Vector3.zero) < 0.01f && Quaternion.Angle(obj.localRotation, Quaternion.identity) < 1f)
        {
            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;

            isPickingUp = false;
        }
    }


    void ThrowObject()
    {
        Item item = heldObject.GetComponent<Item>();
        if (item != null)
        {
            item.Held(false);
        }

        heldItems.RemoveAt(activeItemIndex);
        heldObject.transform.SetParent(null);
        heldRigidbody.isKinematic = false;
        Collider thrownCollider = heldObject.GetComponent<Collider>();

        if (thrownCollider != null)
        {
            thrownCollider.enabled = false;
            StartCoroutine(ReEnableCollider(thrownCollider));
        }

        heldRigidbody.AddForce( playerCamera.transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
        heldRigidbody = null;
        isPickingUp = false;

        if (heldItems.Count == 0)
        {
            activeItemIndex = -1;
            return;
        }

        if (activeItemIndex >= heldItems.Count)
        {
            activeItemIndex = 0;
        }

        heldObject = heldItems[activeItemIndex];
        heldRigidbody = heldObject.GetComponent<Rigidbody>();

        heldObject.SetActive(true);
        heldObject.transform.SetParent(holdPoint, true);

        isPickingUp = true;
    }

    IEnumerator ReEnableCollider(Collider col)
    {
        yield return new WaitForSeconds(0.15f);
        col.enabled = true;
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        TryPickup();
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
        Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * pickupRange);

        if (holdPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(holdPoint.position, 0.15f);
            Gizmos.DrawLine(playerCamera.transform.position, holdPoint.position);
        }
    }
}