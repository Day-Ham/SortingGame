using System.Collections.Generic;
using UnityEngine;

public class ShelfItemSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> shelfItems;
    [SerializeField] private int numPerShelfItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstantiateShelfItems();
    }

    private void InstantiateShelfItems()
    {
        foreach (GameObject obj in shelfItems)
        {
            for (int i = 0; i < numPerShelfItem; i++)
            {
                GameObject newShelfItem = Instantiate(obj);
                newShelfItem.SetActive(false);
                newShelfItem.transform.position = transform.position;
                newShelfItem.transform.rotation = transform.rotation;
                newShelfItem.GetComponent<ShelfItem>().shelfItemInitialParent = transform;
                newShelfItem.transform.SetParent(transform);
            }
        }
    }


}
