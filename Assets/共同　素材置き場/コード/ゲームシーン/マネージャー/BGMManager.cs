using UnityEngine;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;


    [Header("BGM")]
    public AudioClip normalBGM;
    public AudioClip resultBGM;
    public AudioClip endlessBGM;
    public AudioClip clearBGM;
    public AudioClip bossBGM;

    // ★追加
    public AudioClip endRollBGM;



    [Header("音量設定")]
    [Range(0f,1f)]
    public float masterVolume = 1f;

    [Range(0f,1f)]
    public float normalVolume = 1f;

    [Range(0f,1f)]
    public float resultVolume = 1f;

    [Range(0f,1f)]
    public float endlessVolume = 1f;

    [Range(0f,1f)]
    public float clearVolume = 1f;

    [Range(0f,1f)]
    public float bossVolume = 1f;

    // ★追加
    [Range(0f,1f)]
    public float endRollVolume = 1f;



    [Header("フェード")]
    public float fadeTime = 0.5f;



    void Start()
    {
        masterVolume = OptionSettings.BGMVolume;
    }



    //=========================
    // 通常BGM
    //=========================

    public void PlayNormalBGM()
    {
        if (bgmSource == null)
            return;


        bgmSource.DOKill();
        bgmSource.Stop();


        if(GameSettings.isEndlessMode && endlessBGM != null)
            bgmSource.clip = endlessBGM;
        else
            bgmSource.clip = normalBGM;


        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();


        bgmSource.DOFade(
            GetCurrentTargetVolume(),
            fadeTime
        );
    }
    // =========================
// 🎵 リザルトBGM
// =========================
public void PlayResultBGM()
{
    if (bgmSource == null || resultBGM == null)
        return;


    bgmSource.DOKill();

    bgmSource.Stop();


    bgmSource.clip = resultBGM;

    bgmSource.loop = true;

    bgmSource.volume = 0f;

    bgmSource.Play();


    bgmSource.DOFade(
        masterVolume * resultVolume,
        fadeTime
    );
}



    //=========================
    // ボスBGM
    //=========================

    public void PlayBossBGM()
    {
        PlayClip(
            bossBGM,
            bossVolume
        );
    }



    //=========================
    // エンドロールBGM
    //=========================

   public void PlayEndRollBGM()
{
    PlayClip(
        endRollBGM,
        endRollVolume,
        false
    );
}




    //=========================
    // クリアBGM
    //=========================

    public void PlayClearBGM()
    {
        PlayClip(
            clearBGM,
            clearVolume
        );
    }



    //=========================
    // 共通再生
    //=========================

    void PlayClip(AudioClip clip, float volume, bool loop = true)
    {
        if(bgmSource == null || clip == null)
            return;


        bgmSource.DOKill();

        bgmSource.Stop();


        bgmSource.clip = clip;

        bgmSource.loop = loop;

        bgmSource.volume = 0f;


        bgmSource.Play();


        bgmSource.DOFade(
            masterVolume * volume,
            fadeTime
        );
    }




    //=========================
    // 停止
    //=========================

    public void StopBGM()
    {
        if(bgmSource == null)
            return;


        bgmSource.DOKill();


        bgmSource.DOFade(
            0f,
            fadeTime
        )
        .OnComplete(() =>
        {
            bgmSource.Stop();
        });
    }




    //=========================
    // 音量変更
    //=========================

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateVolume();
    }


    void UpdateVolume()
    {
        if(bgmSource == null)
            return;

        bgmSource.volume =
            GetCurrentTargetVolume();
    }




    float GetCurrentTargetVolume()
    {

        if(bgmSource.clip == normalBGM)
            return masterVolume * normalVolume;


        if(bgmSource.clip == resultBGM)
            return masterVolume * resultVolume;


        if(bgmSource.clip == endlessBGM)
            return masterVolume * endlessVolume;


        if(bgmSource.clip == clearBGM)
            return masterVolume * clearVolume;


        if(bgmSource.clip == bossBGM)
            return masterVolume * bossVolume;


        if(bgmSource.clip == endRollBGM)
            return masterVolume * endRollVolume;


        return masterVolume;
    }
}