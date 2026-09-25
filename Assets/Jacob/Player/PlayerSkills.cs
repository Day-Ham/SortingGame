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
    [SerializeField] private float skill1Cooldown = 20f;
    [SerializeField] private string highlightLayerName = "Highlight";
    [SerializeField] private string shelfHighlightLayerName = "ShelfHighlight";
    [SerializeField] private int skill1Level;
    [SerializeField] private int skill1Price = 50;

    [Header("Skill 2")]
    [SerializeField] private float skill2Cooldown = 20f;
    [SerializeField] private int skill2Level;
    [SerializeField] private int skill2Price = 50;

    [Header("Skill 3")]
    [SerializeField] private float skill3Cooldown = 30f;
    [SerializeField] private int skill3Level;
    [SerializeField] private int skill3Price = 50;

    private float highlightTimer;
    private float skill1CooldownTimer;
    private float skill2CooldownTimer;
    private float skill3CooldownTimer;

    private int highlightLayer;
    private int shelfHighlightLayer;

    private List<GameObject> highlightedObjects = new List<GameObject>();
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();
    UIManager ui;

    private void Awake()
    {
        highlightLayer = LayerMask.NameToLayer(highlightLayerName);
        shelfHighlightLayer = LayerMask.NameToLayer(shelfHighlightLayerName);
    }

    private void Start()
    {
        ui = UIManager.instance;

        ui.DisableUI(ui.skill1Icon);
        ui.DisableUI(ui.skill2Icon);
        ui.DisableUI(ui.skill3Icon);

        //skill1Icon.SetActive(skill1Level > 0);
        //skill2Icon.SetActive(skill2Level > 0);
        //skill3Icon.SetActive(skill3Level > 0);
        
        ui.skill1CooldownSlider.value = 1f;
        ui.skill2CooldownSlider.value = 1f;
        ui.skill3CooldownSlider.value = 1f;
    }

    private void Update()
    {
        if (UserInput.instance.Skill1Input)
        {
            ActivateSkill1();
        }

        if (UserInput.instance.Skill2Input)
        {
            ActivateSkill2();
        }

        if (UserInput.instance.Skill3Input)
        {
            ActivateSkill3();
        }

        for (int i = highlightedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = highlightedObjects[i];

            if (obj == null)
            {
                highlightedObjects.RemoveAt(i);
                continue;
            }

            if (obj.transform.IsChildOf(playerInteraction.transform))
            {
                RestoreLayer(obj);
                highlightedObjects.RemoveAt(i);
            }
        }

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

            ui.UpdateTimerSlider(ui.skill1CooldownSlider, skill1Cooldown, skill1CooldownTimer);
    
            if (skill1CooldownTimer <= 0f)
            {
                skill1CooldownTimer = 0f;
                ui.skill1CooldownSlider.value = 1f;
            }
        }

        if (skill2CooldownTimer > 0f)
        {
            skill2CooldownTimer -= Time.deltaTime;

            ui.UpdateTimerSlider(ui.skill2CooldownSlider, skill2Cooldown, skill2CooldownTimer);


            if (skill2CooldownTimer <= 0f)
            {
                skill2CooldownTimer = 0f;
                ui.skill2CooldownSlider.value = 1f;
            }
        }

        if (skill3CooldownTimer > 0f)
        {
            skill3CooldownTimer -= Time.deltaTime;

            ui.UpdateTimerSlider(ui.skill3CooldownSlider, skill3Cooldown, skill3CooldownTimer);

            if (skill3CooldownTimer <= 0f)
            {
                skill3CooldownTimer = 0f;
                ui.skill3CooldownSlider.value = 1f;
            }
        }
    }

    #region Skill 1
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
            ui.EnableUI(ui.skill1Icon);
        }
        else
        {
            highlightDuration += 2f;
        }

        skill1Price += 50;
        ui.FillSlider(ui.skill1Slider, 5);
    }
    #endregion

    #region Skill 2
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

            if (shelf == null)
            {
                RestoreLayer(item.gameObject);

                highlightedObjects.Remove(item.gameObject);

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
            ui.EnableUI(ui.skill2Icon);
        }
        else
        {
            skill2Cooldown -= 2f;
        }

        skill2Price += 50;
        ui.FillSlider(ui.skill2Slider, 5);
    }
    #endregion

    #region Skill 3
    private void ActivateSkill3()
    {
        if (skill3Level <= 0)
            return;

        if (skill3CooldownTimer > 0f)
            return;

        if (playerInteraction.heldItems.Count == 0)
            return;

        ItemChecker[] shelves = FindObjectsByType<ItemChecker>();

        if (shelves.Length == 0)
            return;

        List<GameObject> backpackItems = new List<GameObject>(playerInteraction.heldItems);
        foreach (GameObject itemObject in backpackItems)
        {
            if (itemObject == null)
                continue;

            Item item = itemObject.GetComponent<Item>();

            if (item == null || item.Data == null)
                continue;

            ItemChecker matchingShelf = null;

            foreach (ItemChecker shelf in shelves)
            {
                if (shelf == null)
                    continue;

                if (shelf.IsItemValid(item))
                {
                    matchingShelf = shelf;
                    break;
                }
            }

            if (matchingShelf == null)
                continue;

            playerInteraction.RemoveItemFromPlayer(itemObject);
            matchingShelf.PlaceItem(item);
        }

        skill3CooldownTimer = skill3Cooldown;
    }

    public void BuySkill3()
    {
        if (skill3Level >= 5)
            return;

        if (currencyManager.coin < skill3Price)
            return;

        currencyManager.ChangeCoin(-skill3Price);

        skill3Level++;

        if (skill3Level == 1)
        {
            ui.EnableUI(ui.skill3Icon);
        }
        else
        {
            skill3Cooldown -= 2f;
        }

        skill3Price += 50;

        ui.FillSlider(ui.skill3Slider, 5);
    }
    #endregion
}