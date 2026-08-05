using UnityEngine;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;

    [Header("BGM")]
    public AudioClip normalBGM;
    public AudioClip resultBGM;
    public AudioClip endlessBGM; // ★追加
    public AudioClip clearBGM;
    public AudioClip bossBGM;

    [Header("音量設定")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float normalVolume = 1f;
    [Range(0f, 1f)] public float resultVolume = 1f;
    [Range(0f, 1f)] public float endlessVolume = 1f; // ★追加
    [Range(0f,1f)]  public float clearVolume = 1f;
    [Range(0f,1f)]public float bossVolume = 1f;

    [Header("フェード設定")]
    public float fadeTime = 0.5f;

    // =========================
    // 🎵 通常 or 無限BGM
    // =========================

    void Start()
{
    masterVolume = OptionSettings.BGMVolume;
}
    public void PlayNormalBGM()
    {
        if (bgmSource == null) return;

        bgmSource.DOKill();

        bgmSource.Stop();

        // 🔥 モードで分岐
        if (GameSettings.isEndlessMode && endlessBGM != null)
        {
            bgmSource.clip = endlessBGM;
        }
        else
        {
            if (normalBGM == null) return;
            bgmSource.clip = normalBGM;
        }

        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float targetVolume = GetCurrentTargetVolume();

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
    // 🔇 停止
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

        bgmSource.volume = GetCurrentTargetVolume();
    }

    // =========================
    // 🔥 現在のBGMに応じた音量
    // =========================
    float GetCurrentTargetVolume()
{
    if (bgmSource.clip == normalBGM)
        return masterVolume * normalVolume;

    if (bgmSource.clip == resultBGM)
        return masterVolume * resultVolume;

    if (bgmSource.clip == endlessBGM)
        return masterVolume * endlessVolume;

    if (bgmSource.clip == clearBGM)
        return masterVolume * clearVolume;

    if (bgmSource.clip == bossBGM)
        return masterVolume * bossVolume;

    return masterVolume;
}
    public void PlayClearBGM()
{
    if (bgmSource == null || clearBGM == null)
        return;

    bgmSource.DOKill();

    bgmSource.Stop();

    bgmSource.clip = clearBGM;
    bgmSource.loop = true;
    bgmSource.volume = 0f;
    bgmSource.Play();

    bgmSource.DOFade(GetCurrentTargetVolume(), fadeTime);
}

public void PlayBossBGM()
{
    if (bgmSource == null || bossBGM == null)
        return;

    bgmSource.DOKill();

    bgmSource.Stop();

    bgmSource.clip = bossBGM;
    bgmSource.loop = true;
    bgmSource.volume = 0f;
    bgmSource.Play();

    bgmSource.DOFade(GetCurrentTargetVolume(), fadeTime);
}
    }