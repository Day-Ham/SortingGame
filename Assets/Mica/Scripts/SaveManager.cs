using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        ES3Settings settings = new ES3Settings("SaveFile.es3");

        ES3AutoSaveMgr.Current.Save();

        Debug.Log("Auto Save completed.");
    }

    public void LoadGame(int index)
    {
        SceneManager.sceneLoaded += OnSaveSceneLoaded;
        SceneManager.LoadScene(index);
    }

    private void OnSaveSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSaveSceneLoaded;

        ES3AutoSaveMgr.Current.Load();

        ItemChecker[] checkers = FindObjectsByType<ItemChecker>();

        foreach (ItemChecker checker in checkers)
        {
            checker.CleanAndReorganize();
        }
    }
}