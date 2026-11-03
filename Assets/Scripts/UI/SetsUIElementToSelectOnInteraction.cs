using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SetsUIElementToSelectOnInteraction : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable elementToSelect;

    [Header("Visualization")]
    [SerializeField] private bool showVisualization;
    [SerializeField] private Color navigationColor = Color.cyan;

    private InputDeviceDetector inputDetector;
    private InputDeviceDetector.InputMode mode;
    private GameObject hoveredElement;
    private GameObject currentSelected;

    private void OnDrawGizmos()
    {
        if (!showVisualization) return;
        if (elementToSelect == null) return;

        Gizmos.color = navigationColor;
        Gizmos.DrawLine(gameObject.transform.position, elementToSelect.gameObject.transform.position);
    }

    private void Awake()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
            Debug.Log("Did not find an Event System in your Scene", this);
    }

    private void Start()
    {
        inputDetector = InputDeviceDetector.Instance;
    }

    private void Reset()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
            Debug.Log("Did not find an Event System in your Scene", this);
    }

    private void Update()
    {
        if (inputDetector == null || EventSystem.current == null) return;
        mode = inputDetector.CurrentMode;

        if (Cursor.visible && mode == InputDeviceDetector.InputMode.KeyboardMouse)
        {
            hoveredElement = GetHoveredUI();
        }

        if (mode == InputDeviceDetector.InputMode.Gamepad)
        {
            GameObject selected = GetSelectedUI();
            if (hoveredElement == null && selected == null)
            {
                JumpToElement();
            }
            else if (hoveredElement == null)
            {
                currentSelected = selected;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(hoveredElement);
                currentSelected = hoveredElement;
                hoveredElement = null;
            }

            //SelectSecondaryElement();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void JumpToElement()
    {
        if (eventSystem == null)
            Debug.Log("This item has no event system referenced yet.", this);

        if (elementToSelect == null)
            Debug.Log("This should jump where?", this);

        eventSystem.SetSelectedGameObject(elementToSelect.gameObject);
        currentSelected = elementToSelect.gameObject;
    }

    public GameObject GetHoveredUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Transform current = result.gameObject.transform;

            while (current != null)
            {
                if (current.CompareTag("MainUIElement"))
                {
                    Selectable selectable = current.GetComponent<Selectable>();

                    if (selectable != null &&
                        selectable.navigation.mode != Navigation.Mode.None)
                    {
                        return current.gameObject;
                    }
                }

                current = current.parent;
            }
        }

        return null;
    }

    public GameObject GetSelectedUI()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null || !selected.activeInHierarchy)
            return null;

        if (!selected.CompareTag("MainUIElement"))
            return null;

        Selectable selectable = selected.GetComponent<Selectable>();

        if (selectable == null ||
            selectable.navigation.mode == Navigation.Mode.None)
            return null;

        return selected;
    }

    //WIP
    public void SelectSecondaryElement()
    {
        GameObject secondaryUI = FindChildWithTag(currentSelected.transform, "SecondaryUIElement");

        if (secondaryUI == null) return;
        
        if (currentSelected.transform.GetChild(0).gameObject != null)
        {
            Button btn = currentSelected.GetComponent<Button>();

            if (btn == null) return;

            Image image = btn.targetGraphic as Image;

            if (image != null)
            {
                SpriteState state = btn.spriteState;
                image.sprite = state.pressedSprite;
            }
            currentSelected.transform.GetChild(0).gameObject.SetActive(true);
        }

        eventSystem.SetSelectedGameObject(secondaryUI);
    }

    public GameObject FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
                return child.gameObject;

            GameObject result = FindChildWithTag(child, tag);

            if (result != null)
                return result;
        }

        return null;
    }
}
