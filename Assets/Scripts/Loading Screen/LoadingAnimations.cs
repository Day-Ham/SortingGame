using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimations : MonoBehaviour
{
    public static LoadingAnimations Instance { get; private set; }

    [Header("Robot Buddy")]
    [SerializeField] private RectTransform handle;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 2f;
    private float startY;

    [Header("Loading")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private float interval = 1f;
    private int periodCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startY = handle.anchoredPosition.y;
        InvokeRepeating(nameof(UpdateLoadingText), 1f, interval);
    }
    
    // Update is called once per frame
    void Update()
    {
        //Robot Buddy Animation
        Vector2 position = handle.anchoredPosition;
        float yOffset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        handle.anchoredPosition = new Vector2(position.x, startY + yOffset);
    }

    private void UpdateLoadingText()
    {
        periodCount = (periodCount + 1) % 4;
        loadingText.text = "LOADING" + new string('.', periodCount);
    }
}
