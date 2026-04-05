using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // PlayerHealthがあるかチェック
        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage();
        }
    }
}