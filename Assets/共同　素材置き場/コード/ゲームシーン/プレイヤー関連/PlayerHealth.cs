using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isDead = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void TakeDamage()
    {
        if (isDead) return;

        Die();
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player Dead");

        // 👉 プレイヤー停止（見た目用）
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().simulated = false;

        // 👉 ここで後で爆散演出入れる

        // GameManagerに通知
        gameManager.GameOver();
    }
}