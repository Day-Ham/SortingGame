using System.Collections.Generic;
using UnityEngine;

public class ItemChecker : MonoBehaviour
{
    public enum ContainerType
    {
        Ore,
        Canister,
        Hardrive,
        Medicine,
        Fuel,
        Food,
        Weapon
    }

    [Header("Container Info")]
    [SerializeField] private ContainerType type;
    private List<GameObject> heldItems = new List<GameObject>();

    private ItemData itemData;

    bool IsItemViable()
    {
        //if (heldItems.Count > 0) return true;
        //if(heldItems.)

        //if ()
        //{

        //}
        return false;
    }
    public void PlaceItem()
    {
        //if ()
        //{

        //}
    } 

    
    
    void GetItemInfo()
    {

    }
}
