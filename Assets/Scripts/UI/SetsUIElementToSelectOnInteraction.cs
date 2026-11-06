using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SetsUIElementToSelectOnInteraction : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private EventSystem eventSystem;
    public Selectable elementToSelect;
    private Stack<GameObject> selectionHistory = new Stack<GameObject>();
    public Dictionary<Button, Sprite> normalSprites { get; private set; } = new();

    [Header("Custom Secondary Element")]
    [SerializeField] private GameObject secondaryOption;

    private InputDeviceDetector inputDetector;
    private InputDeviceDetector.InputMode mode;
    private UserInput userInput;
    private TitleScreenManager titleManager;

    private GameObject hoveredElement;
    private GameObject currentSelected;
    private bool isSecondarySelected;

    private void Awake()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
            Debug.Log("Did not find an Event System in your Scene", this);

        normalSprites.Clear();
        StoreNormalSprites();
        ClearSelectionHistory();
    }

    private void Start()
    {
        inputDetector = InputDeviceDetector.Instance;
        userInput = UserInput.instance;
        titleManager = FindAnyObjectByType<TitleScreenManager>();
    }

    private void OnDisable()
    {
        ReturnToParent();
        isSecondarySelected = false;

        ClearSelectionHistory();

        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.isSecondarySelected = isSecondarySelected;
        }

        if (titleManager != null)
        {
            titleManager.isSecondarySelected = isSecondarySelected;
        }
    }

    private void Reset()
    {
        eventSystem = FindAnyObjectByType<EventSystem>();
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
            if (userInput.SubmitInput)
            {
                SelectSecondaryElement();
            }

            if (userInput.EscapeInput && isSecondarySelected)
            {
                ReturnToParent();
            }

            if (isSecondarySelected) return;

            GameObject selected = GetSelectedUI();

            if (selected != null)
            {
                Debug.Log("Currently Selected: " + selected.name);
            }

            if (hoveredElement == null && selected == null)
            {
                JumpToElement(elementToSelect.gameObject);
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
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void StoreNormalSprites()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            Image image = button.targetGraphic as Image;

            if (image != null)
                normalSprites[button] = image.sprite;
        }
    }

    public void JumpToElement(GameObject element)
    {
        if (eventSystem == null || element == null) return;

        eventSystem.SetSelectedGameObject(element);
        currentSelected = element;
    }

    public GameObject GetHoveredUI()
    {
        if (Mouse.current == null) return null;

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

    public void SelectSecondaryElement()
    {
        if (isSecondarySelected || currentSelected == null) return;

        GameObject secondaryUI = null;
        if (secondaryOption != null)
        {
            if (secondaryOption.CompareTag("SecondaryUIElement"))
            {
                secondaryUI = secondaryOption;
            }
            else
            {
                secondaryUI = FindChildWithTag(secondaryOption.transform, "SecondaryUIElement");
            }
        }
        else
        {
            secondaryUI = FindChildWithTag(currentSelected.transform, "SecondaryUIElement");
        }

        if (secondaryUI == null) return;

        // Keep the parent selected visually.
        Button btn = currentSelected.GetComponent<Button>();

        if (btn != null)
        {
            Image image = btn.targetGraphic as Image;

            if (image != null)
                image.sprite = btn.spriteState.pressedSprite;
        }

        // Select the secondary element.
        eventSystem.SetSelectedGameObject(secondaryUI);

        isSecondarySelected = true;

        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.isSecondarySelected = isSecondarySelected;
        }

        if (titleManager != null)
        {
            titleManager.isSecondarySelected = isSecondarySelected;
        }
    }

    public void ReturnToParent()
    {
        if (!isSecondarySelected || currentSelected == null) return;

        Button btn = currentSelected.GetComponent<Button>();

        if (btn != null)
        {
            Image image = btn.targetGraphic as Image;

            if (image != null)
            {
                Sprite normalSprite = normalSprites[btn];
                image.sprite = normalSprite;
            }
                
        }

        eventSystem.SetSelectedGameObject(currentSelected);

        isSecondarySelected = false;

        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.isSecondarySelected = isSecondarySelected;
        }

        if (titleManager != null)
        {
            titleManager.isSecondarySelected = isSecondarySelected;
        }
    }

    public GameObject FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag) && child.gameObject.activeInHierarchy)
                return child.gameObject;

            GameObject result = FindChildWithTag(child, tag);

            if (result != null && result.activeInHierarchy)
                return result;
        }

        return null;
    }

    public void SelectAndRemember(GameObject nextElement)
    {
        if (eventSystem == null || nextElement == null) return;

        GameObject previous = eventSystem.currentSelectedGameObject;

        if (previous != null)
        {
            selectionHistory.Push(previous);

            Button button = previous.GetComponent<Button>();
            if (button != null)
            {
                Image image = button.targetGraphic as Image;
                if (image != null)
                {
                    image.sprite = button.spriteState.pressedSprite;
                }
            }
        }

        eventSystem.SetSelectedGameObject(nextElement);
        currentSelected = nextElement;
    }

    public void ReturnToPreviousSelection()
    {
        if (eventSystem == null || selectionHistory.Count == 0) return;

        GameObject previous = selectionHistory.Pop();

        if (previous == null || !previous.activeInHierarchy) return;

        Button button = previous.GetComponent<Button>();
        if (button != null)
        {
            Image image = button.targetGraphic as Image;
            if (image != null)
            {
                image.sprite = normalSprites[button];
            }
        }

        eventSystem.SetSelectedGameObject(previous);
        currentSelected = previous;
    }

    public void ClearSelectionHistory()
    {
        selectionHistory.Clear();
    }
}