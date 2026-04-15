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

        // ベルト方向
        Vector2 dir = moveDirection.normalized;

        if (useLocalDirection)
        {
            dir = transform.TransformDirection(moveDirection).normalized;
        }

        // 🔥 プレイヤーコードが毎FixedUpdateで速度上書きしてるので、
        // ベルト側は「位置を直接動かす」方式にする
        Vector2 moveAmount = dir * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + moveAmount);
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