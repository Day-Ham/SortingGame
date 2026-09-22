using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Skill 1")]
    [SerializeField] private float highlightDuration = 10f;
    [SerializeField] private float cooldownDuration = 5f;
    [SerializeField] private string highlightLayerName = "Highlight";
    [SerializeField] private GameObject skill1Icon;
    [SerializeField] private Slider skill1CooldownSlider;
    [SerializeField] private Slider skill1LevelSlider;
    [SerializeField] private int skill1Level;
    [SerializeField] private int skill1Price = 50;

    private float highlightTimer;
    private float cooldownTimer;

    private int highlightLayer;
    private InputAction skill1Action;

    private List<GameObject> highlightedObjects = new List<GameObject>();
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    private void Awake()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        skill1Action = playerInput.actions.FindAction("Skill1");
        highlightLayer = LayerMask.NameToLayer(highlightLayerName);
    }

    private void Start()
    {
        skill1Icon.SetActive(skill1Level > 0);
        skill1CooldownSlider.value = 0f;
    }

    private void Update()
    {
        if (highlightTimer > 0f)
        {
            highlightTimer -= Time.deltaTime;

            if (highlightTimer <= 0f)
            {
                highlightTimer = 0f;
                ClearHighlights();
            }
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;

            skill1CooldownSlider.value = 1f - (cooldownTimer / cooldownDuration);

            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                skill1CooldownSlider.value = 1f;
            }
        }
    }

    private void OnEnable()
    {
        skill1Action.performed += OnSkill1Pressed;
    }

    private void OnDisable()
    {
        skill1Action.performed -= OnSkill1Pressed;
    }

    private void OnSkill1Pressed(InputAction.CallbackContext context)
    {
        ActivateSkill1();
    }

    private void ActivateSkill1()
    {
        if (skill1Level <= 0)
            return;

        if (cooldownTimer > 0f)
            return;

        if (playerInteraction.heldObject == null)
            return;

        Item heldItem = playerInteraction.heldObject.GetComponent<Item>();

        if (heldItem == null || heldItem.Data == null)
            return;

        ClearHighlights();

        Item[] allItems = FindObjectsByType<Item>();

        foreach (Item item in allItems)
        {
            if (item.Data == null)
                continue;

            if (item.Data.displayName != heldItem.Data.displayName)
                continue;

            if (item.gameObject == playerInteraction.heldObject)
                continue;

            HighlightObject(item.gameObject);
        }

        highlightTimer = highlightDuration;
        cooldownTimer = cooldownDuration;
    }


    private void HighlightObject(GameObject obj)
    {
        if (highlightLayer == -1)
            return;

        SetHighlightLayer(obj);

        highlightedObjects.Add(obj);
    }

    private void SetHighlightLayer(GameObject obj)
    {
        if (!originalLayers.ContainsKey(obj))
        {
            originalLayers[obj] = obj.layer;
        }

        obj.layer = highlightLayer;

        foreach (Transform child in obj.transform)
        {
            SetHighlightLayer(child.gameObject);
        }
    }

    private void ClearHighlights()
    {
        foreach (GameObject obj in highlightedObjects)
        {
            if (obj != null)
            {
                RestoreLayer(obj);
            }
        }

        highlightedObjects.Clear();
        originalLayers.Clear();
    }

    private void RestoreLayer(GameObject obj)
    {
        if (originalLayers.TryGetValue(obj, out int originalLayer))
        {
            obj.layer = originalLayer;
            originalLayers.Remove(obj);
        }

        foreach (Transform child in obj.transform)
        {
            RestoreLayer(child.gameObject);
        }
    }
    private void FillSlider(Slider slider, int maxLevels)
    {
        slider.value += 1f / maxLevels;
    }

    public void BuySkill1()
    {
        if (skill1Level >= 5)
            return;

        if (currencyManager.coin < skill1Price)
            return;

        currencyManager.ChangeCoin(-skill1Price);

        skill1Level++;

        if (skill1Level == 1)
        {
            skill1Icon.SetActive(true);
        }
        else
        {
            highlightDuration += 2f;
        }

        skill1Price += 50;
        FillSlider(skill1LevelSlider, 5);
    }
}