using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction scrollUpAction;
    InputAction scrollDownAction;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform backpackPoint;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private float pickupSpeed = 12f;

    [Header("Outline Settings")]
    [SerializeField] private string outlineLayerName = "Outline";
    private int outlineLayer;
    private GameObject previousLookedAtObject;
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

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
    [SerializeField] private TMP_Text maxHeldItemsText;
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
        playerInput = GetComponent<PlayerInput>();
        scrollUpAction = playerInput.actions.FindAction("ScrollUp");
        scrollDownAction = playerInput.actions.FindAction("ScrollDown");
        outlineLayer = LayerMask.NameToLayer(outlineLayerName);

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
    }

    public void OnScrollUp(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (heldItems.Count > 1)
        {
            SwitchItem(1);
        }
    }

    public void OnScrollDown(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (heldItems.Count > 1)
        {
            SwitchItem(-1);
        }
    }

    void CheckLookedAtObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, pickupRange, pickUpLayer | interactLayer);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        GameObject pickupObject = null;
        GameObject interactObject = null;

        foreach (RaycastHit hit in hits)
        {
            GameObject obj = hit.collider.gameObject;

            if (pickupObject == null && IsInLayerMask(obj, pickUpLayer))
            {
                pickupObject = obj;
            }

            if (interactObject == null && IsInLayerMask(obj, interactLayer))
            {
                interactObject = obj;
            }
        }

        // prio pickup layer over interact layer, find last object put on the shelf
        if (pickupObject != null && interactObject != null)
        {
            lookedAtObject = interactObject;
            ItemChecker shelf = lookedAtObject.GetComponent<ItemChecker>();
            if (shelf != null)
            {
                Item lastItem = shelf.GetLastObject();
                if (lastItem != null)
                {
                    lookedAtObject = lastItem.gameObject;
                    Debug.Log("Looking at " + lookedAtObject.name);
                }
            }
        }
        else if (pickupObject != null)
        {
            lookedAtObject = pickupObject;
        }
        else if (interactObject != null)
        {
            lookedAtObject = interactObject;
        }
        else
        {
            lookedAtObject = null;

            crosshairImage.sprite = normalCrosshair;

            itemNameText.text = "";
            itemNameText.gameObject.SetActive(false);

            UpdateOutlineTarget(null);
            return;
        }

        //apply outline only to pickupable items that are not on a shelf
        Item item = lookedAtObject.GetComponent<Item>();

        if (item != null && item.GetComponentInParent<ItemChecker>() == null)
        {
            UpdateOutlineTarget(lookedAtObject);
        }
        else
        {
            UpdateOutlineTarget(null);
        }

        //check ItemChecker
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

            itemNameText.text = "";
            itemNameText.gameObject.SetActive(false);

            return;
        }

        // Check Item UI
        if (item != null && item.Data != null)
        {
            itemNameText.text = item.Data.displayName;
            itemNameText.gameObject.SetActive(true);

            return;
        }

        crosshairImage.sprite = normalCrosshair;

        itemNameText.text = "";
        itemNameText.gameObject.SetActive(false);
    }

    private void UpdateOutlineTarget(GameObject newTarget)
    {
        if (previousLookedAtObject == newTarget)
            return;

        if (previousLookedAtObject != null)
        {
            RestoreLayer(previousLookedAtObject);
        }

        if (newTarget != null && outlineLayer != -1)
        {
            SetOutlineLayer(newTarget);
        }

        previousLookedAtObject = newTarget;
    }

    private void SetOutlineLayer(GameObject obj)
    {
        if (!originalLayers.ContainsKey(obj))
        {
            originalLayers[obj] = obj.layer;
        }

        obj.layer = outlineLayer;

        foreach (Transform child in obj.transform)
        {
            SetOutlineLayer(child.gameObject);
        }
    }

    private void RestoreLayer(GameObject obj)
    {
        if (originalLayers.TryGetValue(obj, out int originalLayer))
        {
            obj.layer = originalLayer;
            originalLayers.Remove(obj);
        }

        foreach (Transform child in obj.transform)
        {
            RestoreLayer(child.gameObject);
        }
    }

    bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        if (originalLayers.TryGetValue(obj, out int originalLayer))
        {
            return (layerMask.value & (1 << originalLayer)) != 0;
        }
        return (layerMask.value & (1 << obj.layer)) != 0;
    }

    void TryPickup(Item item)
    {
        GameObject obj = item.gameObject;
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null && item != null)
        {
            UpdateOutlineTarget(null);

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

        heldItems.RemoveAt(activeItemIndex);

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

    void ThrowObject()
    {
        if (heldObject == null) return;

        GameObject objToThrow = heldObject;
        Rigidbody rbToThrow = heldRigidbody;

        RemoveItemFromPlayer();

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
        maxHeldItemsText.text = "/ " + maxHeldItems.ToString();

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

        if (heldItems.Count >= maxHeldItems) return;

        Item item = lookedAtObject.GetComponent<Item>();
        if (item != null)
        {
            ItemChecker shelf = item.GetComponentInParent<ItemChecker>();
            if (shelf != null)
            {
                shelf.RemoveItem();
            }

            TryPickup(item);
            return;
        }
    }

    public void OnThrow(InputValue value)
    {
        if (!value.isPressed)
            return;

        ItemChecker checker = lookedAtObject != null ? lookedAtObject.GetComponent<ItemChecker>() : null;

        if (checker != null && heldObject != null)
        {
            Debug.Log("Trying to Place item");
            checker.PlaceItem();
            return;
        }

        if (heldObject != null)
        {
            ThrowObject();
        }
    }

    public void UpgradeCapacity(int amount)
    {
        maxHeldItems += amount;
        UpdateInventoryUI();
    }

    public void UpgradeReach(float amount)
    {
        pickupRange += amount;
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