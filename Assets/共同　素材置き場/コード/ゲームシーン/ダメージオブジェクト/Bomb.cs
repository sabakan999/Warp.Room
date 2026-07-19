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

        // 残り1秒
        if (timer <= 1f && !warningStarted)
        {
            warningStarted = true;

            if (bombSprite != null && warningSprite != null)
                bombSprite.sprite = warningSprite;

            StartWarningAnimation();
        }

        if (timer <= 0f)
        {
            Explode();
        }
    }

    void StartWarningAnimation()
    {
        if (bombSprite == null)
            return;

        bombSprite.transform
            .DOScale(bombOriginalScale * warningScale, 1f)
            .SetEase(Ease.Linear)
            .SetLink(gameObject);
    }

    void Explode()
    {
        exploded = true;

        // 爆弾消す
        if (bombSprite != null)
            bombSprite.enabled = false;

        // カウント消す
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

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

        ShowFire(upFire, useUp);
        ShowFire(downFire, useDown);
        ShowFire(leftFire, useLeft);
        ShowFire(rightFire, useRight);

        StartCoroutine(HideFire());
    }

    void ShowFire(GameObject fire, bool enabledDirection)
    {
        if (!enabledDirection || fire == null)
            return;

        fire.SetActive(true);

        Vector3 targetScale = fire.transform.localScale;

        // 最初は細い
        fire.transform.localScale =
            new Vector3(
                0.05f,
                targetScale.y,
                targetScale.z
            );

        // パッと伸びる
        fire.transform
            .DOScale(targetScale, 0.08f)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }

    IEnumerator HideFire()
    {
        yield return new WaitForSeconds(fireDuration);

        HideFireAnimation(upFire);
        HideFireAnimation(downFire);
        HideFireAnimation(leftFire);
        HideFireAnimation(rightFire);

        yield return new WaitForSeconds(0.08f);

        DisableAllFire();

        Destroy(gameObject);
    }

    void HideFireAnimation(GameObject fire)
    {
        if (fire == null || !fire.activeSelf)
            return;

        Vector3 scale = fire.transform.localScale;

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

    void DisableAllFire()
    {
        if (upFire != null) upFire.SetActive(false);
        if (downFire != null) downFire.SetActive(false);
        if (leftFire != null) leftFire.SetActive(false);
        if (rightFire != null) rightFire.SetActive(false);
    }
}