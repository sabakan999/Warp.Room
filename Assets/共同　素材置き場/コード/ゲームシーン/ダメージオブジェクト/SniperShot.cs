using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SniperShot : MonoBehaviour
{
    [Header("起動時間")]
    public float startDelay = 2f;

    [Header("予告")]
    public GameObject targetMark;
    public float aimFadeTime = 1f;

    [Header("発射待機")]
    public float shootDelay = 0.5f;

    [Header("着弾")]
    public GameObject hitMark;

    [Header("攻撃判定")]
    public GameObject damageArea;
    public float damageDuration = 0.3f;

    [Header("着弾跡")]
    public float hitStayTime = 1f;
    public float hitFadeTime = 1f;

    [Header("破片エフェクト")]
    public ParticleSystem hitParticle;

    [Header("カメラシェイク")]
    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.35f;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip lockOnSE;
    public AudioClip shotSE;

    private SpriteRenderer targetSR;
    private SpriteRenderer hitSR;

    void Start()
    {
        if (targetMark != null)
        {
            targetSR = targetMark.GetComponent<SpriteRenderer>();
            targetMark.SetActive(false);
        }

        if (hitMark != null)
        {
            hitSR = hitMark.GetComponent<SpriteRenderer>();
            hitMark.SetActive(false);
        }

        if (damageArea != null)
            damageArea.SetActive(false);

        if (hitParticle != null)
            hitParticle.gameObject.SetActive(false);

        StartCoroutine(SniperRoutine());
    }

    IEnumerator SniperRoutine()
    {

        GameManager gm = FindFirstObjectByType<GameManager>();

        yield return new WaitForSeconds(startDelay);

        //====================
        // ターゲット表示
        //====================

        if (targetMark != null)
        {
            targetMark.SetActive(true);
            if (gm != null && gm.isGameRunning)
            {
            PlaySE(lockOnSE);
            }

            if (targetSR != null)
            {
                Color c = targetSR.color;
                c.a = 0f;
                targetSR.color = c;

                targetSR
                    .DOFade(1f, aimFadeTime)
                    .SetEase(Ease.Linear)
                    .SetLink(gameObject);

                yield return new WaitForSeconds(aimFadeTime);
            }
        }

        //====================
        // ロックオン待機
        //====================

       

        yield return new WaitForSeconds(shootDelay);
        if (gm != null && gm.isGameRunning)
        {
         PlaySE(shotSE);
        }

        // 効果音遅延対策
        yield return new WaitForSeconds(0.1f);

        //====================
        // 発射
        //====================

        if (targetMark != null)
            targetMark.SetActive(false);

        if (hitMark != null)
        {
            hitMark.SetActive(true);

            if (hitSR != null)
            {
                Color c = hitSR.color;
                c.a = 1f;
                hitSR.color = c;
            }
        }

        

        // カメラシェイク
        if (Camera.main != null)
        {
            Camera.main.transform
                .DOShakePosition(
                    shakeDuration,
                    shakeStrength
                )
                .SetLink(gameObject);
        }

        // 破片パーティクル
        if (hitParticle != null)
        {
            hitParticle.gameObject.SetActive(true);
            hitParticle.Play();
        }

        StartCoroutine(DamageRoutine());

        //====================
        // 着弾跡維持
        //====================

        yield return new WaitForSeconds(hitStayTime);

        //====================
        // フェードアウト
        //====================

        if (hitSR != null)
        {
            hitSR
                .DOFade(0f, hitFadeTime)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);

            yield return new WaitForSeconds(hitFadeTime);
        }

        Destroy(gameObject);
    }

    IEnumerator DamageRoutine()
    {
        if (damageArea == null)
            yield break;

        damageArea.SetActive(true);

        yield return new WaitForSeconds(damageDuration);

        damageArea.SetActive(false);
    }

    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}