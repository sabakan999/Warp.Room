using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ClearSequence : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Animator anim;

    public Camera mainCamera;
    public GameObject clearText; // CLEAR画像
    public GameObject returnText; // 「スペースで戻る」

    [Header("演出設定")]
    public float slowTimeScale = 0.2f;
    public float riseHeight = 20f;
    public float riseDuration = 1.5f;
    public float chargeTime = 0.5f;

    [Header("カメラ")]
    public float zoomInSize = 3f;
    public float zoomOutSize = 7f;
    public float zoomImpactSize = 2.2f;

    [Header("スプライト")]
    public Sprite risingSprite;
    public Sprite chargeSprite;
    public Sprite reachedSprite;

    [Header("パーティクル")]
    public ParticleSystem[] confettis;

    [Header("効果音")]
    public AudioSource audioSource;
    public AudioClip riseSE;
    public AudioClip chargeSE;
    public AudioClip impactSE;
    public AudioClip decideSE; // 🔥追加

    private bool canInput = false;

    public void Play(Transform targetPlayer)
    {
        player = targetPlayer;

        rb = player.GetComponent<Rigidbody2D>();
        col = player.GetComponent<Collider2D>();
        sr = player.GetComponent<SpriteRenderer>();
        anim = player.GetComponent<Animator>();

        if (returnText != null)
            returnText.SetActive(false);

        StartCoroutine(Sequence());
    }

    void Update()
    {
        if (!canInput) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(ReturnToStageSelect());
        }
    }

    IEnumerator Sequence()
    {
        Time.timeScale = slowTimeScale;

        // 🎥 カメラ寄り
        if (mainCamera != null)
        {
            mainCamera.transform
                .DOMove(player.position + new Vector3(0, 0, -10), 0.4f)
                .SetUpdate(true);

            mainCamera
                .DOOrthoSize(zoomInSize, 0.4f)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(0.4f);

        // 🔒 プレイヤー停止
        var pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        if (anim != null) anim.enabled = false;
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // 🚀 上昇
        if (sr != null && risingSprite != null)
            sr.sprite = risingSprite;

        PlaySE(riseSE);

        float targetY = player.position.y + riseHeight;

        player.DOMoveY(targetY, riseDuration).SetEase(Ease.OutCubic).SetUpdate(true);

        if (mainCamera != null)
        {
            mainCamera.transform
                .DOMoveY(targetY, riseDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(riseDuration);

        // 🧠 タメ
        if (sr != null && chargeSprite != null)
            sr.sprite = chargeSprite;

        PlaySE(chargeSE);

        if (mainCamera != null)
        {
            mainCamera
                .DOOrthoSize(zoomOutSize, chargeTime)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(chargeTime);

        // 💥 点火
        if (sr != null && reachedSprite != null)
            sr.sprite = reachedSprite;

        PlaySE(impactSE);

        foreach (var c in confettis)
        {
            if (c != null) c.Play();
        }

        player.localScale = Vector3.one * 0.3f;

        player.DOScale(1.2f, 0.2f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                player.DOScale(1f, 0.1f).SetUpdate(true);
            });

        if (mainCamera != null)
        {
            mainCamera
                .DOOrthoSize(zoomImpactSize, 0.2f)
                .SetEase(Ease.OutExpo)
                .SetUpdate(true);

            mainCamera.transform
                .DOShakePosition(0.25f, 0.4f)
                .SetUpdate(true);
        }

        // 🎉 CLEAR表示
        if (clearText != null)
        {
            clearText.SetActive(true);
            clearText.transform.localScale = Vector3.zero;

            clearText.transform
                .DOScale(2.0f, 0.25f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    clearText.transform.DOScale(1f, 0.1f).SetUpdate(true);
                });
        }

        // 🔥 少し待ってから入力解禁
        yield return new WaitForSecondsRealtime(0.7f);

        if (returnText != null)
        {
            returnText.SetActive(true);
            returnText.transform.localScale = Vector3.zero;

            returnText.transform
                .DOScale(1.2f, 0.2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    returnText.transform.DOScale(1f, 0.1f).SetUpdate(true);
                });
        }

        canInput = true;

        Time.timeScale = 1f;
    }

    IEnumerator ReturnToStageSelect()
    {
        canInput = false;

        PlaySE(decideSE);

        float wait = 0.2f;
        if (decideSE != null)
            wait = decideSE.length;

        yield return new WaitForSeconds(wait);

        SceneManager.LoadScene("ステージセレクト");
    }

    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}