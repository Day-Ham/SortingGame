using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;

    [Header("Orientation")]
    public Vector3 preferredOrientation;
    [Tooltip("Adds a random Euler offset each time this item is placed.")]
    public bool useRandomRotation;
    [Tooltip("Range in degrees on each axis. 1 rolls from -1 to 1. 2 rolls from -2 to 2.")]
    public Vector3 randomRotationOffset;

    [Header("Shelf Layout")]
    public int rows;
    public int columns;
    public Vector3 spacing;

    [Header("Audio SFX")]
    public AudioClip pickUpSFX;
    public AudioClip putDownSFX;

}
