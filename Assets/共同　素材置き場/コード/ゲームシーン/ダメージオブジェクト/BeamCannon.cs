using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BeamCannon : MonoBehaviour
{
    [Header("タイミング")]
    public float activateDelay = 2f;
    public float warningTime = 0.5f;
    public float beamDuration = 1f;

    [Header("見た目")]
    public GameObject cannonVisual;
    public GameObject warningLine;
    public Transform beamVisual;
    public GameObject beamHitbox;

    [Header("ビーム演出")]
    public float beamExtendTime = 0.15f;
    public float beamDisappearTime = 0.2f;

    [Header("発射演出")]
    public float cameraShakeDuration = 0.15f;
    public float cameraShakeStrength = 0.15f;
    public float flashAlpha = 1.5f;

    [Header("発射予兆エフェクト")]
    public GameObject flashEffectPrefab;
    public Transform flashPoint;
    public float flashLifeTime = 0.3f;
    public float flashLeadTime = 0.2f;

    [Header("砲台出現演出")]
    public float appearDistance = 1f;
    public float appearTime = 0.3f;

    private Vector3 cannonStartPos;
    private Vector3 cannonEndPos;
    private Vector3 beamOriginalScale;

    private SpriteRenderer beamRenderer;
    private Color beamOriginalColor;

    void Start()
    {
        SetupInitialState();
        StartCoroutine(FireRoutine());
    }

    void SetupInitialState()
    {
        if (warningLine != null)
            warningLine.SetActive(false);

        if (beamVisual != null)
        {
            beamOriginalScale = beamVisual.localScale;

            beamRenderer = beamVisual.GetComponent<SpriteRenderer>();

            if (beamRenderer != null)
                beamOriginalColor = beamRenderer.color;

            beamVisual.gameObject.SetActive(false);

            beamVisual.localScale = new Vector3(
                0f,
                beamOriginalScale.y,
                beamOriginalScale.z
            );
        }

        if (beamHitbox != null)
            beamHitbox.SetActive(false);

        if (cannonVisual != null)
        {
            cannonEndPos = cannonVisual.transform.localPosition;
            cannonStartPos = cannonEndPos - transform.right * appearDistance;

            cannonVisual.transform.localPosition = cannonStartPos;
            cannonVisual.SetActive(false);
        }
    }

    IEnumerator FireRoutine()
    {
        // 起動待ち
        yield return new WaitForSeconds(activateDelay);

        // 砲台出現
        if (cannonVisual != null)
        {
            cannonVisual.SetActive(true);

            cannonVisual.transform
                .DOLocalMove(cannonEndPos, appearTime)
                .SetEase(Ease.OutBack);
        }

        yield return new WaitForSeconds(appearTime);

        // 警告表示
        if (warningLine != null)
            warningLine.SetActive(true);

        yield return new WaitForSeconds(warningTime);

        // 警告非表示
        if (warningLine != null)
            warningLine.SetActive(false);

        // =========================
        // 発射予兆エフェクト
        // =========================
        if (flashEffectPrefab != null)
        {
            Vector3 pos =
                flashPoint != null ?
                flashPoint.position :
                transform.position;

            Quaternion rot =
                flashPoint != null ?
                flashPoint.rotation :
                transform.rotation;

            GameObject flash = Instantiate(
                flashEffectPrefab,
                pos,
                rot,
                transform.parent
            );

            Animator anim = flash.GetComponent<Animator>();

            if (anim != null &&
                anim.runtimeAnimatorController != null &&
                anim.runtimeAnimatorController.animationClips.Length > 0)
            {
                Destroy(
                    flash,
                    anim.runtimeAnimatorController.animationClips[0].length
                );
            }
            else
            {
                Destroy(flash, flashLifeTime);
            }
        }

        // 発射前の溜め
        yield return new WaitForSeconds(flashLeadTime);

        // カメラシェイク
        if (Camera.main != null)
        {
            Camera.main.transform.DOShakePosition(
                cameraShakeDuration,
                cameraShakeStrength,
                15,
                90,
                false,
                true
            );
        }

        // レーザー表示
        if (beamVisual != null)
        {
            beamVisual.gameObject.SetActive(true);

            if (beamRenderer != null)
            {
                beamRenderer.color = beamOriginalColor;

                Color flashColor = beamOriginalColor;
                flashColor.a = Mathf.Clamp01(flashAlpha);

                beamRenderer.color = flashColor;

                beamRenderer
                    .DOColor(beamOriginalColor, 0.1f)
                    .SetEase(Ease.OutQuad);
            }

            beamVisual.localScale = new Vector3(
                0f,
                beamOriginalScale.y,
                beamOriginalScale.z
            );

            beamVisual
                .DOScaleX(beamOriginalScale.x, beamExtendTime)
                .SetEase(Ease.OutCubic);
        }

        // 判定ON
        if (beamHitbox != null)
            beamHitbox.SetActive(true);

        yield return new WaitForSeconds(beamDuration);

        // 判定OFF
        if (beamHitbox != null)
            beamHitbox.SetActive(false);

        // 消滅演出
        if (beamVisual != null)
        {
            Sequence disappear = DOTween.Sequence();

            disappear.Join(
                beamVisual.DOScaleY(0f, beamDisappearTime)
            );

            if (beamRenderer != null)
            {
                disappear.Join(
                    beamRenderer.DOFade(0f, beamDisappearTime)
                );
            }

            yield return disappear.WaitForCompletion();

            beamVisual.gameObject.SetActive(false);

            beamVisual.localScale = beamOriginalScale;

            if (beamRenderer != null)
                beamRenderer.color = beamOriginalColor;
        }
    }
}