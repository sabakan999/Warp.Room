using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class BeamCannon : MonoBehaviour
{
    [Header("タイミング")]
    public float activateDelay = 2f;
    public float warningTime = 0.5f;
    public float beamDuration = 1f;

    [Header("大砲の位置")]
    public Transform cannonPoint;

    [Header("見た目")]
    public GameObject cannonVisual;
    public GameObject warningLine;
    public Transform beamVisual;
    public GameObject beamHitbox;

    [Header("警告マーク")]
    public Transform warningIcon;
    public WarningDetector warningDetector;

    [Header("警告マーク点滅")]
    public Sprite normalWarningSprite;
    public Sprite flashWarningSprite;

    public int warningFlashCount = 3;
    

    private SpriteRenderer warningIconRenderer;

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

    [Header("SE")]
    public AudioClip warningSE;
    public AudioClip beamSE;

    private Vector3 cannonStartPos;
    private Vector3 cannonEndPos;
    private Vector3 beamOriginalScale;

    private SpriteRenderer beamRenderer;
    private Color beamOriginalColor;

    void Start()
    {
        SetupInitialState();
        StartCoroutine(FireRoutine());
        if(warningIcon != null)
        {
            warningIconRenderer =
                warningIcon.GetComponent<SpriteRenderer>();

            warningIcon.gameObject.SetActive(false);
        }
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
        if (warningIcon != null)
        {
            warningIcon.gameObject.SetActive(false);
        }
    }

    IEnumerator FireRoutine()
    {
        // 起動待ち
        yield return new WaitForSeconds(activateDelay);

        GameManager gm = FindFirstObjectByType<GameManager>();

        // 砲台出現
        if (cannonVisual != null)
        {
            cannonVisual.SetActive(true);

            cannonVisual.transform
                .DOLocalMove(cannonEndPos, appearTime)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
        }

        yield return new WaitForSeconds(appearTime);

        // 警告表示
        if (warningLine != null)
        {
            warningLine.SetActive(true);

            if (warningDetector != null)
            {
                            
                yield return new WaitForFixedUpdate();
                Debug.Log("検出数:" + warningDetector.HitCount());
                UpdateWarningIcon();

                StartCoroutine(
                    WarningFlashRoutine()
                );
            }
        }

        if (warningSE != null && gm != null && gm.isGameRunning)
             MultiSEManager.Instance.PlaySE(warningSE);


        yield return new WaitForSeconds(warningTime);

        // 警告非表示
        if (warningLine != null)
            warningLine.SetActive(false);

        if (warningIcon != null)
            warningIcon.gameObject.SetActive(false);

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

        if (warningSE != null)
         MultiSEManager.Instance.StopSE(warningSE);
        
        if (beamSE != null && gm != null && gm.isGameRunning)
         MultiSEManager.Instance.PlaySE(beamSE);

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
            ).SetLink(gameObject)
            ;
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
                    .SetEase(Ease.OutQuad)
                    .SetLink(gameObject);
            }

            beamVisual.localScale = new Vector3(
                0f,
                beamOriginalScale.y,
                beamOriginalScale.z
            );

            beamVisual
                .DOScaleX(beamOriginalScale.x, beamExtendTime)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject);
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
                .SetLink(gameObject)
            );

            if (beamRenderer != null)
            {
                disappear.Join(
                    beamRenderer.DOFade(0f, beamDisappearTime)
                    .SetLink(gameObject)
                );
            }

            yield return disappear.WaitForCompletion();

            beamVisual.gameObject.SetActive(false);

            beamVisual.localScale = beamOriginalScale;

            if (beamRenderer != null)
                beamRenderer.color = beamOriginalColor;
        }
    }

  void UpdateWarningIcon()
{
    if (warningIcon == null ||
        warningDetector == null)
        return;

        

    List<Vector3> points =
        warningDetector.GetHitPoints();


    Debug.Log("===== Warning Debug =====");

    Debug.Log("BeamCannon(transform) : " + transform.position);

    if (cannonVisual != null)
        Debug.Log("CannonVisual         : " + cannonVisual.transform.position);

    if (warningLine != null)
        Debug.Log("WarningLine          : " + warningLine.transform.position);

    Debug.Log("HitPoint Count       : " + points.Count);

    for (int i = 0; i < points.Count; i++)
    {
        Debug.Log("HitPoint[" + i + "]      : " + points[i]);
    }


    if (points.Count == 0)
    {
        Debug.Log("枠検出なし");
        return;
    }


    Vector3 target;


    // 大砲のワールド座標
    Vector3 cannonPosition;

    if (cannonVisual != null)
    {
        cannonPosition = cannonVisual.transform.position;
    }
    else
    {
        // 念のため未設定なら親を使用
        cannonPosition = transform.position;
    }


    // 枠が1つの場合
    if (points.Count == 1)
    {
        target =
            Vector3.Lerp(
                cannonPosition,
                points[0],
                0.5f
            );
    }
    // 枠が2つ以上の場合
    else
    {
        target =
            Vector3.Lerp(
                points[0],
                points[1],
                0.5f
            );
    }


    Debug.Log("Cannon Used         : " + cannonPosition);
    Debug.Log("Calculated Target   : " + target);


    warningIcon.position = target;

    // ⚠マークは常に画面正面
    warningIcon.rotation = Quaternion.identity;

    warningIcon.gameObject.SetActive(true);
}

IEnumerator WarningFlashRoutine()
{
    if(warningIconRenderer == null)
        yield break;


    float interval =
        warningTime /
        (warningFlashCount * 2);


    for(int i = 0; i < warningFlashCount; i++)
    {
        warningIconRenderer.sprite =
            normalWarningSprite;

        yield return new WaitForSeconds(interval);


        warningIconRenderer.sprite =
            flashWarningSprite;

        yield return new WaitForSeconds(interval);
    }


    warningIconRenderer.sprite =
        normalWarningSprite;
}
    
}