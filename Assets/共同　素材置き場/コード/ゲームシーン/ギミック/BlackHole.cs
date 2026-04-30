using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("吸引範囲")]
    public float radius = 5f;

    [Header("吸引力")]
    public float force = 20f;

    [Header("最大吸引力（近距離）")]
    public float maxForce = 100f;

    [Header("対象タグ")]
    public string targetTag = "Player";

    [Header("減衰カーブ（距離依存）")]
    public AnimationCurve forceCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    void FixedUpdate()
    {
        // 🔥 2D用に変更
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D col in hits)
        {
            if (!col.CompareTag(targetTag)) continue;

            Rigidbody2D rb = col.attachedRigidbody;
            if (rb == null) continue;

            Vector2 direction = (Vector2)transform.position - rb.position;
            float distance = direction.magnitude;

            if (distance < 0.1f) continue;

            // 距離を0〜1に正規化（近いほど1）
            float t = 1f - Mathf.Clamp01(distance / radius);

            // 吸引力計算
            float currentForce = Mathf.Lerp(force, maxForce, forceCurve.Evaluate(t));

            rb.AddForce(direction.normalized * currentForce, ForceMode2D.Force);
        }
    }

    // 🔥 2D用ギズモ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}