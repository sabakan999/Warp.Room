using UnityEngine;

public class Spring : MonoBehaviour
{
    public float jumpForce = 15f;

    public Sprite normalSprite;
    public Sprite pressedSprite;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip jumpSE;

    private SpriteRenderer sr;

    private bool isPressed = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = normalSprite;

        // AudioSourceが設定されていなければ自動取得
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 上から踏んだ
            if (contact.normal.y < -0.5f)
            {
                isPressed = true;
                sr.sprite = pressedSprite;
                if (audioSource != null && jumpSE != null)
                        audioSource.PlayOneShot(jumpSE);


                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    
                    
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }

                break;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (isPressed)
        {
            sr.sprite = pressedSprite;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        isPressed = false;
        sr.sprite = normalSprite;
    }
}