using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private GameObject resetToDefaultPromp;
    [SerializeField] private Button defaultButton;
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;

    [Header("Control Schemas")]
    [SerializeField] private List<ControlSchema> controlSchema;
    private string _targetControlSchema;
    private RebindSaveLoad _targetRebindSaveLoad;

    private void Start()
    {

        defaultButton.onClick.AddListener(OpenDefaultPrompt);
        confirmResetButton.onClick.AddListener(ResetControlsSchemeBinding);
        cancelResetButton.onClick.AddListener(CloseDefaultPrompt);
    }

    public void ResetControlsSchemeBinding()
    {
        CheckSchema();
        
        InputActionMap playerMap = _inputActions.FindActionMap("Player", true);
        foreach (InputAction action in playerMap.actions)
        {
            action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlSchema));
        }

        CloseDefaultPrompt();

        //Save keybinds
        _targetRebindSaveLoad.Save();
        PlayerPrefs.Save();
    }

    private void CheckSchema()
    {
        for (int i = 0; i < controlSchema.Count; i++)
        {
            if (controlSchema[i].schemaPanel.activeInHierarchy)
            {
                _targetControlSchema = controlSchema[i].schema;
                _targetRebindSaveLoad = controlSchema[i].rebindSaveLoad;

                Debug.Log("Control schema: " + _targetControlSchema);
                Debug.Log("Rebind save load: " + _targetRebindSaveLoad);
                return;
            }
        }
    }

    private void OpenDefaultPrompt()
    {
        resetToDefaultPromp.SetActive(true);
    }

    private void CloseDefaultPrompt()
    {
        resetToDefaultPromp.SetActive(false);
    }
}

[Serializable]
public class ControlSchema
{
    public GameObject schemaPanel;
    public string schema;
    public RebindSaveLoad rebindSaveLoad;
}
