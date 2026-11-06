using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class KeybindingOverlayManager : MonoBehaviour
{
    UserInput input;
    [SerializeField] private InputSystemUIInputModule inputModule;
    private void Awake()
    {
        input = UserInput.instance;
        
        if (input != null)
        {
            input.SetUIInput(true);
            inputModule.move.action.Enable();
            inputModule.submit.action.Enable();
        }
    }
    private void OnEnable()
    {
        if (input != null)
        {
            input.SetUIInput(false);
            inputModule.move.action.Disable();
            inputModule.submit.action.Disable();
        }
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.SetUIInput(true);
            inputModule.move.action.Enable();
            inputModule.submit.action.Enable();
        }
    }
}
