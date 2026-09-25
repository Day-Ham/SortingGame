using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform backpackPoint;

    [Header("Look At")]
    [SerializeField] private GameObject lookedAtObject;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private float pickupSpeed = 12f;
    public GameObject heldObject;
    private Rigidbody heldRigidbody;
    private int activeItemIndex = -1;
    private bool isPickingUp;

    [Header("Outline Settings")]
    [SerializeField] private string outlineLayerName = "Outline";
    private int outlineLayer;
    private GameObject previousLookedAtObject;
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    [Header("Backpack")]
    [SerializeField] private int maxHeldItems = 3;
    [SerializeField] private float switchDelay = 0.25f;
    public List<GameObject> heldItems = new List<GameObject>();
    private bool isSwitching;
    public int MaxHeldItems => maxHeldItems;

    [Header("Throw")]
    [SerializeField] private float throwForce = 5f;    

    [Header("Item Spawner")]
    ItemSpawner spawner;

    UIManager ui;

    private void Start()
    {
        ui = UIManager.instance;

        outlineLayer = LayerMask.NameToLayer(outlineLayerName);

        UpdateInventoryUI();

        spawner = FindAnyObjectByType<ItemSpawner>();
        if (spawner == null)
            Debug.Log("Item Spawner is not found");
    }

    void Update()
    {
        CheckLookedAtObject();
        OnScrollUp();
        OnScrollDown();
        OnInteract();
        OnThrow();

        if (heldObject != null && isPickingUp)
        {
            PickupLerp();
        }
    }

    #region General
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

            ui.SetCrosshair(ui.normalCrosshair);

            ui.SetUIText(ui.lookedAtItemNameText, "");
            ui.DisableUI(ui.lookedAtItemNameText.gameObject);

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

        //check the shelf in ItemChecker
        ItemChecker checker = lookedAtObject.GetComponent<ItemChecker>();

        if (checker != null)
        {
            Debug.Log("Looking at a shelf");
            
            //check if item is viable to be put on the shelf and change crosshairs to match
            if (checker.IsItemValid())
            {
                ui.SetCrosshair(ui.oCrosshair);
            }
            else
            {
                ui.SetCrosshair(ui.xCrosshair);
            }

            ui.SetUIText(ui.lookedAtItemNameText, "");
            ui.DisableUI(ui.lookedAtItemNameText.gameObject);

            return;
        }

        // Check Item UI
        if (item != null && item.Data != null)
        {
            ui.SetUIText(ui.lookedAtItemNameText, item.Data.displayName);
            ui.EnableUI(ui.lookedAtItemNameText.gameObject);
            return;
        }

        ui.SetCrosshair(ui.normalCrosshair);

        ui.SetUIText(ui.lookedAtItemNameText, "");
        ui.DisableUI(ui.lookedAtItemNameText.gameObject);
    }

    void UpdateInventoryUI()
    {
        ui.SetUIText(ui.amountHeldText, heldItems.Count.ToString());
        ui.SetUIText(ui.maxHeldItemsText, "/ " + maxHeldItems.ToString());

        if (heldObject != null)
        {
            Item heldItem = heldObject.GetComponent<Item>();

            if (heldItem != null && heldItem.Data != null)
            {
                ui.SetUIText(ui.heldItemNameText, heldItem.Data.displayName);
            }
            else
            {
                ui.SetUIText(ui.heldItemNameText, "");
            }
        }
        else
        {
            ui.SetUIText(ui.heldItemNameText, "");
        }

        int backpackIndex = 0;

        for (int i = 0; i < heldItems.Count; i++)
        {
            if (heldItems[i] == heldObject)
                continue;

            Item item = heldItems[i].GetComponent<Item>();

            if (backpackIndex < ui.backpackItemNames.Length)
            {
                if (item != null && item.Data != null)
                {
                    ui.SetUIText(ui.backpackItemNames[backpackIndex], item.Data.displayName);
                }
                else
                {
                    ui.SetUIText(ui.backpackItemNames[backpackIndex], item.Data.displayName);
                }

                backpackIndex++;
            }
        }

        for (int i = backpackIndex; i < ui.backpackItemNames.Length; i++)
        {
            ui.SetUIText(ui.backpackItemNames[i], "");
        }
    }
    #endregion

    #region Outline
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

    #endregion

    #region Pick Up
    public void OnInteract()
    {
        if (!UserInput.instance.InteractionInput)
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

    public void TryPickup(Item item)
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
    #endregion

    #region Backpack
    public void OnScrollUp()
    {
        if (!UserInput.instance.ScrollUpInput)
            return;

        if (heldItems.Count > 1)
        {
            SwitchItem(1);
        }
    }

    public void OnScrollDown()
    {
        if (!UserInput.instance.ScrollDownInput)
            return;

        if (heldItems.Count > 1)
        {
            SwitchItem(-1);
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
    #endregion

    #region Throw
    public void OnThrow()
    {
        if (!UserInput.instance.ThrowInput)
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

    public void RemoveItemFromPlayer(GameObject itemToRemove = null)
    {
        if (heldItems.Count == 0)
            return;

        if (itemToRemove == null)
        {
            itemToRemove = heldObject;
        }

        if (itemToRemove == null)
            return;

        int removeIndex = heldItems.IndexOf(itemToRemove);

        if (removeIndex == -1)
            return;

        Item item = itemToRemove.GetComponent<Item>();

        if (item != null)
        {
            item.Held(false);
        }

        bool wasActiveItem = itemToRemove == heldObject;

        heldItems.RemoveAt(removeIndex);

        if (wasActiveItem)
        {
            heldObject = null;
            heldRigidbody = null;
            isPickingUp = false;

            ui.SetUIText(ui.heldItemNameText, "");

            if (heldItems.Count == 0)
            {
                activeItemIndex = -1;
                UpdateInventoryUI();
                return;
            }

            if (removeIndex >= heldItems.Count)
            {
                activeItemIndex = 0;
            }
            else
            {
                activeItemIndex = removeIndex;
            }

            StartCoroutine(ShowNextItem());
        }
        else
        {
            if (removeIndex < activeItemIndex)
            {
                activeItemIndex--;
            }

            UpdateInventoryUI();
        }
    }
    #endregion

    #region Upgrades
    public void UpgradeCapacity(int amount)
    {
        maxHeldItems += amount;
        UpdateInventoryUI();
    }

    public void UpgradeReach(float amount)
    {
        pickupRange += amount;
    }
    #endregion

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