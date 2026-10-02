using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetsUIElementToSelectOnInteraction : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable elemtentToSelect;

    [Header("Visualization")]
    [SerializeField] private bool showVisualization;
    [SerializeField] private Color navigationColor = Color.cyan;

    private void OnDrawGizmos()
    {
        if (!showVisualization) return;
        if (elemtentToSelect == null) return;

        Gizmos.color = navigationColor;
        Gizmos.DrawLine(gameObject.transform.position, elemtentToSelect.gameObject.transform.position);
    }

    private void Reset()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
            Debug.Log("Did not find an Event System in your Scene", this);
    }

    public void JumpToElement()
    {
        if (eventSystem == null)
            Debug.Log("This item has no event system referenced yet.", this);

        if (elemtentToSelect == null)
            Debug.Log("This should jump where?", this);

        eventSystem.SetSelectedGameObject(elemtentToSelect.gameObject);
    }
}
