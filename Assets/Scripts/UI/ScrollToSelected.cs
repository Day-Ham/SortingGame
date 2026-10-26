using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollToSelected : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    private GameObject lastSelected;

    private void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null || selected == lastSelected)
            return;

        lastSelected = selected;

        RectTransform selectedRect = selected.GetComponent<RectTransform>();

        if (selectedRect == null)
            return;

        Canvas.ForceUpdateCanvases();

        ScrollToSelectedElement(selectedRect);
    }

    private void ScrollToSelectedElement(RectTransform selected)
    {
        RectTransform viewport = scrollRect.viewport;

        Vector3[] selectedCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];

        selected.GetWorldCorners(selectedCorners);
        viewport.GetWorldCorners(viewportCorners);

        float selectedTop = selectedCorners[1].y;
        float selectedBottom = selectedCorners[0].y;

        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        // Selected item is above the viewport
        if (selectedTop > viewportTop)
        {
            float difference = selectedTop - viewportTop;

            ScrollByPixels(difference);
        }
        // Selected item is below the viewport
        else if (selectedBottom < viewportBottom)
        {
            float difference = viewportBottom - selectedBottom;

            ScrollByPixels(-difference);
        }
    }

    private void ScrollByPixels(float pixels)
    {
        RectTransform content = scrollRect.content;

        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        float scrollableHeight = contentHeight - viewportHeight;

        if (scrollableHeight <= 0)
            return;

        float normalizedChange = pixels / scrollableHeight;

        scrollRect.verticalNormalizedPosition =
            Mathf.Clamp01(
                scrollRect.verticalNormalizedPosition + normalizedChange
            );
    }
}
