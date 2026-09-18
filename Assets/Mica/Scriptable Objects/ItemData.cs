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

    [Header("Shelf Layout")]
    public int rows;
    public int columns;
    public Vector2 spacing;

    [Header("Audio SFX")]
    public AudioClip audioSFX;
}
