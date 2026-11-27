using UnityEngine;
using TMPro;

public class PlayerTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private int tutorialCount = 1;
    [SerializeField] GameObject pickUpTutorial;
    [SerializeField] GameObject placeTutorial;
    [SerializeField] GameObject throwTutorial;
    [SerializeField] GameObject changeTutorial;

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
                if (playerInteraction.HasPlacedItem)
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
                pickUpTutorial.SetActive(true);
                placeTutorial.SetActive(false);
                throwTutorial.SetActive(false);
                changeTutorial.SetActive(false);
                break;

            case 2:
                pickUpTutorial.SetActive(false);
                placeTutorial.SetActive(true);
                throwTutorial.SetActive(false);
                changeTutorial.SetActive(false);
                break;

            case 3:
                pickUpTutorial.SetActive(false);
                placeTutorial.SetActive(false);
                throwTutorial.SetActive(true);
                changeTutorial.SetActive(false);
                break;

            case 4:
                pickUpTutorial.SetActive(false);
                placeTutorial.SetActive(false);
                throwTutorial.SetActive(false);
                changeTutorial.SetActive(true);
                break;
        }
    }


}
