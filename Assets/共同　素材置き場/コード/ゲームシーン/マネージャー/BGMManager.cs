using UnityEngine;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;

    [Header("BGM")]
    public AudioClip normalBGM;
    public AudioClip resultBGM;

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

        // 🔥 フェードイン
        bgmSource.DOFade(1f, fadeTime);
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

        // 🔥 フェードイン
        bgmSource.DOFade(1f, fadeTime);
    }

    // =========================
    // 🔇 停止（フェードアウト）
    // =========================
    public void StopBGM()
    {
        if (bgmSource == null) return;

        bgmSource.DOKill();

        // 🔥 フェードアウトしてから停止
        bgmSource.DOFade(0f, fadeTime)
            .OnComplete(() =>
            {
                bgmSource.Stop();
            });
    }
}