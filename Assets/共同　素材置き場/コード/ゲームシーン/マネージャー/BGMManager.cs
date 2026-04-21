using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;

    [Header("BGM")]
    public AudioClip normalBGM;

    private void Awake()
    {
        // 何もしない（シングルトン廃止）
    }

    // 🔥 通常BGM再生（カウントダウン後に呼ぶ）
    public void PlayNormalBGM()
    {
        if (bgmSource == null || normalBGM == null) return;

        bgmSource.Stop(); // 念のため毎回リセット
        bgmSource.clip = normalBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // 🔥 停止
    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
}