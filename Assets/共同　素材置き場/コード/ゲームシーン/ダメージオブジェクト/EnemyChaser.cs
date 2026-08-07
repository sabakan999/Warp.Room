using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("追跡設定")]
    public float moveSpeed = 2f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;
        

        Vector3 dir = (player.position - transform.position).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

        FacePlayer();
    }

    void FacePlayer()
    {
        if (spriteRenderer == null) return;

        if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false; // 右向き
        else
            spriteRenderer.flipX = true;  // 左向き
    }
}