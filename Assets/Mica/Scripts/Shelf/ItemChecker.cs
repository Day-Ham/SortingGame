using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class ItemChecker : MonoBehaviour
{
    [Header("Container Info")]
    [SerializeField] private ItemType type;
    [SerializeField] private BoxCollider shelfArea;

    [Header("Container Items")]
    [SerializeField] private List<Item> heldItems = new List<Item>();
    [SerializeField] private string displayName;
    private Item currentItem;

    [Header("Player")]
    PlayerInteraction player;

    [Header("Item Spawner")]
    ItemSpawner spawner;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerInteraction>();
        if (player == null)
            Debug.Log("Player is not found");

        spawner = FindAnyObjectByType<ItemSpawner>();
        if (spawner == null)
            Debug.Log("Item Spawner is not found");
    }

    public bool IsItemValid()
    {
        //is the item valid?
        
        if (player.heldObject == null) return false;

        currentItem = player.heldObject.GetComponent<Item>();
        if(currentItem == null)
        {
            Debug.Log("Held item does not have the Item script attached");
            return false;
        }

        //is the item type the same as the container type?
        if (currentItem.Data.type != type)
        {
            Debug.Log("Held item not same type as container type");
            return false;
        }

        //is the container empty?
        if (heldItems.Count == 0)
        {
            return true;
        }
        else
        {
            //check if container display name is the same as held item display name
            if (currentItem.Data.displayName == displayName)
            {
                return true;
            }
        }

        return false;
    }

    public void PlaceItem()
    {
        if (!IsItemValid())
        {
            Debug.Log("Item cannot be placed on the shelf.");
            currentItem = null;
            return;
        }

        displayName = currentItem.Data.displayName;
        heldItems.Add(currentItem);

        // Item Data
        Vector3 orientation = currentItem.Data.preferredOrientation;
        int rows = currentItem.Data.rows;
        int cols = currentItem.Data.columns;
        Vector2 spacing = currentItem.Data.spacing;

        Bounds bounds = shelfArea.bounds;
        Vector3 botCenter = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);

        Bounds itemBounds = currentItem.GetComponent<Collider>().bounds;
        float height = Mathf.Abs(itemBounds.max.y - itemBounds.min.y);

        for (int k = 0; k < heldItems.Count; k++)
        {
            int c = k % cols;
            int r = k / cols;

            float width = (cols - 1) * spacing.x;
            float length = (rows - 1) * spacing.y;

            float x = (botCenter.x - (width / 2)) + (spacing.x * c);
            float z = (botCenter.z - (length / 2)) + (spacing.y * r);
            float y = botCenter.y + (height / 2);

            Vector3 targetPoint = new Vector3(x, y, z);

            heldItems[k].transform.SetParent(null);
            heldItems[k].transform.position = targetPoint;
            heldItems[k].transform.rotation = Quaternion.Euler(orientation);

            Rigidbody rb = heldItems[k].GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            Debug.Log("Placed " + heldItems[k].name + " on the shelf.");
        }

        player.RemoveItemFromPlayer();
    }
}
