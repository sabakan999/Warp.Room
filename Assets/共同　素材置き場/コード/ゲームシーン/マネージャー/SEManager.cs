using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance;

    [Header("音量")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        // シングルトン化
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

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // 🔊 通常再生
    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.PlayOneShot(clip, masterVolume);
    }

    // 🔊 位置付き再生（3Dっぽくしたい時用）
    public void PlaySEAtPosition(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, pos, masterVolume);
    }

    // 🔊 音量変更
    public void SetVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }
}