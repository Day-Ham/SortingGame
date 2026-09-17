using UnityEngine;

public interface IInteractable
{
    void PlaceItem(Transform heldObject);
    void TakeItem(Transform heldObject);
}
