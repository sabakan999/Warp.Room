using UnityEngine;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;

    [Header("BGM")]
    public AudioClip normalBGM;
    public AudioClip resultBGM;

    [Header("音量設定")]
    [Range(0f, 1f)] public float masterVolume = 1f;   // 全体音量
    [Range(0f, 1f)] public float normalVolume = 1f;   // 通常BGM音量
    [Range(0f, 1f)] public float resultVolume = 1f;   // リザルト音量

    [Header("フェード設定")]
    public float fadeTime = 0.5f;

    // =========================
    // 🎵 通常BGM
    // =========================
    public void PlayNormalBGM()
    {
        if (bgmSource == null || normalBGM == null) return;

        bgmSource.DOKill();

        bgmSource.Stop();
        bgmSource.clip = normalBGM;
        bgmSource.loop = true;

        bgmSource.volume = 0f;
        bgmSource.Play();

        float targetVolume = masterVolume * normalVolume;

        bgmSource.DOFade(targetVolume, fadeTime);
    }

    // =========================
    // 🎵 リザルトBGM
    // =========================
    public void PlayResultBGM()
    {
        if (bgmSource == null || resultBGM == null) return;

        bgmSource.DOKill();

        bgmSource.Stop();
        bgmSource.clip = resultBGM;
        bgmSource.loop = true;

        bgmSource.volume = 0f;
        bgmSource.Play();

        float targetVolume = masterVolume * resultVolume;

        bgmSource.DOFade(targetVolume, fadeTime);
    }

    // =========================
    // 🔇 停止（フェードアウト）
    // =========================
    public void StopBGM()
    {
        if (bgmSource == null) return;

        bgmSource.DOKill();

        bgmSource.DOFade(0f, fadeTime)
            .OnComplete(() =>
            {
                bgmSource.Stop();
            });
    }

    // =========================
    // 🔊 リアルタイム音量変更
    // =========================
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateVolume();
    }

    void UpdateVolume()
    {
        if (bgmSource == null) return;

        float baseVolume = (bgmSource.clip == normalBGM)
            ? normalVolume
            : resultVolume;

        bgmSource.volume = masterVolume * baseVolume;
    }
}