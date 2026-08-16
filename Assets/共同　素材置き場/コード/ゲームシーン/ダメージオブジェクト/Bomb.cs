using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Bomb : MonoBehaviour
{
    [Header("爆発時間")]
    public float explodeTime = 3f;

    [Header("スプライト")]
    public Sprite normalSprite;
    public Sprite warningSprite;

    [Header("爆弾本体")]
    public SpriteRenderer bombSprite;

    [Header("カウント表示")]
    public Text countdownText;

    [Header("爆炎")]
    public GameObject upFire;
    public GameObject downFire;
    public GameObject leftFire;
    public GameObject rightFire;

    [Header("爆炎方向")]
    public bool useUp = true;
    public bool useDown = true;
    public bool useLeft = true;
    public bool useRight = true;

    [Header("爆炎持続")]
    public float fireDuration = 1f;

    [Header("警告演出")]
    public float warningScale = 1.2f;

    [Header("カメラシェイク")]
    public float shakeDuration = 0.2f;
    public float shakeStrength = 0.4f;

    [Header("SE")]
    public AudioClip warningSE;
    public AudioClip explodeSE;

    private float timer;
    private bool exploded;
    private bool warningStarted;

    private Vector3 bombOriginalScale;


    void Start()
    {
        timer = explodeTime;

        if (bombSprite == null)
            bombSprite = GetComponent<SpriteRenderer>();

        if (bombSprite != null)
            bombOriginalScale = bombSprite.transform.localScale;

        DisableAllFire();

        if (bombSprite != null && normalSprite != null)
            bombSprite.sprite = normalSprite;
    }


    void Update()
    {
        if (exploded)
            return;

        timer -= Time.deltaTime;

        if (countdownText != null)
        {
            countdownText.text =
                Mathf.Max(timer, 0f).ToString("F1");
        }

        GameManager gm =
            FindFirstObjectByType<GameManager>();


        // ========================================
        // 残り1秒
        // ========================================

        if (timer <= 1f && !warningStarted)
        {
            warningStarted = true;

            if (bombSprite != null && warningSprite != null)
                bombSprite.sprite = warningSprite;


            if (warningSE != null &&
                gm != null &&
                gm.isGameRunning)
            {
                MultiSEManager.Instance.PlaySE(warningSE);
            }

            StartWarningAnimation();
        }


        // ========================================
        // 爆発
        // ========================================

        if (timer <= 0f)
        {
            if (MultiSEManager.Instance != null)
            {
                MultiSEManager.Instance.StopSE(warningSE);
            }


            if (explodeSE != null &&
                gm != null &&
                gm.isGameRunning)
            {
                MultiSEManager.Instance.PlaySE(explodeSE);
            }

            Explode();
        }
    }


    // ========================================
    // 警告アニメーション
    // ========================================

    void StartWarningAnimation()
    {
        if (bombSprite == null)
            return;

        bombSprite.transform
            .DOScale(
                bombOriginalScale * warningScale,
                1f
            )
            .SetEase(Ease.Linear)
            .SetLink(gameObject);
    }


    // ========================================
    // 爆発
    // ========================================

    void Explode()
    {
        exploded = true;


        // ========================================
        // 爆弾本体を消す
        // ========================================

        if (bombSprite != null)
            bombSprite.enabled = false;


        // ========================================
        // カウント表示を消す
        // ========================================

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);


        // ========================================
        // 爆炎を即座に表示
        // ========================================

        ShowAllFire();


        // ========================================
        // カメラシェイク
        // ========================================

        if (Camera.main != null)
        {
            Transform cameraTransform =
                Camera.main.transform;

            // シェイク開始前のカメラ位置を保存
            Vector3 originalCameraPosition =
                cameraTransform.position;

            Quaternion originalCameraRotation =
                cameraTransform.rotation;


            // 念のため既存のカメラTweenを停止
            cameraTransform.DOKill();


            cameraTransform
                .DOShakePosition(
                    shakeDuration,
                    shakeStrength
                )
                .OnComplete(() =>
                {
                    // ====================================
                    // シェイク終了後に必ず元の位置へ戻す
                    // ====================================

                    cameraTransform.position =
                        originalCameraPosition;

                    cameraTransform.rotation =
                        originalCameraRotation;
                });
        }


        // ========================================
        // 爆炎終了処理
        // ========================================

        StartCoroutine(HideFire());
    }


    // ========================================
    // 爆炎をすべて表示
    // ========================================

    void ShowAllFire()
    {
        ShowFire(upFire, useUp);
        ShowFire(downFire, useDown);
        ShowFire(leftFire, useLeft);
        ShowFire(rightFire, useRight);
    }


    // ========================================
    // 爆炎表示
    // ========================================

    void ShowFire(
        GameObject fire,
        bool enabledDirection)
    {
        if (!enabledDirection || fire == null)
            return;

        fire.SetActive(true);

        Vector3 targetScale =
            fire.transform.localScale;


        // 最初は細い
        fire.transform.localScale =
            new Vector3(
                0.05f,
                targetScale.y,
                targetScale.z
            );


        // パッと伸びる
        fire.transform
            .DOScale(
                targetScale,
                0.08f
            )
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }


    // ========================================
    // 爆炎終了
    // ========================================

    IEnumerator HideFire()
    {
        // 爆炎を設定時間維持
        yield return new WaitForSeconds(
            fireDuration
        );


        // 爆炎を縮める
        HideFireAnimation(upFire);
        HideFireAnimation(downFire);
        HideFireAnimation(leftFire);
        HideFireAnimation(rightFire);


        // 縮小アニメーション待ち
        yield return new WaitForSeconds(0.08f);


        // 爆炎を非表示
        DisableAllFire();


        // ========================================
        // すべての演出終了後にBombを破棄
        // ========================================

        Destroy(gameObject);
    }


    // ========================================
    // 爆炎消滅アニメーション
    // ========================================

    void HideFireAnimation(GameObject fire)
    {
        if (fire == null ||
            !fire.activeSelf)
            return;


        Vector3 scale =
            fire.transform.localScale;


        fire.transform
            .DOScale(
                new Vector3(
                    0.05f,
                    scale.y,
                    scale.z
                ),
                0.08f
            )
            .SetEase(Ease.InQuad)
            .SetLink(gameObject);
    }


    // ========================================
    // 爆炎を非表示
    // ========================================

    void DisableAllFire()
    {
        if (upFire != null)
            upFire.SetActive(false);

        if (downFire != null)
            downFire.SetActive(false);

        if (leftFire != null)
            leftFire.SetActive(false);

        if (rightFire != null)
            rightFire.SetActive(false);
    }
}