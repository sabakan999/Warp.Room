using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    [Header("SE再生用")]
    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    // 通常再生
    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    // 位置付き再生
    public void PlaySEAtPosition(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, pos, OptionSettings.SEVolume);
    }
}