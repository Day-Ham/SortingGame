using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private Button resetAllButton;

    [Header("Control Schemas")]
    [SerializeField] private List<ControlSchema> controlSchema;
    private string _targetControlSchema;
    private RebindSaveLoad _targetRebindSaveLoad;

    private void Start()
    {
        resetAllButton.onClick.AddListener(ResetControlsSchemeBinding);
    }

    public void ResetControlsSchemeBinding()
    {
        CheckSchema();
        foreach (InputActionMap map in _inputActions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlSchema));
            }
        }

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
}

[Serializable]
public class ControlSchema
{
    public GameObject schemaPanel;
    public string schema;
    public RebindSaveLoad rebindSaveLoad;
}
