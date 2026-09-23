using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour
{
    PlayerInput playerInput;
    InputAction skill1Action;
    InputAction skill2Action;
    InputAction skill3Action;

    [Header("References")]
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private CurrencyManager currencyManager;

    [Header("Skill 1")]
    [SerializeField] private float highlightDuration = 10f;
    [SerializeField] private float skill1Cooldown = 20f;
    [SerializeField] private string highlightLayerName = "Highlight";
    [SerializeField] private string shelfHighlightLayerName = "ShelfHighlight";
    [SerializeField] private GameObject skill1Icon;
    [SerializeField] private Slider skill1CooldownSlider;
    [SerializeField] private Slider skill1LevelSlider;
    [SerializeField] private int skill1Level;
    [SerializeField] private int skill1Price = 50;

    [Header("Skill 2")]
    [SerializeField] private float skill2Cooldown = 20f;
    [SerializeField] private GameObject skill2Icon;
    [SerializeField] private Slider skill2CooldownSlider;
    [SerializeField] private Slider skill2LevelSlider;
    [SerializeField] private int skill2Level;
    [SerializeField] private int skill2Price = 50;


    private float highlightTimer;
    private float skill1CooldownTimer;
    private float skill2CooldownTimer;

    private int highlightLayer;
    private int shelfHighlightLayer;

    private List<GameObject> highlightedObjects = new List<GameObject>();
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        skill1Action = playerInput.actions.FindAction("Skill1");
        skill2Action = playerInput.actions.FindAction("Skill2");
        skill3Action = playerInput.actions.FindAction("Skill3");

        highlightLayer = LayerMask.NameToLayer(highlightLayerName);
        shelfHighlightLayer = LayerMask.NameToLayer(shelfHighlightLayerName);
    }

    private void Start()
    {
        skill1Icon.SetActive(skill1Level > 0);
        skill2Icon.SetActive(skill2Level > 0);
        skill1CooldownSlider.value = 0f;
        skill2CooldownSlider.value = 0f;
    }

    private void Update()
    {
        // Skill 1 highlight duration
        if (highlightTimer > 0f)
        {
            highlightTimer -= Time.deltaTime;

            if (highlightTimer <= 0f)
            {
                highlightTimer = 0f;
                ClearHighlights();
            }
        }

        if (skill1CooldownTimer > 0f)
        {
            skill1CooldownTimer -= Time.deltaTime;

            skill1CooldownSlider.value =
                1f - (skill1CooldownTimer / skill1Cooldown);

            if (skill1CooldownTimer <= 0f)
            {
                skill1CooldownTimer = 0f;
                skill1CooldownSlider.value = 1f;
            }
        }

        if (skill2CooldownTimer > 0f)
        {
            skill2CooldownTimer -= Time.deltaTime;

            skill2CooldownSlider.value =
                1f - (skill2CooldownTimer / skill2Cooldown);

            if (skill2CooldownTimer <= 0f)
            {
                skill2CooldownTimer = 0f;
                skill2CooldownSlider.value = 1f;
            }
        }
    }

    private void OnEnable()
    {
        skill1Action.performed += OnSkill1Pressed;
        skill2Action.performed += OnSkill2Pressed;
    }

    private void OnDisable()
    {
        skill1Action.performed -= OnSkill1Pressed;
        skill2Action.performed -= OnSkill2Pressed;
    }

    private void FillSlider(Slider slider, int maxLevels)
    {
        slider.value += 1f / maxLevels;
    }

    #region Skill 1
    private void OnSkill1Pressed(InputAction.CallbackContext context)
    {
        ActivateSkill1();
    }

    private void ActivateSkill1()
    {
        if (skill1Level <= 0)
            return;

        if (skill1CooldownTimer > 0f)
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

            ItemChecker shelf = item.GetComponentInParent<ItemChecker>();

            if (shelf != null)
            {
                HighlightObject(item.gameObject, shelfHighlightLayer);
            }
            else
            {
                HighlightObject(item.gameObject, highlightLayer);
            }
        }

        highlightTimer = highlightDuration;
        skill1CooldownTimer = skill1Cooldown;
    }


    private void HighlightObject(GameObject obj, int layer)
    {
        if (layer == -1)
            return;

        SetHighlightLayer(obj, layer);

        highlightedObjects.Add(obj);
    }

    private void SetHighlightLayer(GameObject obj, int layer)
    {
        if (!originalLayers.ContainsKey(obj))
        {
            originalLayers[obj] = obj.layer;
        }

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetHighlightLayer(child.gameObject, layer);
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
    #endregion

    #region Skill 2
    private void OnSkill2Pressed(InputAction.CallbackContext context)
    {
        ActivateSkill2();
    }

    private void ActivateSkill2()
    {
        if (skill2Level <= 0)
            return;

        if (skill2CooldownTimer > 0f)
            return;

        if (playerInteraction.heldItems.Count >= playerInteraction.MaxHeldItems)
            return;

        if (playerInteraction.heldObject == null)
            return;

        Item heldItem = playerInteraction.heldObject.GetComponent<Item>();

        if (heldItem == null || heldItem.Data == null)
            return;

        string displayName = heldItem.Data.displayName;

        Item[] allItems = FindObjectsByType<Item>();

        foreach (Item item in allItems)
        {
            if (playerInteraction.heldItems.Count >= playerInteraction.MaxHeldItems)
                break;

            if (item.Data == null)
                continue;

            if (item.Data.displayName != displayName)
                continue;

            if (item.gameObject == playerInteraction.heldObject)
                continue;

            if (playerInteraction.heldItems.Contains(item.gameObject))
                continue;

            ItemChecker shelf = item.GetComponentInParent<ItemChecker>();

            if(shelf == null)
            {
                playerInteraction.TryPickup(item);
            }
        }

        skill2CooldownTimer = skill2Cooldown;
    }
    public void BuySkill2()
    {
        if (skill2Level >= 5)
            return;

        if (currencyManager.coin < skill2Price)
            return;

        currencyManager.ChangeCoin(-skill2Price);

        skill2Level++;

        if (skill2Level == 1)
        {
            skill2Icon.SetActive(true);
        }
        else
        {
            skill2Cooldown -= 2f;
        }

        skill2Price += 50;
        FillSlider(skill2LevelSlider, 5);
    }

    #endregion
}