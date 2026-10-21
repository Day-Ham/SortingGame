using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item")]
    public List<ItemSpawnEntry> items;
    [SerializeField] private bool itemsSpawned;

    [Header("Spawning")]
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private float spawnHeight = 0.5f;

    [Header("Settings")]
    [SerializeField] private int maxAttempts = 30;
    [SerializeField] private LayerMask obstacleLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (itemsSpawned) return;
        InstantiateItems();
    }

    private void InstantiateItems()
    {
        foreach (ItemSpawnEntry entry in items)
        {
            Item itemComponent = entry.item.GetComponent<Item>();
            if (itemComponent == null) continue;

            int numberOfItem = itemComponent.Data.rows * itemComponent.Data.columns;

            for (int i = 0; i < numberOfItem; i++) 
            {
                for (int j = 0; j < maxAttempts; j++)
                {
                    Vector3 spawnPoint = GetRandomSpawnPoint();
                    if (float.IsInfinity(spawnPoint.x)) continue;

                    if (IsPositionBlocked(spawnPoint, entry.item)) continue;

                    GameObject newItem = Instantiate(entry.item, spawnPoint, Quaternion.Euler(GetRandomRotation()));
                    newItem.transform.SetParent(transform);     
                    break;
                }
            }
        }

        itemsSpawned = true;
    }

    Vector3 GetRandomSpawnPoint()
    {
        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

            Vector3 randomPoint = new Vector3(randomX, bounds.center.y, randomZ);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position + Vector3.up * spawnHeight;
            }
        }

        return Vector3.positiveInfinity;
    }
    Vector3 GetRandomRotation()
    {
        float randomX = UnityEngine.Random.Range(0,360);
        float randomY = UnityEngine.Random.Range(0,360);
        float randomZ = UnityEngine.Random.Range(0, 360);

        return new Vector3(randomX, randomY, randomZ);
    }

    private bool IsPositionBlocked(Vector3 position, GameObject itemPrefab)
    {
        Collider col = itemPrefab.GetComponent<Collider>();
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
    public bool isComplete;
}