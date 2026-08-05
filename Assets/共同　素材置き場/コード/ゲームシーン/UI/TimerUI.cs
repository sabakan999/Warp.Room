using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TimerUI : MonoBehaviour
{
    [Header("ゲージ")]
    public Image gauge;

    [Header("色")]
    public Color normalColor = Color.white;
    public Color bossColor = Color.red;

    private float maxTime;
    private float currentTime;
    private bool isRunning = false;
    public RectTransform timerRoot;

    void Awake()
    {
        if (gauge != null)
            gauge.color = normalColor;
    }

    //==========================
    // 通常タイマー
    //==========================
    public void StartTimer(float duration)
{
    if (gauge != null)
        gauge.enabled = true;

    maxTime = duration;
    currentTime = duration;
    isRunning = true;

    gauge.fillAmount = 1f;
    gauge.color = normalColor;
}

    public void StopTimer()
{
    isRunning = false;

    if (gauge != null)
        gauge.enabled = false;
}

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0, maxTime);

        gauge.fillAmount = currentTime / maxTime;
    }

    //==========================
    // ラスボス演出
    //==========================

    /// <summary>
    /// 時計を震わせる
    /// </summary>
   public void PlayShake(float duration)
{
    if (timerRoot == null)
        return;

    timerRoot.DOKill();

    timerRoot.DOShakeAnchorPos(
        duration,
        8f,
        30,
        90,
        false
    );
}

    /// <summary>
    /// 空ゲージから満タンまで回復
    /// </summary>
    public IEnumerator PlayBossCharge(float duration)
{
    gameObject.SetActive(true);

    isRunning = false;

    gauge.enabled = true;
    gauge.fillAmount = 0f;

    Tween t = gauge
        .DOFillAmount(1f, duration)
        .SetEase(Ease.Linear);

    yield return t.WaitForCompletion();

    // ★ここで60秒タイマー開始
    StartTimer(60f);
}

    /// <summary>
    /// ゲージを赤色へ
    /// </summary>
    public void SetBossMode()
    {
        gauge.DOColor(bossColor, 0.2f);
    }
}