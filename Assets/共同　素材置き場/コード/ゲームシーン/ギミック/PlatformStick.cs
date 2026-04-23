using UnityEngine;
using System.Collections;

public class PlatformStick : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!gameObject.activeInHierarchy) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DetachSafe(collision.transform));
        }
    }

    IEnumerator DetachSafe(Transform target)
    {
        yield return null; // 1フレーム待つ（これ超重要）

        if (target != null)
        {
            target.SetParent(null);
        }
    }

    // 🔥 これが本命（親が消えるとき）
    private void OnDisable()
    {
        // 子を全部外す
        foreach (Transform child in transform)
        {
            child.SetParent(null);
        }
    }
}