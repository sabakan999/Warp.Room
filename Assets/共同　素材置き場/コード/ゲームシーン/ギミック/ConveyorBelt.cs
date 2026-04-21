using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("対象タグ（複数）")]
    public string[] targetTags = { "Player" };

    [Header("ベルト設定")]
    public float moveSpeed = 3f;
    public Vector2 moveDirection = Vector2.right;

    [Header("方向設定")]
    public bool useLocalDirection = true;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsTargetTag(collision.gameObject.tag))
            return;

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        // =========================
        // ベルト方向
        // =========================
        Vector2 dir = moveDirection.normalized;

        if (useLocalDirection)
        {
            dir = transform.TransformDirection(moveDirection).normalized;
        }

        // =========================
        // 🔥 「押す」処理（重要）
        // =========================
        Vector2 velocity = rb.linearVelocity;

        velocity.x += dir.x * moveSpeed * Time.fixedDeltaTime;

        // Y方向は絶対に触らない（ジャンプ保護）
        rb.linearVelocity = velocity;
    }

    bool IsTargetTag(string objTag)
    {
        foreach (string tag in targetTags)
        {
            if (objTag == tag)
                return true;
        }
        return false;
    }
}