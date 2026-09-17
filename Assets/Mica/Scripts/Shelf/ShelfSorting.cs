using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfSorting : MonoBehaviour, IInteractable
{
    [Header("Shelf")]
    public Transform[] shelfSlots;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float moveSpeed;
    public bool isSorted;
    private Dictionary<Transform, Coroutine> activeMoveCoroutines = new Dictionary<Transform, Coroutine>();

    [Header("Player")]
    private PlayerInteraction playerInteraction;
    

    private void Awake()
    {
        playerInteraction = FindAnyObjectByType<PlayerInteraction>();
        isSorted = false;
    }

    private void Update()
    {
        CleanupDetachedItems();

        if (CheckIfSorted()) return;
        
        Sort();
    }

    public void Interact()
    {
        if (playerInteraction.heldObject == null) return;
        Transform shelfItem = playerInteraction.heldObject.transform;

        if (shelfItem.GetComponent<ShelfItem>() == null) return;

        Transform emptySlot = FindEmptySlot();
        if (emptySlot == null) return;

        shelfItem.SetParent(emptySlot);
        playerInteraction.DropObject();
        StartMoveToSlot(emptySlot, shelfItem);
    }

    private bool CheckIfSorted()
    {
        isSorted = false;

        if (shelfSlots == null || shelfSlots.Length == 0) return false;

        foreach (Transform slot in shelfSlots)
        {
            if (slot.childCount == 0) return false;
        }

        string firstName = shelfSlots[0].GetChild(0).name;

        for (int i = 1; i < shelfSlots.Length; i++)
        {
            if (!shelfSlots[i].GetChild(0).name.Contains(firstName)) return false;
        }

        isSorted = true;
        return true;
    }

    private void Sort()
    {
        for (int i = 0; i < shelfSlots.Length; i++)
        {
            if (shelfSlots[i].childCount != 0) continue;
            
            for (int j = i + 1; j < shelfSlots.Length; j++)
            {
                if (shelfSlots[j].childCount == 0) continue;

                Transform shelfItem = shelfSlots[j].GetChild(0);

                shelfItem.SetParent(shelfSlots[i]);
                StartMoveToSlot(shelfSlots[i], shelfItem);
                return;
            }
        }
    }

    private void StartMoveToSlot(Transform slot, Transform item)
    {
        StopMovingToSlot(item);

        Coroutine newCoroutine = StartCoroutine(MoveToSlot(slot, item));
        activeMoveCoroutines[item] = newCoroutine;
    }

    public void StopMovingToSlot(Transform item)
    {
        if (item == null) return;

        if (activeMoveCoroutines.TryGetValue(item, out Coroutine coroutine))
        {
            if (coroutine != null) StopCoroutine(coroutine);
            activeMoveCoroutines.Remove(item);
        }

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private IEnumerator MoveToSlot(Transform slot, Transform item)
    {
        if (!item.TryGetComponent<Rigidbody>(out Rigidbody rb)) yield break;

        rb.useGravity = false;
        rb.constraints |= RigidbodyConstraints.FreezeRotation;

        while (item != null && slot != null && item.parent == slot && Vector3.Distance(item.position, slot.position) > 0.01f)
        {
            Vector3 direction = slot.position - item.position;
            rb.linearVelocity = direction * moveSpeed;
            yield return null;
        }

        if (item != null)
        {
            if (rb != null) rb.linearVelocity = Vector3.zero;
            if (slot != null && item.parent == slot)
            {
                item.SetPositionAndRotation(slot.position, slot.rotation);
            }
            activeMoveCoroutines.Remove(item);
        }
    }

    private void CleanupDetachedItems()
    {
        List<Transform> itemsToRemove = null;

        foreach (var pair in activeMoveCoroutines)
        {
            Transform item = pair.Key;

            if (item == null || !IsChildOfAnySlot(item))
            {
                if (itemsToRemove == null) itemsToRemove = new List<Transform>();
                itemsToRemove.Add(item);
            }
        }

        if (itemsToRemove != null)
        {
            for (int i = 0; i < itemsToRemove.Count; i++)
            {
                StopMovingToSlot(itemsToRemove[i]);
            }
        }
    }

    private bool IsChildOfAnySlot(Transform item)
    {
        for (int i = 0; i < shelfSlots.Length; i++)
        {
            if (item.parent == shelfSlots[i]) return true;
        }
        return false;
    }

    private Transform FindEmptySlot()
    {
        for (int i = 0; i < shelfSlots.Length; i++)
        {
            if (shelfSlots[i].childCount == 0)
            {
                return shelfSlots[i].transform;
            }
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.excludeLayers |= pickupLayer;
    }

    private void OnTriggerExit(Collider other)
    {
        other.excludeLayers &= ~pickupLayer;
    }
}