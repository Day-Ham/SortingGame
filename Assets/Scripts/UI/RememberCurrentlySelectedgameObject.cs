using UnityEngine;
using UnityEngine.EventSystems;

public class RememberCurrentlySelectedgameObject : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject lastSelectedElement;

    private void Reset()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();

        if (!eventSystem)
        {
            Debug.Log("Did not find an Event System in your Scene", this);
            return;
        }

        lastSelectedElement = eventSystem.firstSelectedGameObject;
    }

    private void Update()
    {
        if (!eventSystem) return;

        if (eventSystem.currentSelectedGameObject &&
            lastSelectedElement != eventSystem.currentSelectedGameObject)
            lastSelectedElement = eventSystem.currentSelectedGameObject;

        if (!eventSystem.currentSelectedGameObject && lastSelectedElement)
            eventSystem.SetSelectedGameObject(lastSelectedElement);
    }
}
