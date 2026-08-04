using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private bool isDead = false;

    private GameManager gameManager;

    [Header("バリア")]
    public GameObject barrierPrefab;
    private GameObject barrierInstance;
    private bool hasBarrier = false;

    [Header("無敵")]
    public float invincibleTime = 0.5f;
    private bool isInvincible = false;

    [Header("点滅")]
    public float blinkInterval = 0.1f;
    private SpriteRenderer spriteRenderer;

    // 🔊 SE
    [Header("SE")]
    public AudioClip damageSE;
    public AudioClip barrierBreakSE;
    public AudioClip barrierGetSE;

    [Header("呪い")]
    public bool hasCurse = false;
    public ParticleSystem curseEffect;

   

   
    private CurseUI curseUI;
void Start()
{
    gameManager = FindFirstObjectByType<GameManager>();
    spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    curseUI = FindFirstObjectByType<CurseUI>();

    if (curseUI != null)
        curseUI.Hide();

    if (gameManager != null && gameManager.hasBarrier)
    {
        hasBarrier = true;
        CreateBarrierVisual();
    }

    if (curseEffect != null)
        curseEffect.Stop();
}

    // =========================
    // 🛡 バリア取得
    // =========================
    public void AddBarrier()
    {
        if (hasBarrier) return;

        hasBarrier = true;

        if (gameManager != null)
            gameManager.hasBarrier = true;

        CreateBarrierVisual();

        SEManager.Instance?.PlaySE(barrierGetSE);

        Debug.Log("バリア獲得");
    }

    public bool HasBarrier()
    {
        return hasBarrier;
    }

    void CreateBarrierVisual()
    {
        if (barrierPrefab != null)
        {
            barrierInstance = Instantiate(barrierPrefab, transform);
            barrierInstance.transform.localPosition = Vector3.zero;
        }
    }

    // =========================
    // 💥 ダメージ
    // =========================
    public void TakeDamage()
    {
        if (isDead || isInvincible) return;

        if (hasBarrier)
        {
            hasBarrier = false;
            TutorialManager tutorial = FindFirstObjectByType<TutorialManager>();

            if (tutorial != null)
            {
                tutorial.ReportBarrierUsed();
            }

            if (gameManager != null)
                gameManager.hasBarrier = false;

            if (barrierInstance != null)
                Destroy(barrierInstance);

            SEManager.Instance?.PlaySE(barrierBreakSE);

            Debug.Log("バリアで防いだ");

            StartCoroutine(InvincibleRoutine());
            return;
        }

        SEManager.Instance?.PlaySE(damageSE);

        Die();
    }

    // =========================
    // 💀 死亡
    // =========================
  void Die()
{
    isDead = true;

    Debug.Log("Player Dead");

    if (curseUI != null)
        curseUI.gameObject.SetActive(false);

    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    rb.linearVelocity = Vector2.zero;
    rb.simulated = false;

    TutorialManager tutorial = FindFirstObjectByType<TutorialManager>();

    if (tutorial != null)
    {
        tutorial.ReportPlayerDead(gameObject);
        return;
    }

    gameManager.GameOver();
}

    // =========================
    // ⏱ 無敵＋点滅
    // =========================
    IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        float timer = 0f;

        while (timer < invincibleTime)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }

    // =========================
    // 👻 呪い取得
    // =========================
   public void AddCurse()
{
    if (hasCurse) return;

    hasCurse = true;

    if (curseEffect != null)
        curseEffect.Play();

    if (curseUI != null)
        curseUI.Show();
}

    // =========================
    // ✨ 呪い解除
    // =========================
    public void RemoveCurse()
{
    hasCurse = false;

    if (curseEffect != null)
    {
        curseEffect.Stop();
        curseEffect.Clear();
    }

    if (curseUI != null)
         curseUI.Hide();
}

    // =========================
    // 👻 呪い所持確認
    // =========================
    public bool HasCurse()
    {
        return hasCurse;
    }

    public bool IsDead()
    {
        return isDead;
    }
}