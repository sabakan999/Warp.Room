using UnityEngine;

public class CurseRemover : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health == null)
            return;

        // 呪いを持っていなければ何もしない
        if (!health.hasCurse)
            return;

        // 呪い解除
        health.RemoveCurse();

        Debug.Log("呪い解除");

        // 一度きりなら消す
        Destroy(gameObject);
    }
}