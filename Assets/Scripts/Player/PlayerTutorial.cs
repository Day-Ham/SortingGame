using UnityEngine;
using TMPro;

public class PlayerTutorial : MonoBehaviour
{
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private int tutorialCount = 1;

    PlayerInteraction playerInteraction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInteraction = GetComponent<PlayerInteraction>();  
        UpdateTutorialText();
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInput.instance == null)
            return;

        switch (tutorialCount)
        {
            case 1:
                if (playerInteraction.heldObject != null)
                {
                    tutorialCount++;
                    UpdateTutorialText();
                }
                break;

            case 2:
                if (UserInput.instance.ThrowInput && playerInteraction.LookedAtObject != null 
                    && playerInteraction.LookedAtObject.GetComponent<ItemChecker>() != null)
                {
                    tutorialCount++;
                    UpdateTutorialText();
                }
                break;

            case 3:
                if (UserInput.instance.ThrowInput && (playerInteraction.LookedAtObject == null || playerInteraction.LookedAtObject.GetComponent<ItemChecker>() == null))
                {
                    tutorialCount++;
                    UpdateTutorialText();
                }
                break;

            case 4:
                if (UserInput.instance.ScrollUpInput || UserInput.instance.ScrollDownInput)
                {
                    tutorialPanel.SetActive(false);
                }
                break;
        }
    }

    private void UpdateTutorialText()
    {
        switch (tutorialCount)
        {
            case 1:
                tutorialText.text = "Click LMB to pick up item.";
                break;

            case 2:
                tutorialText.text = "Click RMB to place item.";
                break;

            case 3:
                tutorialText.text = "Click RMB to throw item.";
                break;

            case 4:
                tutorialText.text = "Scroll up or Scroll down to change item.";
                break;
        }
    }
}
