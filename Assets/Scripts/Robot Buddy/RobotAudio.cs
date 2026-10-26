
using UnityEngine;

public class RobotAudio : MonoBehaviour
{
    [Header("Pickup Sound")]
    [SerializeField] private AudioSource pickupAudioSource;
    [SerializeField] private AudioClip pickupClip;

    [Header("Robot sounds")]
    [SerializeField] private AudioSource beepAudioSource;
    [SerializeField] private AudioClip[] beepBoopClips;

    [SerializeField] private float minBeepDelay;
    [SerializeField] private float maxBeepDelay;

    private float beepTimer;

    private void Start()
    {
        ResetBeepTimer();
    }

    private void Update()
    {
        beepTimer -= Time.deltaTime;

        if (beepTimer <= 0f)
        {
            PlayRandomBeep();
            ResetBeepTimer();
        }
    }

    public void PlayPickupSound()
    {
        if (pickupAudioSource != null && pickupClip != null)
        {
            pickupAudioSource.PlayOneShot(pickupClip);
        }
    }

    private void PlayRandomBeep()
    {
        if (beepAudioSource == null || beepBoopClips.Length == 0)
            return;

        AudioClip clip = beepBoopClips[Random.Range(0, beepBoopClips.Length)];

        if (clip != null)
        {
            beepAudioSource.PlayOneShot(clip);
        }
    }

    private void ResetBeepTimer()
    {
        beepTimer = Random.Range(minBeepDelay, maxBeepDelay);
    }
}
