using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class FallingPlatform : MonoBehaviour
{
    [Header("落下までの時間")]
    public float fallDelay = 1f;

    [Header("落下速度")]
    public float fallSpeed = 5f;

    [Header("復活するか")]
    public bool respawn = false;

    [Header("復活時間")]
    public float respawnTime = 3f;

    [Header("色")]
    public Color startColor = Color.white;
    public Color dangerColor = Color.red;

    [Header("当たり判定を許可するレイヤー")]
    public LayerMask collideLayers;

    [Header("一方通行設定")]
    public float oneWayThreshold = 0.1f; // 上から判定の余裕

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private Vector3 startPosition;

    private float timer = 0f;
    private bool isStepped = false;
    private bool isFalling = false;
    private bool respawnScheduled = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;

        startPosition = transform.position;
        sr.color = startColor;
    }

    void Update()
    {
        // =========================
        // ⏳ カウント
        // =========================
        if (isStepped && !isFalling)
        {
            timer += Time.deltaTime;

            if (fallDelay > 0f)
            {
                float t = Mathf.Clamp01(timer / fallDelay);
                sr.color = Color.Lerp(startColor, dangerColor, t);
            }

            if (timer >= fallDelay)
            {
                isFalling = true;
            }
        }

        // =========================
        // ⬇ 落下（物理じゃなく移動）
        // =========================
        if (isFalling)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            if (respawn && !respawnScheduled)
            {
                respawnScheduled = true;
                Invoke(nameof(Respawn), respawnTime);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // =========================
        // 🎯 レイヤー制限
        // =========================
        if ((collideLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // =========================
        // ⬆ 上から乗ったか判定
        // =========================
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 接触点が上からならOK
            if (contact.normal.y < -0.5f)
            {
                isStepped = true;

                if (fallDelay <= 0f)
                {
                    sr.color = dangerColor;
                    isFalling = true;
                }

                return;
            }
        }

        // 横や下からはすり抜け
        Physics2D.IgnoreCollision(col, collision.collider, true);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if ((collideLayers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        isStepped = false;
        timer = 0f;
        sr.color = startColor;

        // 念のため衝突戻す
        Physics2D.IgnoreCollision(col, collision.collider, false);
    }

    void Respawn()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    void OnEnable()
    {
        ResetPlatform();
    }

    void ResetPlatform()
    {
        transform.position = startPosition;

        timer = 0f;
        isStepped = false;
        isFalling = false;
        respawnScheduled = false;

        if (sr != null)
            sr.color = startColor;
    }
}