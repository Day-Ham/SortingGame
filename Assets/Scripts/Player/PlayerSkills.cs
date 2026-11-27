using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSkills : MonoBehaviour
{
    [Header("Skill 1")]
    [SerializeField] private float highlightDuration = 10;
    private float highlightTimer;
    [SerializeField] private float skill1Cooldown = 20f;
    private float skill1CooldownTimer;
    [SerializeField] private int skill1Level;
    [SerializeField] private int skill1InitialPrice;
    [SerializeField] private int skill1CurrentPrice;
    [SerializeField] private string highlightLayerName = "Highlight";
    [SerializeField] private string shelfHighlightLayerName = "ShelfHighlight";
    private int highlightLayer;
    private int shelfHighlightLayer;
    private List<GameObject> highlightedObjects = new List<GameObject>();
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();
    [SerializeField] private GameObject skill1Icon;
    [SerializeField] private Slider skill1CooldownSlider;
    [SerializeField] private TMP_Text skill1PriceText;
    [SerializeField] private Slider skill1BuySlider;

    [Header("Skill 2")]
    [SerializeField] private float skill2Cooldown = 20f;
    private float skill2CooldownTimer;
    [SerializeField] private int skill2Level;
    [SerializeField] private int skill2InitialPrice;
    [SerializeField] private int skill2CurrentPrice;
    [SerializeField] private GameObject skill2Icon;
    [SerializeField] private Slider skill2CooldownSlider;
    [SerializeField] private TMP_Text skill2PriceText;
    [SerializeField] private Slider skill2BuySlider;

    [Header("Skill 3")]
    [SerializeField] private float skill3Cooldown = 30f;
    private float skill3CooldownTimer;
    [SerializeField] private int skill3Level;
    [SerializeField] private int skill3InitialPrice;
    [SerializeField] private int skill3CurrentPrice;
    [SerializeField] private GameObject skill3Icon;
    [SerializeField] private Slider skill3CooldownSlider;
    [SerializeField] private TMP_Text skill3PriceText;
    [SerializeField] private Slider skill3BuySlider;
    private bool skill3Used;

    [Header("Points")]
    [SerializeField] private int totalPointsSpent;

    [Header("Reset")]
    [SerializeField] private Button resetButton;
    public GameObject resetPrompt;
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;

    [Header("Other UI Elements")]
    public GameObject skillsMenu;
    [SerializeField] private Button closeButton;

    [Header("Steam Manager Check")]
    [SerializeField] private bool skillUsed;

    //References
    private PlayerInteraction playerInteraction;
    private CurrencyAndShelvedCountManager currencyAndShelvedManager;

    private void Awake()
    {
        highlightLayer = LayerMask.NameToLayer(highlightLayerName);
        shelfHighlightLayer = LayerMask.NameToLayer(shelfHighlightLayerName);

        playerInteraction = GetComponent<PlayerInteraction>();
        currencyAndShelvedManager = FindAnyObjectByType<CurrencyAndShelvedCountManager>();
    }

    private void Start()
    {
        resetButton.onClick.AddListener(OpenResetPrompt);
        confirmResetButton.onClick.AddListener(ResetSkills);
        cancelResetButton.onClick.AddListener(CloseResetPrompt);
        closeButton.onClick.AddListener(CloseSkillsMenu);

        skill1CooldownSlider.value = 1f;
        skill2CooldownSlider.value = 1f;
        skill3CooldownSlider.value = 1f;

        OnLoading();
        UpdateUIPrices();
    }

    private void Update()
    {
        if (UserInput.instance == null && PauseManager.instance == null) return;

        if (UserInput.instance.ResetBindingsInput)
        {
            if (skillsMenu.activeInHierarchy && !resetPrompt.activeInHierarchy)
                OpenResetPrompt();
        }

        if (UserInput.instance.EscapeInput)
        {
            if (resetPrompt.activeInHierarchy)
            {
                CloseResetPrompt();
                return;
            }
            else if (skillsMenu.activeInHierarchy)
            {
                CloseSkillsMenu();
            }
        }

        if (UserInput.instance.SkillInput && !PauseManager.instance.isPaused)
        {
            if (!skillsMenu.activeInHierarchy)
            {
                OpenSkillsMenu();
            }
            else if (skillsMenu.activeInHierarchy && !resetPrompt.activeInHierarchy)
            {
                CloseSkillsMenu();
            }
        }

        if (UserInput.instance != null)
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

            UpdateCooldownSlider(skill1CooldownSlider, skill1Cooldown, skill1CooldownTimer);
    
            if (skill1CooldownTimer <= 0f)
            {
                skill1CooldownTimer = 0f;
                skill1CooldownSlider.value = 1f;
            }
        }

        if (skill2CooldownTimer > 0f)
        {
            skill2CooldownTimer -= Time.deltaTime;

            UpdateCooldownSlider(skill2CooldownSlider, skill2Cooldown, skill2CooldownTimer);

            if (skill2CooldownTimer <= 0f)
            {
                skill2CooldownTimer = 0f;
                skill2CooldownSlider.value = 1f;
            }
        }

        if (skill3CooldownTimer > 0f)
        {
            skill3CooldownTimer -= Time.deltaTime;

            UpdateCooldownSlider(skill3CooldownSlider, skill3Cooldown, skill3CooldownTimer);

            if (skill3CooldownTimer <= 0f)
            {
                skill3CooldownTimer = 0f;
                skill3CooldownSlider.value = 1f;
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

        skillUsed = true;

        if(SteamManager.instance != null)
            SteamManager.instance.SkillWasUsed();
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

        if (currencyAndShelvedManager.skillPoints < skill1CurrentPrice)
            return;

        currencyAndShelvedManager.ChangeSkillPoints(-skill1CurrentPrice);
        totalPointsSpent += skill1CurrentPrice;

        skill1Level++;

        if (skill1Level == 1)
        {
            skill1Icon.SetActive(true);
        }
        else
        {
            skill1Cooldown -= 2f;
            highlightDuration += 2f;
        }

        skill1CurrentPrice += Mathf.CeilToInt(skill1CurrentPrice * 0.5f);

        UpdateUIPrices();

        FillBuySlider(skill1BuySlider, 5);

        if (SteamManager.instance != null)
            SteamManager.instance.SkillPurchased(1, skill1Level, 5);

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
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

        StartCoroutine(PickUpMatchingItems(displayName, allItems));              
    }

    private IEnumerator PickUpMatchingItems(string displayName, Item[] allItems)
    {
        foreach (Item item in allItems)
        {
            if (playerInteraction.heldItems.Count >= playerInteraction.MaxHeldItems)
                break;

            if (item == null || item.Data == null)
                continue;

            if (item.Data.displayName != displayName)
                continue;

            if (playerInteraction.heldItems.Contains(item.gameObject))
                continue;

            ItemChecker shelf = item.GetComponentInParent<ItemChecker>();

            // Only pick up items that are not on a shelf.
            if (shelf != null) continue;

            // Wait picking up or switching actin to finish.
            yield return new WaitUntil(() => !playerInteraction.isPickingUp && !playerInteraction.isSwitching);

            // Recheck inventory after waiting.
            if (playerInteraction.heldItems.Count >= playerInteraction.MaxHeldItems)
                break;

            if (item == null || playerInteraction.heldItems.Contains(item.gameObject))
                continue;

            // Restore the item's original layer and remove its highlight.
            RestoreLayer(item.gameObject);
            highlightedObjects.Remove(item.gameObject);

            // Attempt to pick up the item.
            playerInteraction.TryPickup(item);

            // Wait until the item has actually finished moving into the player's hand.
            yield return new WaitUntil(() => !playerInteraction.isPickingUp);
        }

        skillUsed = true;

        if (SteamManager.instance != null)
            SteamManager.instance.SkillWasUsed();
        

        skill2CooldownTimer = skill2Cooldown;
    }

    public void BuySkill2()
    {
        if (skill2Level >= 5)
            return;

        if (currencyAndShelvedManager.skillPoints < skill2CurrentPrice)
            return;

        currencyAndShelvedManager.ChangeSkillPoints(-skill2CurrentPrice);
        totalPointsSpent += skill2CurrentPrice;

        skill2Level++;

        if (skill2Level == 1)
        {
            skill2Icon.SetActive(true);
        }
        else
        {
            skill2Cooldown -= 2f;
        }

        skill2CurrentPrice += Mathf.CeilToInt(skill2CurrentPrice * 0.5f);

        UpdateUIPrices();

        FillBuySlider(skill2BuySlider, 5);

        if (SteamManager.instance != null)
            SteamManager.instance.SkillPurchased(2, skill2Level, 5);
        

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
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

        skill3Used = false;
        StartCoroutine(ActivateSkill3Routine());
    }

    IEnumerator ActivateSkill3Routine()
    {
        ItemChecker[] shelves = FindObjectsByType<ItemChecker>()
            .OrderByDescending(shelf => shelf.heldItems.Count) 
            .ToArray();

        if (shelves.Length == 0) yield break;

        List<GameObject> backpackItems = new List<GameObject>(playerInteraction.heldItems);
        foreach (GameObject itemObject in backpackItems)
        {
            if (itemObject == null) continue;

            Item item = itemObject.GetComponent<Item>();

            if (item == null || item.Data == null) continue;

            ItemChecker matchingShelf = null;

            foreach (ItemChecker shelf in shelves)
            {
                if (shelf == null) continue;

                if (shelf.IsItemValid(item))
                {
                    matchingShelf = shelf;
                    break;
                }
                else
                {
                    continue;
                }
            }

            if (matchingShelf == null) continue;

            yield return new WaitUntil(() => !matchingShelf.isPlacingItem && !playerInteraction.isPickingUp && !playerInteraction.isSwitching);
            
            playerInteraction.RemoveItemFromPlayer(itemObject);
            matchingShelf.PlaceItem(item);

            skill3Used = true;
            
            skillUsed = true;

            if(SteamManager.instance != null)
                SteamManager.instance.SkillWasUsed();

            if (skill3Used)
                skill3CooldownTimer = skill3Cooldown;

            yield return new WaitUntil(() => !matchingShelf.isPlacingItem && !playerInteraction.isPickingUp && !playerInteraction.isSwitching);
        }
    }

    public void BuySkill3()
    {
        if (skill3Level >= 5)
            return;

        if (currencyAndShelvedManager.skillPoints < skill3CurrentPrice)
            return;

        currencyAndShelvedManager.ChangeSkillPoints(-skill3CurrentPrice);
        totalPointsSpent += skill3CurrentPrice;

        skill3Level++;

        if (skill3Level == 1)
        {
            skill3Icon.SetActive(true);
        }
        else
        {
            skill3Cooldown -= 2f;
        }

        skill3CurrentPrice += Mathf.CeilToInt(skill3CurrentPrice * 0.5f);

        UpdateUIPrices();

        FillBuySlider(skill3BuySlider, 5);

        if (SteamManager.instance != null)
            SteamManager.instance.SkillPurchased(3, skill3Level, 5);

        if (SaveManager.instance != null)
            SaveManager.instance.SaveGame();
    }
    #endregion
    private void UpdateUIPrices()
    {
        skill1PriceText.text = skill1CurrentPrice.ToString("F0");
        skill2PriceText.text = skill2CurrentPrice.ToString("F0");
        skill3PriceText.text = skill3CurrentPrice.ToString("F0");
    }

    #region Menu and Prompt Handling
    private void OpenSkillsMenu()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.SetAsCurrentOpenMenu(skillsMenu);
        }
    }

    private void CloseSkillsMenu()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.CloseMenu(skillsMenu);
        }
    }

    private void OpenResetPrompt()
    {
        if (GameUIManager.instance != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameUIManager.instance.TweenOpen(resetPrompt);
        }
    }

    private void CloseResetPrompt()
    {
        if (GameUIManager.instance != null)
        {
            GameUIManager.instance.TweenClose(resetPrompt);
        }
    }
    #endregion

    public void OnLoading()
    {
        if (skillUsed)
        {
            if(SteamManager.instance)
                SteamManager.instance.SkillWasUsed();
        }

        if (skill1Level > 0) skill1Icon.SetActive(true);
        else skill1Icon.SetActive(false);

        if (skill2Level > 0) skill2Icon.SetActive(true);
        else skill2Icon.SetActive(false);

        if (skill3Level > 0) skill3Icon.SetActive(true);
        else skill3Icon.SetActive(false);

        for (int i = 0; i < skill1Level; i++)
        {
            FillBuySlider(skill1BuySlider, 5);
            if (SteamManager.instance)
                SteamManager.instance.SkillPurchased(1, skill1Level, 5);
        }

        for (int i = 0; i < skill2Level; i++)
        {
            FillBuySlider(skill2BuySlider, 5);
            if (SteamManager.instance)
                SteamManager.instance.SkillPurchased(2, skill2Level, 5);
        }

        for (int i = 0; i < skill3Level; i++)
        {
            FillBuySlider(skill3BuySlider, 5);
            if (SteamManager.instance)
                SteamManager.instance.SkillPurchased(3, skill3Level, 5);
        }

        UpdateUIPrices();
    }

    public void UpdateCooldownSlider(Slider slider, float cooldownTime, float cooldownTimer)
    {
        slider.value = 1f - (cooldownTimer / cooldownTime);
    }

    public void FillBuySlider(Slider slider, int maxValue)
    {
        slider.value += 1f / maxValue;
    }

    public void ResetSkills()
    {
        //Skill 1
        skill1Level = 0;
        skill1Icon.SetActive(false);
        highlightDuration = 10f;
        skill1CurrentPrice = skill1InitialPrice;
        skill1BuySlider.value = 0f;

        //Skill 2
        skill2Level = 0;
        skill2Icon.SetActive(false); ;
        skill2Cooldown = 20f;
        skill2CurrentPrice = skill2InitialPrice;
        skill2BuySlider.value = 0f;

        //Skill 3
        skill3Level = 0;
        skill3Icon.SetActive(false);
        skill3Cooldown = 30f;
        skill3CurrentPrice = skill3InitialPrice;
        skill3BuySlider.value = 0f;

        currencyAndShelvedManager.ChangeSkillPoints(totalPointsSpent);
        totalPointsSpent = 0;

        UpdateUIPrices();
        CloseResetPrompt();
    }

    //QA EDITOR
    public void BuyMaxSkills()
    {
        ResetSkills();

        //Skill 1
        for (int i = 0; i<5; i++)
        {
            if (skill1Level >= 5) break;

            skill1Level++;

            if (skill1Level == 1)
            {
                skill1Icon.SetActive(true);
            }
            else
            {
                highlightDuration += 2f;
            }

            skill1CurrentPrice += Mathf.CeilToInt(skill1CurrentPrice * 0.5f);
            FillBuySlider(skill1BuySlider, 5);
            if (SteamManager.instance)
                SteamManager.instance.SkillPurchased(1, skill1Level, 5);
        }

        //Skill 2
        for (int i = 0; i<5; i++)
        {
            if (skill2Level >= 5) break;

            skill2Level++;

            if (skill2Level == 1)
            {
                skill2Icon.SetActive(true);
            }
            else
            {
                skill2Cooldown -= 2f;
            }

            skill2CurrentPrice += Mathf.CeilToInt(skill2CurrentPrice * 0.5f);
            FillBuySlider(skill2BuySlider, 5);
            if (SteamManager.instance)
                SteamManager.instance.SkillPurchased(2, skill2Level, 5);
        }

        //Skill 3
        for (int i = 0; i<5; i++)
        {
            if (skill3Level >= 5) break;

            skill3Level++;

            if (skill3Level == 1)
            {
                skill3Icon.SetActive(true);
            }
            else
            {
                skill3Cooldown -= 2f;
            }

            skill3CurrentPrice += Mathf.CeilToInt(skill3CurrentPrice * 0.5f);

            FillBuySlider(skill3BuySlider, 5);
            if (SteamManager.instance)
                SteamManager.instance.SkillPurchased(3, skill3Level, 5);
        }
    }
}