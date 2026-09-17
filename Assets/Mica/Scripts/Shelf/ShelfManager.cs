using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    [Header("Single Shelf")]
    [SerializeField] private float shelfHeight;

    [Header("Whole Shelf")]
    [SerializeField] private Shelf[] shelfObj;
    [SerializeField] private float moveSpeed = 5f;
    private int shelfCount = 1;
    private Vector3 targetPosition;
    private bool isUpgrading;

    private void Start()
    {
        targetPosition = transform.position;
    }
    private void Update()
    {
        if (!isUpgrading && shelfObj[shelfCount - 1].isComplete)
        {
            UpgradeShelf();
        }

        MoveShelf();
    }

    private void UpgradeShelf()
    {
        if (shelfCount >= shelfObj.Length) return;
        shelfCount++;
        targetPosition = transform.position + Vector3.up * shelfHeight;
        isUpgrading = true;
    }

    private void MoveShelf()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            isUpgrading = false;
        }
    }
}
