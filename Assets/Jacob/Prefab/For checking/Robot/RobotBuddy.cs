using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotBuddy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Cute Hover")]
    [SerializeField] private float hoverAmount;
    [SerializeField] private float hoverSpeed;

    private float baseOffset;

    [Header("Pickup")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupSpeed;

    [Header("Target")]
    [SerializeField] private Item targetItem;
    [SerializeField] private GameObject heldObject;
    [SerializeField] private List<Item> foundItems = new List<Item>();

    private Item heldItem;

    private Rigidbody heldRigidbody;

    private bool isPickingUp;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseOffset = agent.baseOffset;

    }

    private void Update()
    {
        float bounce = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        agent.baseOffset = baseOffset + bounce;

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

        agent.SetDestination(targetItem.transform.position);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            TryPickup(targetItem);
        }
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

            if (item.IsPlaced)
                continue;

            ItemSpawner spawner = item.GetComponentInParent<ItemSpawner>();

            if (spawner == null)
                continue;

            foundItems.Add(item);
        }

        if (foundItems.Count == 0)
            return;

        targetItem = foundItems[Random.Range(0, foundItems.Count)];
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
        }
    }
}