using UnityEngine;

public class Spring : MonoBehaviour
{
    public float jumpForce = 15f;

    public Sprite normalSprite;
    public Sprite pressedSprite;

    private SpriteRenderer sr;

    private bool isPressed = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = normalSprite;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 🔽 上から踏んだ
            if (contact.normal.y < -0.5f)
            {
                isPressed = true;
                sr.sprite = pressedSprite;

                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // 🔥 気持ちいいジャンプにする
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
            // 🔽 乗ってる間は潰れたまま
            sr.sprite = pressedSprite;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // 🔽 離れたら戻る
        isPressed = false;
        sr.sprite = normalSprite;
    }
}