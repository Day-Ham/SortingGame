using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    [SerializeField] private string stage1Scene;
    //[SerializeField] private string stage2Scene;
    //[SerializeField] private string stage3Scene;
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

    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    public void LoadStage1Scene()
    {
        SceneManager.LoadScene(stage1Scene);
    }
    
    //public void LoadStage2Scene()
    //{
    //    SceneManager.LoadScene(stage2Scene);
    //}
    
    //public void LoadStage3Scene()
    //{
    //    SceneManager.LoadScene(stage3Scene);
    //}

    public void QuitGame()
    {
        Application.Quit();
    }
}