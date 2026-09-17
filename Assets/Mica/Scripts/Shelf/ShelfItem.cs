using UnityEngine;

public class ShelfItem : MonoBehaviour, IPickupable
{
    [SerializeField] private string shelfItemName;
    public Transform shelfItemInitialParent;
    private void Awake()
    {
        transform.name = shelfItemName;
    }

    public void Pickup()
    {
        transform.SetParent(shelfItemInitialParent);
    }
}

