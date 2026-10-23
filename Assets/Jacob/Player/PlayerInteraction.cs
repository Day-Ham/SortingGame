using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform backpackPoint;

    [Header("Look At")]
    [SerializeField] private GameObject lookedAtObject;
    public GameObject LookedAtObject => lookedAtObject;
    private GameObject lookAtPickUpObject;
    private GameObject lookAtInteractObject;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float pickupSpeed = 12f;
    public GameObject heldObject;
    private Rigidbody heldRigidbody;
    private int activeItemIndex = -1;
    public bool isPickingUp;
    private bool playPickUpSFX;

    [Header("Outline Settings")]
    [SerializeField] private string outlineLayerName = "Outline";
    private int outlineLayer;
    private GameObject previousLookedAtObject;
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    [Header("Backpack")]
    [SerializeField] private int maxHeldItems = 3;
    [SerializeField] private float switchDelay = 0.25f;
    public List<GameObject> heldItems = new List<GameObject>();
    public bool isSwitching;
    public int MaxHeldItems => maxHeldItems;

    [Header("Throw")]
    [SerializeField] private float throwForce = 5f;    

    [Header("Item Spawner")]
    ItemSpawner spawner;

    [Header("Robot Buddy")]
    [SerializeField] private GameObject robotBuddyShelfPromp;

    GameUIManager ui;

    [Header("Target Layers")]
    [SerializeField] private LayerMask throwTargetLayers;

    private void Start()
    {
        ui = GameUIManager.instance;

        outlineLayer = LayerMask.NameToLayer(outlineLayerName);

        UpdateInventoryUI();

        spawner = FindAnyObjectByType<ItemSpawner>();
        if (spawner == null)
            Debug.Log("Item Spawner is not found");
    }

    void Update()
    {
        LookAtObjects();
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
    void LookAtObjects()
    {
        //Look at all objects in a straight line belonging to pick up, interact, and obstacle layers
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, pickupRange, pickUpLayer | interactLayer | obstacleLayer);
        
        //Arrange from nearest to farthest
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        GameObject pickUpObject = null;
        GameObject interactObject = null;

        foreach (RaycastHit hit in hits)
        {
            GameObject obj = hit.collider.gameObject;

            //Don't detect anything if nearest object is an obstacle
            if (IsInLayerMask(obj, obstacleLayer))
            {
                break;
            }

            //Nearest pick upable and interactable objects found, stop checking other hits
            if (pickUpObject != null && interactObject != null) break;

            //Detect the nearest pick upable object
            if (pickUpObject == null && IsInLayerMask(obj, pickUpLayer))
            {
                pickUpObject = obj;
                Debug.Log("Pickup object: " + obj.name);
            }

            //Detect the nearest interactable object
            if (interactObject == null && IsInLayerMask(obj, interactLayer))
            {
                interactObject = obj;
                Debug.Log("Interact object: " + obj.name);
            }
        }

        //refresh lookAtPickUpObject and lookAtInteractObject
        if (pickUpObject != null)
        {
            lookAtPickUpObject = pickUpObject;
        }
        else
        {
            lookAtPickUpObject = null;
        }

        if (interactObject != null)
        {
            lookAtInteractObject = interactObject;
        }
        else
        {
            lookAtInteractObject = null;
        }

        if (lookAtPickUpObject == null && lookAtInteractObject == null)
        {
            lookedAtObject = null;
        }

        SetCrosshair();
        OutlinePickUpObject();
        DisplayNameOfPickUpObject();
    }

    void DisplayNameOfPickUpObject()
    {
        if (lookAtPickUpObject == null)
        {
            ui.SetUIText(ui.lookedAtItemNameText, "");
            ui.DisableUI(ui.lookedAtItemNameText.gameObject);
            return;
        }

        // display item name
        Item item = lookAtPickUpObject.GetComponent<Item>();
        if (item != null && item.Data != null)
        {
            ui.SetUIText(ui.lookedAtItemNameText, item.Data.displayName);
            ui.EnableUI(ui.lookedAtItemNameText.gameObject);
            return;
        }
    }

    void SetCrosshair()
    {
        // no object found
        if (lookAtPickUpObject == null && lookAtInteractObject == null)
        {
            ui.SetCrosshair(ui.normalCrosshair);

            UpdateOutlineTarget(null);
            return;
        }

        //check the shelf in ItemChecker
        if (lookAtInteractObject != null)
        {
            ItemChecker checker = lookAtInteractObject.GetComponent<ItemChecker>();
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
            }
        }
        else
        {
            ui.SetCrosshair(ui.normalCrosshair);
        }
    }

    void UpdateInventoryUI()
    {
        if (ui == null) return;
        ui.SetUIText(ui.amountHeldText, heldItems.Count.ToString());
        ui.SetUIText(ui.maxHeldItemsText, "/ " + maxHeldItems.ToString());

        // No items
        if (heldItems.Count == 0)
        {
            ui.SetUIText(ui.heldItemNameText, "");

            for (int i = 0; i < ui.backpackItemNames.Length; i++)
            {
                ui.SetUIText(ui.backpackItemNames[i], "");
            }

            return;
        }

        // Current held item
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

        // Other backpack items, starting after the active item
        for (int i = 1; i < heldItems.Count; i++)
        {
            int index = (activeItemIndex + i) % heldItems.Count;

            GameObject itemObject = heldItems[index];

            if (itemObject == null) continue;

            Item item = itemObject.GetComponent<Item>();

            if (item != null && item.Data != null && i - 1 < ui.backpackItemNames.Length)
            {
                ui.SetUIText(ui.backpackItemNames[i - 1], item.Data.displayName);
            }
        }

        // Clear unused backpack UI slots
        int displayedItems = Mathf.Max(0, Mathf.Min(heldItems.Count - 1, ui.backpackItemNames.Length));
        for (int i = displayedItems; i < ui.backpackItemNames.Length; i++)
        {
            ui.SetUIText(ui.backpackItemNames[i], "");
        }
    }
    #endregion

    #region Outline
    void OutlinePickUpObject()
    {
        //apply outline only to pickupable items that are not on a shelf
        if (lookAtPickUpObject == null) return;

        Item item = lookAtPickUpObject.GetComponent<Item>();

        if (item != null && item.GetComponentInParent<ItemChecker>() == null)
        {
            UpdateOutlineTarget(lookAtPickUpObject);
        }
        else
        {
            UpdateOutlineTarget(null);
        }
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

    #endregion

    #region Pick Up
    void LookAtPickUp()
    {
        // prioritize pickup layer over interact layer, find last object put on the shelf
        if (lookAtPickUpObject != null && lookAtInteractObject != null)
        {
            ItemChecker shelfParent = lookAtPickUpObject.GetComponentInParent<ItemChecker>();
            if (shelfParent == null)
            {
                lookedAtObject = lookAtPickUpObject;
            }
            else //if the item has a shelf parent, set pick up object as the last object placed on the shelf
            {
                ItemChecker shelf = lookAtInteractObject.GetComponent<ItemChecker>();
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
        }
        else if (lookAtPickUpObject != null)
        {
            lookedAtObject = lookAtPickUpObject;
        }
    }

    public void OnInteract()
    {
        if (UserInput.instance == null) return;

        if (!UserInput.instance.InteractionInput)
            return;

        if (isPickingUp || isSwitching) return;

        LookAtPickUp();

        if (lookedAtObject == null) return;

        if (heldItems.Count >= maxHeldItems) return;

        Item item = lookedAtObject.GetComponent<Item>();

        if (item == null) return;
        if (heldItems.Contains(item.gameObject)) return;

        ItemChecker shelf = item.GetComponentInParent<ItemChecker>();
        if (shelf != null)
        {
            if (shelf.isPlacingItem) return;
            shelf.RemoveItem();
        }

        TryPickup(item);
    }

    public void TryPickup(Item item)
    {
        if (item == null || isPickingUp || isSwitching) return;
        
        GameObject obj = item.gameObject;
        if (heldItems.Contains(obj)) return;
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) return;

        isPickingUp = true;
        
        UpdateOutlineTarget(null);

        if (heldObject != null)
        {
            heldObject.SetActive(false);
        }

        heldObject = obj;
        heldRigidbody = rb;

        heldItems.Add(obj);
        activeItemIndex = heldItems.Count - 1;
        
        UpdateInventoryUI();

        heldRigidbody.isKinematic = true;
        
        item.Held(true, transform);
        heldObject.transform.SetParent(holdPoint, true);

        playPickUpSFX = true;
    }

    void PickupLerp()
    {
        //play pick up audio
        if (playPickUpSFX)
        {
            AudioSource audioSource = heldObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                Item item = heldObject.GetComponent<Item>();
                if (item != null && item.Data != null)
                {
                    audioSource.clip = item.Data.pickUpSFX;
                    audioSource.Play();
                    playPickUpSFX = false;
                }
            }
        }
        
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
            playPickUpSFX = false;
        }
    }
    #endregion

    #region Backpack
    public void OnScrollUp()
    {
        if (UserInput.instance == null) return;

        if (!UserInput.instance.ScrollUpInput)
            return;

        if (heldItems.Count > 1)
        {
            SwitchItem(1);
        }
    }

    public void OnScrollDown()
    {
        if (UserInput.instance == null) return;

        if (!UserInput.instance.ScrollDownInput)
            return;

        if (heldItems.Count > 1)
        {
            SwitchItem(-1);
        }
    }

    void SwitchItem(int direction)
    {
        if (isSwitching || isPickingUp) return;

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

        heldObject.transform.SetParent(backpackPoint, false);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        playPickUpSFX = false;
        isPickingUp = true;
        isSwitching = false;

        UpdateInventoryUI();  
    }
    #endregion

    #region Throw
    void LookAtInteract()
    {
        // prioritize interact layer over pick up layer
        if (lookAtInteractObject != null)
        {
            lookedAtObject = lookAtInteractObject;
        }
    }

    public void OnThrow()
    {
        if (UserInput.instance == null) return;

        if (!UserInput.instance.ThrowInput) return;

        if (isPickingUp || isSwitching) return;

        LookAtInteract();

        ItemChecker checker = lookedAtObject != null ? lookedAtObject.GetComponent<ItemChecker>() : null;

        if (checker != null && heldObject != null)
        {
            Debug.Log("Trying to Place item");
            if (checker.isAssignedToBuddy)
            {
                robotBuddyShelfPromp.SetActive(true);
                return;
            }

            if (checker.isPlacingItem) return;
            
            Debug.Log("NOW PLACING THE ITEM");
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
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f) );

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, throwTargetLayers))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(100f);
            }

            Vector3 throwDirection = (targetPoint - objToThrow.transform.position).normalized;
            rbToThrow.AddForce(throwDirection * throwForce,ForceMode.Impulse);
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
            itemToRemove.transform.SetParent(null, true);

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

    public void CleanAndReorganize()
    {
        Debug.Log($"{name} has {holdPoint.childCount} children before cleaning.");
        heldItems.Clear();

        for (int i = 0; i < holdPoint.childCount; i++)
        {
            Transform child = holdPoint.GetChild(i);
            Item item = child.GetComponent<Item>();

            if (item != null)
            {
                heldItems.Add(item.gameObject);

                Rigidbody rb = item.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                item.transform.position = holdPoint.transform.position;
                item.transform.rotation = holdPoint.transform.rotation;
                item.Held(true, transform);
                item.gameObject.SetActive(false);
            }

            if (i == holdPoint.childCount-1)
            {
                item.gameObject.SetActive(true);
                heldObject = item.gameObject;
            }
        }

        UpdateInventoryUI();
    }

    private void OnDrawGizmos()
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