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
    public AudioClip lockOnSE;
    public AudioClip shotSE;


    private SpriteRenderer targetSR;
    private SpriteRenderer hitSR;


    void Start()
    {
        if (targetMark != null)
        {
            targetSR =
                targetMark.GetComponent<SpriteRenderer>();

            targetMark.SetActive(false);
        }

        if (hitMark != null)
        {
            hitSR =
                hitMark.GetComponent<SpriteRenderer>();

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
        GameManager gm =
            FindFirstObjectByType<GameManager>();


        // ========================================
        // 起動待機
        // ========================================

        yield return new WaitForSeconds(startDelay);


        // ========================================
        // ターゲット表示
        // ========================================

        if (targetMark != null)
        {
            targetMark.SetActive(true);

            if (gm != null &&
                gm.isGameRunning)
            {
                PlaySE(lockOnSE);
            }


            if (targetSR != null)
            {
                Color c = targetSR.color;
                c.a = 0f;
                targetSR.color = c;


                targetSR
                    .DOFade(
                        1f,
                        aimFadeTime
                    )
                    .SetEase(Ease.Linear)
                    .SetLink(gameObject);


                yield return new WaitForSeconds(
                    aimFadeTime
                );
            }
        }


        // ========================================
        // 発射待機
        // ========================================

        yield return new WaitForSeconds(
            shootDelay
        );


        if (gm != null &&
            gm.isGameRunning)
        {
            PlaySE(shotSE);
        }


        // 効果音遅延対策
        yield return new WaitForSeconds(0.1f);


        // ========================================
        // 発射
        // ========================================

        if (targetMark != null)
            targetMark.SetActive(false);


        // ========================================
        // 着弾マーク表示
        // ========================================

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


        // ========================================
        // 破片パーティクル
        // ========================================

        if (hitParticle != null)
        {
            hitParticle.gameObject.SetActive(true);
            hitParticle.Play();
        }


        // ========================================
        // ダメージ判定
        // ========================================

        StartCoroutine(
            DamageRoutine()
        );


        // ========================================
        // カメラシェイク
        //
        // 着弾演出と同時に開始
        // ========================================

        StartCameraShake();


        // ========================================
        // 着弾跡維持
        // ========================================

        yield return new WaitForSeconds(
            hitStayTime
        );


        // ========================================
        // フェードアウト
        // ========================================

        if (hitSR != null)
        {
            hitSR
                .DOFade(
                    0f,
                    hitFadeTime
                )
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);


            yield return new WaitForSeconds(
                hitFadeTime
            );
        }


        // ========================================
        // 最後に自身を破棄
        // ========================================

        Destroy(gameObject);
    }


    // ========================================
    // カメラシェイク
    // ========================================

    void StartCameraShake()
    {
        if (Camera.main == null)
            return;


        Transform cameraTransform =
            Camera.main.transform;


        // ----------------------------------------
        // シェイク開始前のカメラ状態を保存
        // ----------------------------------------

        Vector3 originalCameraPosition =
            cameraTransform.position;

        Quaternion originalCameraRotation =
            cameraTransform.rotation;


        // ----------------------------------------
        // 念のため既存のカメラTweenを停止
        // ----------------------------------------

        cameraTransform.DOKill();


        // ----------------------------------------
        // カメラシェイク
        //
        // SniperShot自身とはリンクさせない
        // ----------------------------------------

        cameraTransform
            .DOShakePosition(
                shakeDuration,
                shakeStrength
            )
            .OnComplete(() =>
            {
                // --------------------------------
                // シェイク終了後に必ず元へ戻す
                // --------------------------------

                cameraTransform.position =
                    originalCameraPosition;

                cameraTransform.rotation =
                    originalCameraRotation;
            });
    }


    // ========================================
    // ダメージ判定
    // ========================================

    IEnumerator DamageRoutine()
    {
        if (damageArea == null)
            yield break;


        damageArea.SetActive(true);


        yield return new WaitForSeconds(
            damageDuration
        );


        damageArea.SetActive(false);
    }


    // ========================================
    // SE
    // ========================================

    void PlaySE(AudioClip clip)
    {
        if (clip == null)
            return;


        if (MultiSEManager.Instance != null)
        {
            MultiSEManager.Instance.PlaySE(clip);
        }
    }
}