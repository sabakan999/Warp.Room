using System.Collections.Generic;
using UnityEngine;

public class MultiSEManager : MonoBehaviour
{
    public static MultiSEManager Instance;

    [Range(0f, 1f)]
    public float volume = 1f;

    // AudioClipごとのAudioSource
    private Dictionary<AudioClip, AudioSource> sourceTable =
        new Dictionary<AudioClip, AudioSource>();

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
        }
    }

    //=========================
    // SE再生
    //=========================
    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
            return;

        AudioSource source;

        // 初めて使うSEならAudioSource生成
        if (!sourceTable.TryGetValue(clip, out source))
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;

            sourceTable.Add(clip, source);
        }

        source.volume = volume;

        // 同じSEが鳴っていたら重ねない
       
        source.clip = clip;
        source.Play();
    }

    //=========================
    // 特定SE停止
    //=========================
    public void StopSE(AudioClip clip)
    {
        if (clip == null)
            return;

        if (sourceTable.TryGetValue(clip, out AudioSource source))
        {
            source.Stop();
        }
    }

    //=========================
    // 全停止
    //=========================
    public void StopAllSE()
    {
        foreach (AudioSource source in sourceTable.Values)
        {
            source.Stop();
        }
    }

    //=========================
    // 音量変更
    //=========================
    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);

        foreach (AudioSource source in sourceTable.Values)
        {
            source.volume = volume;
        }
    }
}