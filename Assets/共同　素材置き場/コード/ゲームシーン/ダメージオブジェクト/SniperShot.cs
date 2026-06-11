using UnityEngine;
using UnityEngine.UI;
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
        {
            damageArea.SetActive(false);
        }

        StartCoroutine(SniperRoutine());
    }

    IEnumerator SniperRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        // ====================
        // エイム表示
        // ====================

        if (targetMark != null)
        {
            targetMark.SetActive(true);

            if (targetSR != null)
            {
                Color c = targetSR.color;
                c.a = 0f;
                targetSR.color = c;

                targetSR
                    .DOFade(1f, aimFadeTime)
                    .SetEase(Ease.Linear);

                yield return new WaitForSeconds(aimFadeTime);
            }
        }

        // ====================
        // ロックオン待機
        // ====================

        yield return new WaitForSeconds(shootDelay);

        // ====================
        // 発射
        // ====================

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

        StartCoroutine(DamageRoutine());

        // ====================
        // 着弾跡維持
        // ====================

        yield return new WaitForSeconds(hitStayTime);

        // ====================
        // フェードアウト
        // ====================

        if (hitSR != null)
        {
            hitSR
                .DOFade(0f, hitFadeTime)
                .SetEase(Ease.OutQuad);

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
}