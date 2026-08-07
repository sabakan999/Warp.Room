using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // 🔥 追加

public class ClearSequence : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Animator anim;

    public Camera mainCamera;
    public GameObject clearText;
    public GameObject returnText;

    [Header("演出開始時に消すUI")]
    public List<GameObject> hideUIObjects = new List<GameObject>(); // 🔥 追加

    [Header("演出設定")]
    public float slowTimeScale = 0.2f;
    public float riseHeight = 20f;
    public float riseDuration = 1.5f;
    public float chargeTime = 0.5f;

    [Header("カメラ")]
    public float zoomInSize = 3f;
    public float zoomOutSize = 7f;
    public float zoomImpactSize = 2.2f;
    private Vector3 savedCameraPosition;
    private float savedCameraSize;

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
    public AudioClip decideSE;

    public BGMManager bgmManager;

    public System.Action OnBossWarp;

    

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

        // 🔥 ここでUI消す
        HideUIObjects();

        StartCoroutine(Sequence());
    }

    // 🔥 追加：UIを消す処理
    void HideUIObjects()
    {
        foreach (var obj in hideUIObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    void Update()
    {
        if (!canInput) return;

       // Aボタン（提出）
    // Aボタン（提出）
    if (Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.Return) ||
        Input.GetButtonDown("Submit"))
    {
        StartCoroutine(ReturnToStageSelect());
    }

    // Bボタン（キャンセル）
    if (Input.GetKeyDown(KeyCode.Backspace) ||
        Input.GetButtonDown("Cancel"))
    {
        StartCoroutine(ReturnToStageSelect());
    }

    // ＋ボタン（PauseButton）
    if (Input.GetButtonDown("PauseButton"))
    {
        StartCoroutine(ReturnToStageSelect());
    }
    }

    IEnumerator Sequence()
    {
        Time.timeScale = slowTimeScale;
        savedCameraPosition = mainCamera.transform.position;
        savedCameraSize = mainCamera.orthographicSize;

        if (mainCamera != null)
        {
            mainCamera.transform
                .DOMove(player.position + new Vector3(0, 0, -10), 0.4f)
                .SetUpdate(true)
                .SetLink(mainCamera.gameObject);
                

            mainCamera
                .DOOrthoSize(zoomInSize, 0.4f)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .SetLink(mainCamera.gameObject);
        }

        yield return new WaitForSecondsRealtime(0.4f);

        var pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        if (anim != null) anim.enabled = false;
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

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
                .SetUpdate(true)
                .SetLink(mainCamera.gameObject);
        }

        yield return new WaitForSecondsRealtime(riseDuration);

        if (sr != null && chargeSprite != null)
            sr.sprite = chargeSprite;

        PlaySE(chargeSE);

        if (mainCamera != null)
        {
            mainCamera
                .DOOrthoSize(zoomOutSize, chargeTime)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(true)
                .SetLink(mainCamera.gameObject);
        }

        yield return new WaitForSecondsRealtime(chargeTime);

        if (sr != null && reachedSprite != null)
            sr.sprite = reachedSprite;

        PlaySE(impactSE);

        foreach (var c in confettis)
        {
            if (c != null) c.Play();
        }
        
        if (bgmManager != null)
            bgmManager.PlayClearBGM();

        player.localScale = Vector3.one * 0.3f;

        player.DOScale(1.2f, 0.2f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                player.DOScale(1f, 0.1f).SetUpdate(true);
            })
            .SetLink(player.gameObject);

        if (mainCamera != null)
        {
            mainCamera
                .DOOrthoSize(zoomImpactSize, 0.2f)
                .SetEase(Ease.OutExpo)
                .SetUpdate(true)
                .SetLink(mainCamera.gameObject);

            mainCamera.transform
                .DOShakePosition(0.25f, 0.4f)
                .SetUpdate(true)
                .SetLink(mainCamera.gameObject);
        }

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
                })
                .SetLink(clearText);;
        }

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

        float wait = (decideSE != null) ? decideSE.length : 0.2f;
        yield return new WaitForSeconds(wait);

        SceneManager.LoadScene("ステージセレクト");
    }

    void PlaySE(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }

  public void PlayFinalBossIntro(Transform targetPlayer)
{
    player = targetPlayer;

    rb = player.GetComponent<Rigidbody2D>();
    col = player.GetComponent<Collider2D>();
    sr = player.GetComponent<SpriteRenderer>();
    anim = player.GetComponent<Animator>();

    HideUIObjects();

    StartCoroutine(FinalBossIntroSequence());
}
IEnumerator FinalBossIntroSequence()
{
    Time.timeScale = slowTimeScale;
    savedCameraPosition = mainCamera.transform.position;
savedCameraSize = mainCamera.orthographicSize;

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

    var pc = player.GetComponent<PlayerController>();
    if (pc != null) pc.enabled = false;

    if (anim != null) anim.enabled = false;
    if (col != null) col.enabled = false;

    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    if (sr != null && risingSprite != null)
        sr.sprite = risingSprite;

    PlaySE(riseSE);

    float targetY = player.position.y + riseHeight;

    player.DOMoveY(targetY, riseDuration)
          .SetEase(Ease.OutCubic)
          .SetUpdate(true);

    if (mainCamera != null)
    {
        mainCamera.transform
            .DOMoveY(targetY, riseDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    // 上昇途中
    yield return new WaitForSecondsRealtime(0.8f);

    // ★Tween停止
    player.DOKill();

    if (mainCamera != null)
    {
        mainCamera.transform.DOKill();
        mainCamera.DOKill();
    }

    Time.timeScale = 1f;

    // ★GameManagerへ処理を渡す
    OnBossWarp?.Invoke();
}

public void RestoreCamera()
{
    if (mainCamera == null) return;

    mainCamera.transform.position = savedCameraPosition;
    mainCamera.orthographicSize = savedCameraSize;
}

public void ShowUIObjects()
{
    foreach (var obj in hideUIObjects)
    {
        if (obj != null)
            obj.SetActive(true);
    }
}
}