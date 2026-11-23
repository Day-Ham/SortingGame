using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class CreditsManager : MonoBehaviour
{
    [Header("Credits Components")]
    [SerializeField] private GameObject creditsText;
    [SerializeField] private CanvasGroup creditsGroup;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button speedUpButton;
    [SerializeField] private TMP_Text speedUpTxt;

    [Header("Move Parameters")]
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveSpeed;
    private int speedMultiplier = 1;

    private UserInput input;
    [SerializeField] private TitleScreenManager titleScreenManager;

    private Sequence playSequence;
    private Sequence stopSequence;

    private Tween moveTween;

    public bool playingCredits;


    private void Awake()
    {
        exitButton.onClick.AddListener(StopCredits);
        speedUpButton.onClick.AddListener(SpeedUpCredits);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = UserInput.instance;

        playSequence = DOTween.Sequence();
        stopSequence = DOTween.Sequence();

        speedMultiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.ResetBindingsInput)
        {
            SpeedUpCredits();
        }
    }

    private void OnEnable()
    {
        PlayCredits();
    }

    private void PlayCredits()
    {
        playingCredits = true;

        playSequence = DOTween.Sequence();
        playSequence.Append(creditsGroup.DOFade(1f, 1f)
            .SetEase(Ease.Linear));
        playSequence.AppendInterval(3f);

        playSequence.AppendCallback(() =>
        {
            moveTween = creditsText.transform.DOMoveY(
                creditsText.transform.position.y + moveDistance,
                moveDistance / moveSpeed
            ).SetEase(Ease.OutQuad);

            moveTween.OnComplete(StopCredits);
        });
    }

    public void StopCredits()
    {
        playingCredits = false;

        playSequence?.Kill();

        stopSequence = DOTween.Sequence();
        stopSequence.Append(creditsGroup.DOFade(0f, 1f)
            .SetEase(Ease.Linear));
        stopSequence.OnComplete(ResetCredits);
    }

    private void ResetCredits()
    {
        DOTween.KillAll();
        creditsText.transform.localPosition = Vector3.zero;
        creditsGroup.alpha = 0f;
        speedMultiplier = 1;
        speedUpTxt.text = "X" + speedMultiplier;

        CloseCredits();
    }

    private void CloseCredits()
    {
        titleScreenManager.OpenMainMenu();
        gameObject.SetActive(false);
    }

    private void SpeedUpCredits()
    {
        speedMultiplier++;

        if (speedMultiplier > 3)
        {
            speedMultiplier = 1;
        }

        speedUpTxt.text = "X" + speedMultiplier;

        Debug.Log("Multiplier: " + speedMultiplier);

        moveTween.timeScale = speedMultiplier;
    }
}
