using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform backpackPoint;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float pickupSpeed = 12f;

    [Header("Backpack")]
    [SerializeField] private int maxHeldItems = 3;
    [SerializeField] private float switchDelay = 0.25f;

    [Header("Throw")]
    [SerializeField] private float throwForce = 5f;

    [Header("UI")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text amountHeldText;
    [SerializeField] private TMP_Text heldItemNameText;
    [SerializeField] private TMP_Text[] backpackItemNames;
    [SerializeField] private GameObject lookedAtObject;

    public GameObject heldObject;
    private Rigidbody heldRigidbody;

    public List<GameObject> heldItems = new List<GameObject>();
    private int activeItemIndex = -1;

    private bool isPickingUp;
    private bool isSwitching;

    private Item lookedAtItem;
    

    private void Start()
    {
        UpdateInventoryUI();
    }

    void Update()
    {
        CheckLookedAtObject();

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

    void CheckLookedAtObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, interactLayer))
        {
            lookedAtObject = hit.collider.gameObject;

            Item item = hit.collider.GetComponent<Item>();

            if (item != null)
            {
                lookedAtItem = item;

                if (item.Data != null)
                {
                    itemNameText.text = item.Data.displayName;
                    itemNameText.gameObject.SetActive(true);
                }

                return;
            }
        }

        lookedAtObject = null;
        lookedAtItem = null;

        itemNameText.text = "";
        itemNameText.gameObject.SetActive(false);
    }

    void TryPickup(Item item)
    {
        GameObject obj = item.gameObject;
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null && item != null)
        {

            if (heldItems.Count >= maxHeldItems)
            {
                return;

            }

            if (heldObject != null)
            {
                heldObject.SetActive(false);
            }

            heldObject = rb.gameObject;
            heldRigidbody = rb;

            heldItems.Add(heldObject);
            activeItemIndex = heldItems.Count - 1;
            UpdateInventoryUI();

            heldRigidbody.isKinematic = true;
            item.Held(true);
            heldObject.transform.SetParent(holdPoint, true);

            isPickingUp = true;
        }

    }

    void SwitchItem(int direction)
    {
        if (isSwitching)
            return;

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

        StartCoroutine(ShowNextItem());
    }

    void PickupLerp()
    {
        Transform obj = heldObject.transform;

        obj.position = Vector3.Lerp(obj.position, holdPoint.position, pickupSpeed * Time.deltaTime);
        obj.rotation = Quaternion.Lerp(obj.rotation, holdPoint.rotation, pickupSpeed * Time.deltaTime);

        if (Vector3.Distance(obj.position, holdPoint.position) < 0.01f && Quaternion.Angle(obj.rotation, holdPoint.rotation) < 1f)
        {
            obj.position = holdPoint.position;
            obj.rotation = holdPoint.rotation;

            obj.SetParent(holdPoint, true);

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
        heldRigidbody.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
        heldRigidbody = null;
        isPickingUp = false;

        heldItemNameText.text = "";

        if (heldItems.Count == 0)
        {
            activeItemIndex = -1;
            UpdateInventoryUI();
            return;
        }

        if (activeItemIndex >= heldItems.Count)
        {
            activeItemIndex = 0;
        }

        StartCoroutine(ShowNextItem());
    }

    IEnumerator ShowNextItem()
    {
        isSwitching = true;

        yield return new WaitForSeconds(switchDelay);

        heldObject = heldItems[activeItemIndex];
        heldRigidbody = heldObject.GetComponent<Rigidbody>();

        heldObject.SetActive(true);

        UpdateInventoryUI();

        heldObject.transform.SetParent(backpackPoint, false);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        UpdateInventoryUI();

        isPickingUp = true;

        isSwitching = false;
    }

    void UpdateInventoryUI()
    {
        amountHeldText.text = heldItems.Count.ToString();

        if (heldObject != null)
        {
            Item heldItem = heldObject.GetComponent<Item>();

            if (heldItem != null && heldItem.Data != null)
            {
                heldItemNameText.text = heldItem.Data.displayName;
            }
            else
            {
                heldItemNameText.text = "";
            }
        }
        else
        {
            heldItemNameText.text = "";
        }

        int backpackIndex = 0;

        for (int i = 0; i < heldItems.Count; i++)
        {
            if (heldItems[i] == heldObject)
                continue;

            Item item = heldItems[i].GetComponent<Item>();

            if (backpackIndex < backpackItemNames.Length)
            {
                if (item != null && item.Data != null)
                {
                    backpackItemNames[backpackIndex].text = item.Data.displayName;
                }
                else
                {
                    backpackItemNames[backpackIndex].text = "";
                }

                backpackIndex++;
            }
        }

        for (int i = backpackIndex; i < backpackItemNames.Length; i++)
        {
            backpackItemNames[i].text = "";
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (lookedAtObject == null)
            return;

        Item item = lookedAtObject.GetComponent<Item>();
        if (item != null)
        {
            TryPickup(item);
            return;
        }

        ItemChecker checker = lookedAtObject.GetComponent<ItemChecker>();
        if(checker != null)
        {
            //Blah blah blah
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
        Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * pickupRange);

        if (holdPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(holdPoint.position, 0.15f);
            Gizmos.DrawLine(playerCamera.transform.position, holdPoint.position);
        }
    }
}