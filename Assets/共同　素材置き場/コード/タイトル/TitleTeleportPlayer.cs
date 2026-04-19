using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleTeleportPlayer : MonoBehaviour
{
    [Header("プレイヤー")]
    public GameObject playerPrefab;

    [Header("部屋（スポーンポイント）")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("テレポート設定")]
    public float teleportInterval = 3f;
    public float spawnForce = 5f;

    private GameObject playerInstance;

    private bool isStarted = false; // 🔥 開始フラグ

    void Start()
    {
        // 何もしない（待機）
    }

    // 🔥 外部から呼ぶ
    public void StartTeleport()
    {
        if (isStarted) return;

        isStarted = true;

        SpawnPlayerFirst();
        StartCoroutine(TeleportLoop());
    }

    void SpawnPlayerFirst()
    {
        if (spawnPoints.Count == 0 || playerPrefab == null)
            return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

        playerInstance = Instantiate(playerPrefab, point.position, Quaternion.identity);

        AddRandomForce(playerInstance);
    }

    IEnumerator TeleportLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);

            TeleportPlayer();
        }
    }

    void TeleportPlayer()
    {
        if (playerInstance == null || spawnPoints.Count == 0)
            return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

        Rigidbody2D rb = playerInstance.GetComponent<Rigidbody2D>();

        // 速度リセット
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // テレポート
        playerInstance.transform.position = point.position;

        // ポーン！
        AddRandomForce(playerInstance);
    }

    void AddRandomForce(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 force = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1.5f)
        ).normalized * spawnForce;

        rb.AddForce(force, ForceMode2D.Impulse);
    }
}