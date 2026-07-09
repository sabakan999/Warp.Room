using UnityEngine;

public class CurseItem : MonoBehaviour
{
    [Header("取得時SE（任意）")]
    public AudioClip getSE;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health == null)
            return;

        // 呪い付与
        health.AddCurse();

        // SE
        if (getSE != null)
        {
            SEManager.Instance.PlaySE(getSE);
        }

        // アイテム消滅
        Destroy(gameObject);
    }
}