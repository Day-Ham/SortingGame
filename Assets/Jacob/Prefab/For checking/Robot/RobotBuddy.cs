using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class RobotBuddy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Vector3 targetDestination;

    [Header("Cute Hover")]
    [SerializeField] private float hoverAmount;
    [SerializeField] private float hoverSpeed;

    private float baseOffset;

    [Header("Pickup")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupSpeed;

    [Header("Target Item")]
    [SerializeField] private List<Item> foundItems = new List<Item>();
    private Item targetItem;
    [SerializeField] private GameObject heldObject;
    private Rigidbody heldRigidbody;

    [Header("Target Shelf")]
    [SerializeField] private GameObject targetShelf;
    private ItemChecker targetItemChecker;


    //Checks
    [SerializeField] private bool isPickingUp;
    [SerializeField] private bool isPlacing;

    ItemSpawner itemSpawner;

    private void Awake()
    {
        itemSpawner = FindAnyObjectByType<ItemSpawner>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseOffset = agent.baseOffset;
    }

    private void Update()
    {
        float bounce = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        agent.baseOffset = baseOffset + bounce;

        if (isPlacing) return;

        if (isPickingUp)
        {
            if (heldObject == null)
            {
                CancelPickup();
                return;
            }

            Item item = heldObject.GetComponent<Item>();

            if (item == null)
            {
                CancelPickup();
                return;
            }

            if (item.HeldBy != transform)
            {
                Debug.Log($"Robot lost {item.name} to another holder.");

                CancelPickup();
                return;
            }

            PickupLerp();
            return;
        }

        if (targetItem == null)
        {
            if (heldObject == null)
                FindItem();

            return;
        }

        MoveToDestination();
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (targetItemChecker != null && !isPlacing)
            {
                Debug.Log($" Robot is trying to place {targetItem.name} on {targetShelf.name}.");
                isPlacing = true;
                StartCoroutine(PlaceItemOnShelf());
            }
            else
            {
                TryPickup(targetItem);
            } 
        }        
    }

    IEnumerator PlaceItemOnShelf()
    {
        yield return new WaitUntil(() => !targetItemChecker.isPlacingItem);
        Debug.Log("Calling ItemChecker to take the item from Robot Buddy");
        targetItemChecker.PlaceItem(targetItem);

        yield return new WaitUntil(() => !targetItemChecker.isPlacingItem);
        Debug.Log($" Robot placed {targetItem.name} on {targetShelf.name}.");
        ResetTargetShelf();
        FindItem();
        isPlacing = false;
    }

    private void MoveToDestination()
    {
        agent.SetDestination(targetDestination);
    }

    private void FindItem()
    {
        if (heldObject != null)
            return;

        if (targetItem != null)
            return;

        foundItems.Clear();

        Item[] allItems = FindObjectsByType<Item>();

        foreach (Item item in allItems)
        {
            if (item == null)
                continue;

            if (item.IsHeld)
                continue;

            if (item.GetComponentInParent<ItemChecker>())
                continue;

            ItemSpawner spawner = item.GetComponentInParent<ItemSpawner>();

            if (spawner == null)
                continue;

            foundItems.Add(item);
        }

        if (foundItems.Count == 0)
            return;

        targetItem = foundItems[Random.Range(0, foundItems.Count)];
        targetDestination = targetItem.transform.position;
    }

    private void TryPickup(Item item)
    {
        if (item == null)
            return;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        heldObject = item.gameObject;
        heldRigidbody = rb;

        item.Held(true, transform);

        Debug.Log($" Robot picked up {item.name}. HeldBy = {item.HeldBy.name}");

        isPickingUp = true;

        agent.ResetPath();
    }

    private void CancelPickup()
    {
        isPickingUp = false;

        heldObject = null;
        heldRigidbody = null;

        targetItem = null;
    }

    private void PickupLerp()
    {
        if (heldObject == null)
        {
            CancelPickup();
            return;
        }

        Transform obj = heldObject.transform;

        obj.position = Vector3.Lerp(obj.position, holdPoint.position, pickupSpeed * Time.deltaTime);
        obj.rotation = Quaternion.Lerp( obj.rotation, holdPoint.rotation, pickupSpeed * Time.deltaTime);

        if (Vector3.Distance(obj.position, holdPoint.position) < 0.01f && Quaternion.Angle(obj.rotation, holdPoint.rotation) < 1f)
        {
            obj.position = holdPoint.position;
            obj.rotation = holdPoint.rotation;

            obj.SetParent(holdPoint, true);

            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;

            isPickingUp = false;

            Debug.Log($"Robot is now holding {obj.name}");
            FindShelf();
        }
    }

    private void FindShelf()
    {
        ItemChecker[] shelves = FindObjectsByType<ItemChecker>()
            .OrderByDescending(shelf => shelf.heldItems.Count)
            .ToArray();

        if (shelves.Length == 0)
        {
            DropItem();
            return;
        }

        foreach (ItemChecker shelf in shelves)
        {
            if (shelf.IsItemValid(targetItem))
            {
                targetItemChecker = shelf;
                targetShelf = shelf.gameObject;
                break;
            }
            else
            {
                continue;
            }
        }

        if (targetItemChecker == null)
        {
            DropItem();
            return;
        }

        
        targetDestination = targetItemChecker.transform.position;
    }

    private void DropItem()
    {
        heldObject.transform.SetParent(itemSpawner.transform);
        heldRigidbody.isKinematic = false;
        CancelPickup();
        Debug.Log("There are no shelves available :(. Dropping the item.");
    }

    private void ResetTargetShelf()
    {
        targetShelf = null;
        targetItemChecker = null;
    }
}