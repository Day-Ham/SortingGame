using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] private ShelfSorting topShelf;
    [SerializeField] private ShelfSorting botShelf;
    public bool isComplete;

    
    private void Awake()
    {
        isComplete = false;
    }

    private void Update()
    {
        if (topShelf.isSorted && botShelf.isSorted)
            isComplete = true;
     }
}
