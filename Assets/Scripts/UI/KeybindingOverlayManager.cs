using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class KeybindingOverlayManager : MonoBehaviour
{
    UserInput input;
    [SerializeField] private InputSystemUIInputModule inputModule;
    SetsUIElementToSelectOnInteraction setUISelected;
    GameObject previousSelected;
    private bool selectedRemoved;

    private void Awake()
    {
        input = UserInput.instance;
        setUISelected = GetComponent<SetsUIElementToSelectOnInteraction>();

        if (input != null)
        {
            input.SetUIInput(true);
            inputModule.move.action.Enable();
            inputModule.submit.action.Enable();
        }

        selectedRemoved = false;

    }
    private void OnEnable()
    {
        if (input == null) return;

        input.SetUIInput(false);

        if (setUISelected == null)
        {
            inputModule.move.action.Disable();
            inputModule.submit.action.Disable();
        }
        else
        {
            var moveAction = inputModule.move.action;
            if (moveAction != null)
            {
                moveAction.Enable();

                for (int i = 0; i < moveAction.bindings.Count; i++)
                {
                    var binding = moveAction.bindings[i];
                    if (binding.isPartOfComposite && (binding.name == "up" || binding.name == "down"))
                    {
                        moveAction.ApplyBindingOverride(i, string.Empty);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        if (input == null) return;

        if (setUISelected != null)
        {
            var moveAction = inputModule.move.action;
            if (moveAction != null)
            {
                for (int i = 0; i < moveAction.bindings.Count; i++)
                {
                    var binding = moveAction.bindings[i];
                    if (binding.isPartOfComposite && (binding.name == "up" || binding.name == "down"))
                    {
                        moveAction.RemoveBindingOverride(i);
                    }
                }
            }

            if (previousSelected != null)
            {
                setUISelected.JumpToElement(previousSelected.gameObject);
            }
        }

        input.SetUIInput(true);
        inputModule.move.action.Enable();
        inputModule.submit.action.Enable();
        selectedRemoved = false;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && !selectedRemoved)
        {
            previousSelected = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);

            if (setUISelected != null)
            {
                setUISelected.JumpToElement(setUISelected.elementToSelect.gameObject);
            }

            selectedRemoved = true;
        }
    }
}
