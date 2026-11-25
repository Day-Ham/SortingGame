using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreenObj;
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private Slider loadingProgressBar; 
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

    private void Start()
    {
        loadingCanvasGroup.alpha = 0f;
        loadingScreenObj.SetActive(false);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        loadingScreenObj.SetActive(true);
        loadingProgressBar.value = 0;

        StartCoroutine(FadeInAndLoadAsyc(sceneIndex));
    }

    IEnumerator FadeInAndLoadAsyc(int sceneIndex)
    {
        yield return loadingCanvasGroup.DOFade(1f, 0.5f).WaitForCompletion();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            loadingProgressBar.value = asyncLoad.progress/0.9f;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        yield return loadingCanvasGroup.DOFade(0f, 0.5f).WaitForCompletion();
        loadingScreenObj.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}