using UnityEngine;

public class BarrierItem : MonoBehaviour
{
    [SerializeField] private GameObject pickupEffectPrefab;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health == null) return;

        // すでに持ってたら無視
        if (health.HasBarrier()) return;

        // 付与
        health.AddBarrier();

        // 取得演出
        if (pickupEffectPrefab != null)
        {
            Instantiate(
                pickupEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }
        Destroy(gameObject);
    }
}