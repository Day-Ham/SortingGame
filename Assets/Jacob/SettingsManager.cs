using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [SerializeField] private AudioMixer audioMixer;

    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Audio
    public void SetMasterVolume(float level)
    {
        masterVolume = level;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public void SetMusicVolume(float level)
    {
        musicVolume = level;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public void SetSFXVolume(float level)
    {
        sfxVolume = level;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
    #endregion


}
