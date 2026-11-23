using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normalColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
    }
}
