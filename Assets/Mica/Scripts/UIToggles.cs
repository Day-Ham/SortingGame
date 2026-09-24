using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggles : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private List<Toggle> toggles = new List<Toggle>();
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        ResetToFirstToggle();
    }

    void Start()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i;

            toggles[i].onValueChanged.AddListener(state =>
            {
                if (state)
                {
                    ShowPanel(index);
                }
            });
        }
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    public void ResetToFirstToggle()
    {
        toggles[0].isOn = true;
    }

    
}
