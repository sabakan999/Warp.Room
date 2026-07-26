using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OptionBGM : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        ApplyVolume();
    }

    void Update()
    {
        // 設定画面から変更されたらリアルタイム反映
        ApplyVolume();
    }

    void ApplyVolume()
    {
        audioSource.volume = OptionSettings.BGMVolume;
    }
}