using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ChangeDescText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI descText; 

    [Header("Text Settings")]
    [SerializeField] private string normalText;
    [SerializeField] private string hoverText;

    void Start()
    {
        if (descText != null)
        {
            descText.text = normalText;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descText != null)
        {
            descText.text = hoverText;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descText != null)
        {
            descText.text = normalText;
        }
    }
}