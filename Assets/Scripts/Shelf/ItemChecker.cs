using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemChecker : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private ShelfType shelfType;
    [SerializeField] private BoxCollider shelfArea;
    private List<Vector3> shelfPositions = new List<Vector3>();

    [Header("Items")]
    public List<Item> heldItems = new List<Item>();
    [SerializeField] private string displayName;
    public Item currentItem /*{ get; private set; }*/;
    [SerializeField] private int maxItems;
    [SerializeField] private Vector3 orientation;

    [Header("UI")]
    [SerializeField] private TMP_Text displayNameTxt;
    [SerializeField] private Image background;

    [Header("Player")]
    PlayerInteraction player;
    PlayerUpgrade playerUpgrade;

    [Header("Item Spawner")]
    ItemSpawner spawner;

    [Header("Currency Manager")]
    CurrencyManager currency;

    [Header("Placing Parameters")]
    public float moveSpeed = 30f;
    public bool isPlacingItem;

    [Header("Robot Buddy")]
    public bool isAssignedToBuddy;
    public List<RobotBuddy> robotList;

    private GameUIManager ui;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerInteraction>();
        playerUpgrade = FindAnyObjectByType<PlayerUpgrade>();
       
        if (player == null)
            Debug.Log("Player is not found");

        spawner = FindAnyObjectByType<ItemSpawner>();
        if (spawner == null)
            Debug.Log("Item Spawner is not found");

        currency = FindAnyObjectByType<CurrencyManager>();
        if (currency == null)
            Debug.Log("Currency Manager is not found");
    }

    private void Start()
    {
        ui = GameUIManager.instance;
    }

    private void Update()
    {
        UpdateUI();

        if (isPlacingItem)
        {
            MoveToPlaceItem();
        }
    }

    public bool IsItemValid(Item item = null)
    {
        //is the item valid?

        if (item == null)
        {
            if (player.heldObject == null) return false;

            item = player.heldObject.GetComponent<Item>();
        }

        if (item == null)
        {
            Debug.Log("Held item does not have the Item script attached");
            return false;
        }
        
        //is the item type the same as the container type?
        if (item.Data.type != itemType)
        {
            Debug.Log("Held item not same type as container type");
            return false;
        }

        //is the container has items
        if (heldItems.Count > 0)
        {
            //check if container display name is the same as held item display name
            if (item.Data.displayName == displayName)
            {
                return true;
            }
        }
        else if (heldItems.Count == 0)
        {
            return true;
        }
        return false;
    }

    private void ComputeItemPlacements()
    {
        // Item Data
        orientation = currentItem.Data.preferredOrientation; 
        
        int rows = currentItem.Data.rows;
        int cols = currentItem.Data.columns;
        Vector3 spacing = currentItem.Data.spacing;

        BoxCollider box = shelfArea as BoxCollider;
        Vector3 localCenter = box != null ? box.center : Vector3.zero;
        Vector3 localSize = box != null ? box.size : shelfArea.bounds.size;

        Vector3 localBotCenter = new Vector3(localCenter.x, localCenter.y - (localSize.y / 2f), localCenter.z);
        Vector3 localBackCenter = new Vector3(localCenter.x, localCenter.y, localCenter.z - (localSize.z / 2f));

        var offsets = GetMeshOffsets(currentItem);

        float bottomOffset = offsets.bottomOffset;
        float backOffset = offsets.backOffset;

        //Computation of placements
        for (int k = 0; k < maxItems; k++)
        {
            int r;
            int c;

            float x = 0;
            float y = 0;
            float z = 0;

            if (shelfType == ShelfType.Horizontal)
            {
                c = cols - 1 - (k % cols);
                r = k / cols;

                float width = (cols - 1) * spacing.x;
                float length = (rows - 1) * spacing.z;

                x = (localBotCenter.x - (width / 2)) + (spacing.x * c);
                z = (localBotCenter.z - (length / 2)) + (spacing.z * r);
                y = localBotCenter.y - bottomOffset;
            }
            else if (shelfType == ShelfType.Vertical)
            {
                c = cols - 1 - (k % cols);
                r = k / cols;

                float width = (cols - 1) * spacing.x;
                float height = rows * spacing.z;

                x = (localBackCenter.x - (width / 2)) + (spacing.x * c);
                y = (localBackCenter.y + (height / 2)) - (spacing.z * r);
                z = localBackCenter.z - backOffset;
            }
            else if (shelfType == ShelfType.Slanted)
            {
                r = (rows - 1) - (k / cols);
                c = cols - 1 - (k % cols);

                float width = (cols - 1) * spacing.x;
                float length = (rows - 1) * spacing.z;

                x = (localBotCenter.x - (width / 2)) + (spacing.x * c);
                y = localBotCenter.y - bottomOffset + (spacing.y * r);
                z = (localBotCenter.z + (length / 2)) - (spacing.z * r);
            }

            Vector3 localTargetPoint = new Vector3(x, y, z);
            shelfPositions.Add(localTargetPoint);
        }
    }

    private (float bottomOffset, float backOffset) GetMeshOffsets(Item item)
    {
        MeshFilter meshFilter = item.GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogWarning($"No MeshCollider or mesh found on {item.name}.");
            return (item.GetComponent<Collider>().bounds.size.y, item.GetComponent<Collider>().bounds.size.z);
        }

        Bounds meshBounds = meshFilter.sharedMesh.bounds;

        Vector3 scale = meshFilter.transform.lossyScale;
        Vector3 center = Vector3.Scale(meshBounds.center, scale);
        Vector3 size = Vector3.Scale(meshBounds.size, scale);

        Quaternion rotation = Quaternion.Euler(item.Data.preferredOrientation);

        Vector3 rotatedCenter = rotation * center;

        Vector3 right = rotation * Vector3.right;
        Vector3 up = rotation * Vector3.up;
        Vector3 forward = rotation * Vector3.forward;

        float halfHeight = (Mathf.Abs(right.y) * size.x + Mathf.Abs(up.y) * size.y + Mathf.Abs(forward.y) * size.z) * 0.5f;
        float halfDepth = (Mathf.Abs(right.z) * size.x + Mathf.Abs(up.z) * size.y + Mathf.Abs(forward.z) * size.z) * 0.5f;

        float bottomOffset = rotatedCenter.y - halfHeight;
        float backOffset = rotatedCenter.z - halfDepth;

        return (bottomOffset, backOffset);
    }

    public void PlaceItem(Item item = null)
    {
        if (isPlacingItem) return;

        if (item == null)
        {
            if (!IsItemValid())
            {
                Debug.Log("Item cannot be placed on the shelf.");
                currentItem = null;
                return;
            }

            currentItem = player.heldObject.GetComponent<Item>();
        }
        else
        {
            currentItem = item;
        }

        if (currentItem == null) return;

        if (heldItems.Contains(currentItem)) return; //make sure no duplicates

        isPlacingItem = true;

        currentItem.gameObject.layer = 2; //can't be interacted with while being placed

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        currentItem.gameObject.SetActive(true);

        //Change container information if it is empty
        if (heldItems.Count == 0)
        {
            displayName = currentItem.Data.displayName;
            SetMaxItems();
            ComputeItemPlacements();
        }

        //take ownership from player
        if (item == null)
        {
            player.RemoveItemFromPlayer(currentItem.gameObject);
        }

        currentItem.transform.SetParent(transform);

        heldItems.Add(currentItem);

        currentItem.isBeingPlaced = true;
    }

    void MoveToPlaceItem()
    {
        if (currentItem == null)
        {
            isPlacingItem = false;
            return;
        }

        int itemIndex = heldItems.Count - 1;
        if (itemIndex < 0 || itemIndex >= shelfPositions.Count)
        {
            isPlacingItem = false;
            return;
        }

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        currentItem.Held(true, transform);

        Vector3 targetPosition = shelfPositions[itemIndex];
        Quaternion targetRotation = Quaternion.Euler(orientation);

        Vector3 newPosition = Vector3.Lerp(rb.position, transform.TransformPoint(targetPosition), moveSpeed * Time.fixedDeltaTime);
        Quaternion newRotation = Quaternion.Lerp(rb.rotation, transform.rotation * targetRotation, moveSpeed * Time.fixedDeltaTime);

        rb.Move(newPosition, newRotation);

        if (Vector3.Distance(rb.position, transform.TransformPoint(targetPosition)) < 0.01f && Quaternion.Angle(rb.rotation, transform.rotation * targetRotation) < 1f)
        {
            //finalize position
            rb.Move(transform.TransformPoint(targetPosition), transform.rotation * targetRotation);

            Collider[] colliders = currentItem.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = true;
            }

            UpdateUI();

            bool shelfComplete = heldItems.Count == maxItems;
            if (shelfComplete)
            {
                OnShelfComple();
            }
            else
            {
                background.color = Color.white;
                currentItem.gameObject.layer = LayerMask.NameToLayer("Pickup");
            }
                        
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play("Put Down");

            rb.isKinematic = true;

            isPlacingItem = false;
            currentItem.isBeingPlaced = false;

            AddUpgradePoints(); //adds money if first time being placed on shelf
            ui.UpdateShelvedCount(1);
            Debug.Log("Placed " + currentItem.name + " on the shelf.");
            currentItem = null;
        }
    }

    public void RemoveItem()
    {
        if (heldItems.Count == 0) return;

        Item item = heldItems[heldItems.Count - 1];
        heldItems.RemoveAt(heldItems.Count - 1);
        ui.UpdateShelvedCount(-1);

        background.color = Color.white;

        if (heldItems.Count == 0)
        {
            maxItems = 0;
            displayName = "";
            shelfPositions.Clear();
        }

        UpdateUI();
    }

    public Item GetLastObject()
    {
        if (heldItems.Count > 0)
        {
            Item removedItem = heldItems[heldItems.Count - 1];
            return removedItem;
        }
        return null;
    }

    private void SetMaxItems()
    {
        if (spawner.items.Count == 0) return;

        foreach (ItemSpawnEntry entry in spawner.items)
        {
            Item itemInfo = entry.item.GetComponent<Item>();
            if (itemInfo == null) continue;

            if (itemInfo.Data.displayName == displayName)
            {
                maxItems = itemInfo.Data.rows * itemInfo.Data.columns;
                return;
            }
        }
    }

    private void OnShelfComple()
    {
        background.color = Color.green;
        PulseCompletedItems();

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play("Complete");

        if (spawner.items.Count == 0) return;

        foreach (ItemSpawnEntry item in spawner.items)
        {
            Item itemInfo = item.item.GetComponent<Item>();
            if (itemInfo == null) continue;

            if (itemInfo.Data.displayName == displayName)
            {
                AddSkillPoints(item);
                return;
            }
        }
    }

    private void PulseCompletedItems()
    {
        int pickupLayer = LayerMask.NameToLayer("Pickup");

        foreach (Item item in heldItems)
        {
            if (item == null)
                continue;

            GameObject obj = item.gameObject;
            Transform itemTransform = obj.transform;
            itemTransform.DOKill();

            obj.layer = 2; // can't be picked up during the punch

            Vector3 originalScale = itemTransform.localScale;
            itemTransform.DOPunchScale(originalScale * 0.2f, 0.35f, 1, 0f)
                .SetLink(obj)
                .OnKill(() =>
                {
                    if (obj == null)
                        return;

                    itemTransform.localScale = originalScale;
                    obj.layer = pickupLayer;
                });
        }
    }

    private void AddUpgradePoints()
    {
        if (currentItem.IsPlaced) return;
        
        currentItem.IsPlaced = true;
        currency.ChangeUpgradePoints(1);
    }

    private void AddSkillPoints(ItemSpawnEntry item)
    {
        if (item.isComplete) return;

        item.isComplete = true;
        currency.ChangeSkillPoints(1);
    }

    private void UpdateUI()
    {
        displayNameTxt.text = heldItems.Count > 0 ? displayName : "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerUpgrade.CanThrowToShelf)
            return;

        if (isAssignedToBuddy)
            return;

        Item item = other.GetComponentInParent<Item>();

        if (item == null)
            return;

        if (heldItems.Contains(item))
            return;

        if (!IsItemValid(item))
            return;

        PlaceItem(item);
    }

    public void CleanAndReorganize()
    {
        Debug.Log($"{name} has {transform.childCount} children before cleaning.");
        heldItems.Clear();
        shelfPositions.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Item item = transform.GetChild(i).GetComponent<Item>();

            if (item != null)
            {
                heldItems.Add(item);
            }
        }

        if (heldItems.Count == 0) return;

        currentItem = heldItems[0];
        displayName = currentItem.Data.displayName;

        SetMaxItems();
        ComputeItemPlacements();

        for (int i = 0; i<heldItems.Count; i++)
        {
            Item item = heldItems[i];
            item.Held(true, transform);

            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
            }

            item.transform.position = transform.TransformPoint(shelfPositions[i]);
            item.transform.rotation = transform.rotation * Quaternion.Euler(item.Data.preferredOrientation);
        }

        currentItem = null;
        UpdateUI();
    }

    public void AssignToBuddy(bool assigned, RobotBuddy robot)
    {
        if (assigned)
            robotList.Add(robot);
        else
            robotList.Remove(robot);

        isAssignedToBuddy = robotList.Count > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (Vector3 pos in shelfPositions)
        {
            Vector3 worldPosition = transform.TransformPoint(pos);
            Gizmos.DrawWireSphere(worldPosition, 0.15f);
        }
    }
}
