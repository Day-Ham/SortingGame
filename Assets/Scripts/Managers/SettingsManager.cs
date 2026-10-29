using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Mouse Sensitivity")]
    [SerializeField] private float mouseSensitivity = 5f;

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

    #region Sensitivity


    public void SetMouseSensitivity(float value)
    {
        mouseSensitivity = value;
    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }
    #endregion

}
