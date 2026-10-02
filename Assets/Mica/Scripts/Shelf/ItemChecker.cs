using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    [SerializeField] private Item currentItem;
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
    [SerializeField] private float moveSpeed = 30f;
    public bool isPlacingItem;

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

        currentItem = item;

        //is the item type the same as the container type?
        if (currentItem.Data.type != itemType)
        {
            Debug.Log("Held item not same type as container type");
            return false;
        }

        //is the container has items
        if (heldItems.Count > 0)
        {
            //check if container display name is the same as held item display name
            if (currentItem.Data.displayName == displayName)
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

        Bounds itemBounds = currentItem.GetComponent<Collider>().bounds;
        float itemHeight = Mathf.Abs(itemBounds.max.y - itemBounds.min.y);

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
                y = localBotCenter.y + (itemHeight / 2);
            }
            else if (shelfType == ShelfType.Vertical)
            {
                c = cols - 1 - (k % cols);
                r = k / cols;

                float width = (cols - 1) * spacing.x;
                float height = rows * spacing.z;

                x = (localBackCenter.x - (width / 2)) + (spacing.x * c);
                y = (localBackCenter.y + (height / 2)) - (spacing.z * r);
                z = localBackCenter.z + (itemHeight / 2);
            }
            else if (shelfType == ShelfType.Slanted)
            {
                r = (rows - 1) - (k / cols);
                c = cols - 1 - (k % cols);

                float width = (cols - 1) * spacing.x;
                float length = (rows - 1) * spacing.z;

                x = (localBotCenter.x - (width / 2)) + (spacing.x * c);
                y = localBotCenter.y + (itemHeight / 2) + (spacing.y * r);
                z = (localBotCenter.z + (length / 2)) - (spacing.z * r);
            }

            Vector3 localTargetPoint = new Vector3(x, y, z);
            shelfPositions.Add(localTargetPoint);
        }
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

        AddCoin(); //adds money if first time being placed on shelf

        isPlacingItem = true;
    }

    void MoveToPlaceItem()
    {
        int itemIndex = heldItems.Count - 1;

        Rigidbody rb = heldItems[itemIndex].GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Vector3 targetPosition = shelfPositions[itemIndex];
        Quaternion targetRotation = Quaternion.Euler(orientation);

        Vector3 newPosition = Vector3.Lerp(rb.position, transform.TransformPoint(targetPosition), moveSpeed * Time.fixedDeltaTime);
        Quaternion newRotation = Quaternion.Lerp(rb.rotation, transform.rotation * targetRotation, moveSpeed * Time.fixedDeltaTime);

        rb.Move(newPosition, newRotation);

        if (Vector3.Distance(rb.position, transform.TransformPoint(targetPosition)) < 0.01f && Quaternion.Angle(rb.rotation, transform.rotation * targetRotation) < 1f)
        {
            rb.Move(transform.TransformPoint(targetPosition), transform.rotation * targetRotation);

            UpdateUI();
            CheckIfItemIsComplete();

            isPlacingItem = false;
            currentItem = null;
            Debug.Log("Placed " + heldItems[itemIndex].name + " on the shelf.");
        }
    }

    public void RemoveItem()
    {
        if (heldItems.Count == 0) return;

        Item item = heldItems[heldItems.Count - 1];
        heldItems.RemoveAt(heldItems.Count - 1);

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

        foreach (ItemSpawnEntry item in spawner.items)
        {
            Item itemInfo = item.item.GetComponent<Item>();
            if (itemInfo == null) continue;

            if (itemInfo.Data.displayName == displayName)
            {
                maxItems = item.amount;
                return;
            }
        }
    }

    private void CheckIfItemIsComplete()
    {
        if (heldItems.Count != maxItems) return;

        if (spawner.items.Count == 0) return;
        foreach (ItemSpawnEntry item in spawner.items)
        {
            Item itemInfo = item.item.GetComponent<Item>();
            if (itemInfo == null) continue;

            if (itemInfo.Data.displayName == displayName)
            {
                AddMana(item);
                return;
            }
        }
    }

    private void AddCoin()
    {
        if (currentItem.IsPlaced) return;
        
        currentItem.IsPlaced = true;
        currency.ChangeCoin(1);
    }

    private void AddMana(ItemSpawnEntry item)
    {
        if (item.isComplete) return;

        item.isComplete = true;
        currency.ChangeMana(1);
    }

    private void UpdateUI()
    {
        displayNameTxt.text = heldItems.Count > 0 ? displayName : "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerUpgrade.CanThrowToShelf)
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
