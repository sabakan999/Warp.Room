using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OptionSE : MonoBehaviour
{
    private AudioSource audioSource;
    private float currentVolume = -1f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (currentVolume != OptionSettings.SEVolume)
        {
            currentVolume = OptionSettings.SEVolume;
            audioSource.volume = currentVolume;
        }
    }
}