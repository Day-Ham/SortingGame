using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private void Awake()
    {
        ES3AutoSaveMgr.Current.Load();
        ItemChecker[] checkers = FindObjectsByType<ItemChecker>();

        foreach (ItemChecker checker in checkers)
        {
            checker.CleanAndReparent();
        }
    }
    public void SaveGame()
    {
        ES3.Save("SavedScene", SceneManager.GetActiveScene().name);

        ES3AutoSaveMgr.Current.Save();
    }

    public void LoadSaveFile()
    {
        string sceneName = ES3.Load<string>("SavedScene");

        SceneManager.sceneLoaded += OnSaveSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSaveSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSaveSceneLoaded;

        ES3AutoSaveMgr.Current.Load();

        ItemChecker[] checkers = FindObjectsByType<ItemChecker>();

        foreach (ItemChecker checker in checkers)
        {
            checker.CleanAndReparent();
        }
    }
}