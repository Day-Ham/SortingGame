using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite normalCrosshair;
    [SerializeField] private Sprite oCrosshair;
    [SerializeField] private Sprite xCrosshair;
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

    [Header("Item Spawner")]
    ItemSpawner spawner;

    private void Start()
    {
        UpdateInventoryUI();

        spawner = FindAnyObjectByType<ItemSpawner>();
        if (spawner == null)
            Debug.Log("Item Spawner is not found");
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

            ItemChecker checker = lookedAtObject.GetComponent<ItemChecker>();
            if (checker != null)
            {
                Debug.Log("Looking at a shelf");
                if (checker.IsItemValid())
                {
                    crosshairImage.sprite = oCrosshair;
                }
                else
                {
                    crosshairImage.sprite = xCrosshair;
                }
                return;
            }

            Item item = lookedAtObject.GetComponent<Item>();

            if (item != null)
            {
                if (item.Data != null)
                {
                    itemNameText.text = item.Data.displayName;
                    itemNameText.gameObject.SetActive(true);
                }

                return;
            }
        }
        lookedAtObject = null;

        crosshairImage.sprite = normalCrosshair;

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

    public void RemoveItemFromPlayer()
    {
        if (heldObject == null) return;

        Item item = heldObject.GetComponent<Item>();
        if (item != null)
        {
            item.Held(false);
        }

        // Remove the active item from the inventory list
        heldItems.RemoveAt(activeItemIndex);

        // Clear local references
        heldObject = null;
        heldRigidbody = null;
        isPickingUp = false;

        heldItemNameText.text = "";

        // Handle inventory state after removal
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

    void ThrowObject()
    {
        if (heldObject == null) return;

        // Cache references before RemoveItemFromPlayer clears them
        GameObject objToThrow = heldObject;
        Rigidbody rbToThrow = heldRigidbody;

        // 1. Remove from inventory, update UI, and switch to next item
        RemoveItemFromPlayer();

        // 2. Apply throw-specific physics and parenting
        objToThrow.transform.SetParent(spawner != null ? spawner.transform : null);

        if (rbToThrow != null)
        {
            rbToThrow.isKinematic = false;
            rbToThrow.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
        }
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

        ItemChecker checker = lookedAtObject.GetComponent<ItemChecker>();
        if (checker != null)
        {
            Debug.Log("Trying to Place item");
            checker.PlaceItem();
            return;
        }

        Item item = lookedAtObject.GetComponent<Item>();
        if (item != null)
        {
            TryPickup(item);
            return;
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