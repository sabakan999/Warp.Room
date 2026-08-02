using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("生成候補")]
    public List<GameObject> prefabs = new List<GameObject>();

    public Transform spawnPoint;

    [Header("生成設定")]
    public float interval = 2f;

    [Header("ランダム間隔設定")]
    public bool randomInterval = false;
    public float minInterval = 1f;
    public float maxInterval = 3f;

    [Header("寿命")]
    public float lifeTime = 10f;

    [Header("ランダム力")]
    public float forcePower = 2f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            Spawn();

            float waitTime = interval;

            if (randomInterval)
            {
                waitTime = Random.Range(minInterval, maxInterval);
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    void Spawn()
    {
        if (prefabs == null || prefabs.Count == 0) return;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;

        GameObject selected = prefabs[Random.Range(0, prefabs.Count)];
        GameObject obj = Instantiate(selected, pos, Quaternion.identity);

        AddRandomForce(obj);

        Destroy(obj, lifeTime);
    }

    void AddRandomForce(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 force = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(0.3f, 1f)
        ).normalized * forcePower;

        rb.AddForce(force, ForceMode2D.Impulse);
    }
}