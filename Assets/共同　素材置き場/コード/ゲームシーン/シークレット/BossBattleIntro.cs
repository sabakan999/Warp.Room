using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BossBattleIntro : MonoBehaviour
{
    [Header("参照（自動取得）")]
    private TimerUI timerUI;
    private BossController bossController;
    private BGMManager bgmManager;
    private AudioSource audioSource;

    [Header("ボス見た目")]
    public GameObject bossVisual;

    [Header("演出時間")]
    public float shakeTime = 0.8f;
    public float chargeTime = 2.5f;
    public float bossAppearDelay = 0.5f;
    public float battleStartDelay = 2f;

    [Header("SE")]
    public AudioClip bossAppearSE;

    bool isPlaying = false;

  public void StartIntro()
{
    if (isPlaying) return;

    timerUI ??= FindFirstObjectByType<TimerUI>();
    bossController ??= FindFirstObjectByType<BossController>();
    bgmManager ??= FindFirstObjectByType<BGMManager>();

    if (audioSource == null)
    {
        GameObject se = GameObject.Find("SE");
        if (se != null)
            audioSource = se.GetComponent<AudioSource>();
    }

    Debug.Log("===== BossBattleIntro =====");

    Debug.Log($"TimerUI        : {(timerUI != null ? timerUI.name : "NULL")}");
    Debug.Log($"BossController : {(bossController != null ? bossController.name : "NULL")}");
    Debug.Log($"BGMManager     : {(bgmManager != null ? bgmManager.name : "NULL")}");
    Debug.Log($"AudioSource    : {(audioSource != null ? audioSource.gameObject.name : "NULL")}");
    Debug.Log($"BossVisual     : {(bossVisual != null ? bossVisual.name : "NULL")}");

    isPlaying = true;
    StartCoroutine(IntroSequence());
}

  

    IEnumerator IntroSequence()
    {
        //--------------------------------------------------
        // ① 時計を震わせる
        //--------------------------------------------------
        if (timerUI != null)
            timerUI.PlayShake(shakeTime);

        yield return new WaitForSeconds(shakeTime);

        //--------------------------------------------------
        // ② 時計を空→満タンへ
        //--------------------------------------------------
        if (timerUI != null)
            yield return timerUI.PlayBossCharge(chargeTime);

        //--------------------------------------------------
        // ③ 時計を赤色へ
        //--------------------------------------------------
        if (timerUI != null)
            timerUI.SetBossMode();

        yield return new WaitForSeconds(0.3f);

        //--------------------------------------------------
        // ④ ボス登場
        //--------------------------------------------------
        if (bossVisual != null)
        {
            bossVisual.SetActive(true);
            bossVisual.transform.localScale = Vector3.zero;

            bossVisual.transform
                .DOScale(1.2f, 0.25f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    bossVisual.transform.DOScale(1f, 0.1f);
                });
        }

        if (audioSource != null && bossAppearSE != null)
            audioSource.PlayOneShot(bossAppearSE);

        yield return new WaitForSeconds(bossAppearDelay);

        //--------------------------------------------------
        // ⑤ ボスBGM
        //--------------------------------------------------
        if (bgmManager != null)
            bgmManager.PlayBossBGM();

        yield return new WaitForSeconds(battleStartDelay);

        //--------------------------------------------------
        // ⑥ 戦闘開始
        //--------------------------------------------------
        if (bossController != null)
            bossController.BeginBattle();

        isPlaying = false;
    }
}