using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void SaveGame()
    {
        ES3Settings settings = new ES3Settings("SaveFile.es3");

        string sceneName = SceneManager.GetActiveScene().name;

        ES3.Save("SavedScene", sceneName, settings);

        Debug.Log("Saved scene: " + sceneName);

        ES3AutoSaveMgr.Current.Save();

        Debug.Log("Auto Save completed.");
    }


    public void LoadGame()
    {
        ES3Settings settings = new ES3Settings("SaveFile.es3");

        Debug.Log("Save file exists: " + ES3.FileExists(settings));
        Debug.Log("Key exists: " + ES3.KeyExists("SavedScene", settings));

        if (!ES3.KeyExists("SavedScene", settings))
        {
            Debug.LogError("No saved game found.");
            return;
        }

        string sceneName = ES3.Load<string>("SavedScene", settings);

        Debug.Log("Loading scene: " + sceneName);

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
            checker.CleanAndReorganize();
        }
    }
}