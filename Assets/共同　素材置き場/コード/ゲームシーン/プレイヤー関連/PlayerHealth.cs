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

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 🔥 バリア状態を復元
        if (gameManager != null && gameManager.hasBarrier)
        {
            hasBarrier = true;
            CreateBarrierVisual();
        }
    }

    // =========================
    // 🛡 バリア取得
    // =========================
    public void AddBarrier()
    {
        if (hasBarrier) return;

        hasBarrier = true;

        if (gameManager != null)
            gameManager.hasBarrier = true; // 🔥 保存

        CreateBarrierVisual();

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

            if (gameManager != null)
                gameManager.hasBarrier = false; // 🔥 同期

            if (barrierInstance != null)
                Destroy(barrierInstance);

            Debug.Log("バリアで防いだ");

            StartCoroutine(InvincibleRoutine());
            return;
        }

        Die();
    }

    // =========================
    // 💀 死亡
    // =========================
    void Die()
    {
        isDead = true;

        Debug.Log("Player Dead");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

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
}