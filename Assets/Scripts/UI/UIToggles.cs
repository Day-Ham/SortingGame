using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIToggles : MonoBehaviour
{
    public enum TabType
    {
        Primary,
        Secondary
    }

    [Header("Tab Settings")]
    [SerializeField] private TabType tabType;

    [Header("Options")]
    [SerializeField] private List<Toggle> toggles = new List<Toggle>();
    [SerializeField] private List<Sprite> normalSprites = new List<Sprite>();
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    private int currentIndex;
    private float tabCooldownTimer;
    private const float tabCooldown = 0.2f;

    private InputDeviceDetector inputDetector;
    private InputDeviceDetector.InputMode mode;
    private GameObject hoveredElement;

    private void Awake()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i;

            normalSprites.Add(toggles[i].targetGraphic.GetComponent<Image>().sprite);

            toggles[i].onValueChanged.AddListener(state =>
            {
                if (state)
                {
                    currentIndex = index;
                    ShowPanel(index);
                }
            });
        }

        inputDetector = InputDeviceDetector.Instance;
    }

    private void OnEnable()
    {
        tabCooldownTimer = 0f;
        StartCoroutine(DelayedResetRoutine());
    }

    private IEnumerator DelayedResetRoutine()
    {
        yield return null;
        ResetToFirstToggle();
    }

    private void LateUpdate()
    {
        if (UserInput.instance == null) return;

        if (tabCooldownTimer > 0f)
        {
            tabCooldownTimer -= Time.unscaledDeltaTime;
            return;
        }

        bool next = false;
        bool previous = false;

        if (tabType == TabType.Primary)
        {
            next = UserInput.instance.PrimaryTabRightInput;
            previous = UserInput.instance.PrimaryTabLeftInput;
        }
        else
        {
            next = UserInput.instance.SecondaryTabRightInput;
            previous = UserInput.instance.SecondaryTabLeftInput;
        }

        if (next)
        {
            SelectToggle(1);
            tabCooldownTimer = tabCooldown;
        }
        else if (previous)
        {
            SelectToggle(-1);
            tabCooldownTimer = tabCooldown;
        }

        if (inputDetector == null || EventSystem.current == null) return;

        if (toggles.Count == 0 || currentIndex >= toggles.Count) return;

        SetsUIElementToSelectOnInteraction selectable = toggles[currentIndex].GetComponent<SetsUIElementToSelectOnInteraction>();
        if (selectable == null) return;

        if (Cursor.visible)
        {
            hoveredElement = selectable.GetHoveredUI();
        }
        
        mode = inputDetector.CurrentMode;

        if (mode == InputDeviceDetector.InputMode.Gamepad)
        {
            GameObject selected = selectable.GetSelectedUI();
            if (hoveredElement == null && selected == null)
            {
                selectable.JumpToElement();
            }
            else if (hoveredElement == null)
            {
                return;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(hoveredElement);
                hoveredElement = null;
            }
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void SelectToggle(int direction)
    {
        if (toggles.Count == 0)
            return;

        currentIndex = (currentIndex + direction + toggles.Count) % toggles.Count;

        toggles[currentIndex].isOn = true;
        EventSystem.current?.SetSelectedGameObject(toggles[currentIndex].gameObject);
        ShowPanel(currentIndex);
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            bool isActive = i == index;
            panels[i].SetActive(isActive);

            Image image = toggles[i].targetGraphic as Image;

            if (image != null)
            {
                SpriteState state = toggles[i].spriteState;
                image.sprite = isActive ? state.pressedSprite : normalSprites[i];
            }

            toggles[i].transform.GetChild(0).gameObject.SetActive(isActive);
        }
    }

    public void ResetToFirstToggle()
    {
        if (toggles.Count == 0) return;

        currentIndex = 0;
        toggles[0].isOn = true;
        EventSystem.current?.SetSelectedGameObject(toggles[0].gameObject);
        ShowPanel(0);
    }
}