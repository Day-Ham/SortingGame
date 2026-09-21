using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item")]
    public List<ItemSpawnEntry> items;
    private GameObject currentItem;

    [Header("Spawning")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private float spawnHeight = 0.5f;

    [Header("Settings")]
    [SerializeField] private int maxAttempts = 30;
    [SerializeField] private LayerMask obstacleLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstantiateItems();
    }

    private void InstantiateItems()
    {
        foreach (ItemSpawnEntry item in items)
        {
            currentItem = item.item;
            for (int i = 0; i < item.amount; i++) 
            {
                for (int j = 0; j < maxAttempts; j++)
                {
                    Vector3 spawnPoint = GetRandomSpawnPoint();
                    if (IsPositionBlocked(spawnPoint))
                    {
                        continue;
                    }
                    GameObject newItem = Instantiate(item.item, spawnPoint, Quaternion.identity);
                    newItem.transform.SetParent(transform);
                    break;
                }
            }
        }
    }

    Vector3 GetRandomSpawnPoint()
    {
        Bounds bounds = spawnArea.bounds;

        float randomX = UnityEngine.Random.Range(bounds.min.x,bounds.max.x);
        float randomZ = UnityEngine.Random.Range(bounds.min.z,bounds.max.z);

        return new Vector3(randomX, bounds.min.y + spawnHeight,randomZ);
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        Collider col = currentItem.GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("Item has no Collider.");
            return false;
        }
        Vector3 halfExtents = col.bounds.extents;
        return Physics.CheckBox(position, halfExtents, Quaternion.identity,obstacleLayer,QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnArea == null) return;

        Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
    }
}

[Serializable]
public class ItemSpawnEntry
{
    public GameObject item;
    public int amount;
    public bool isComplete;
}