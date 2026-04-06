using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyWalker : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 2f;
    private int direction = 1; // 1:右 -1:左

    [Header("壁検知")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
        CheckWall();
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void CheckWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * direction,
            wallCheckDistance,
            groundLayer
        );

        if (hit.collider != null)
        {
            Flip();
        }
    }

    void Flip()
    {
        direction *= -1;

        // 見た目反転（任意）
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // デバッグ用
    void OnDrawGizmosSelected()
    {
        if (wallCheck == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            wallCheck.position,
            wallCheck.position + Vector3.right * direction * wallCheckDistance
        );
    }
}