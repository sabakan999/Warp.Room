using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class FallingPlatform : MonoBehaviour
{
    [Header("落下までの時間")]
    public float fallDelay = 1f;

    [Header("復活するか")]
    public bool respawn = false;

    [Header("復活時間")]
    public float respawnTime = 3f;

    [Header("初期色")]
    public Color startColor = Color.white;

    [Header("危険色")]
    public Color dangerColor = Color.red;

    [Header("レイヤー設定")]
    public string playerLayerName = "Player";
    public string platformLayerName = "Platform";

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private int playerLayer;
    private int platformLayer;

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

        // レイヤー取得
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        platformLayer = LayerMask.NameToLayer(platformLayerName);
    }

    void Update()
    {
        if (!isStepped || isFalling) return;

        timer += Time.deltaTime;

        // 色変化
        if (fallDelay > 0f)
        {
            float t = Mathf.Clamp01(timer / fallDelay);
            sr.color = Color.Lerp(startColor, dangerColor, t);
        }

        if (timer >= fallDelay)
        {
            Fall();
        }
    }

    void Fall()
    {
        if (isFalling) return;

        isFalling = true;
        rb.bodyType = RigidbodyType2D.Dynamic;

        // 👇 Player以外との衝突を無効化
        for (int i = 0; i < 32; i++)
        {
            if (i != playerLayer)
            {
                Physics2D.IgnoreLayerCollision(platformLayer, i, true);
            }
        }

        // 復活予約
        if (respawn && !respawnScheduled)
        {
            respawnScheduled = true;
            Invoke(nameof(Respawn), respawnTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isStepped = true;

            if (fallDelay <= 0f)
            {
                sr.color = dangerColor;
                Fall();
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isStepped = false;
            timer = 0f;
            sr.color = startColor;
        }
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
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.position = startPosition;

        timer = 0f;
        isStepped = false;
        isFalling = false;
        respawnScheduled = false;

        // 👇 衝突を元に戻す
        for (int i = 0; i < 32; i++)
        {
            Physics2D.IgnoreLayerCollision(platformLayer, i, false);
        }

        if (sr != null)
            sr.color = startColor;
    }
}