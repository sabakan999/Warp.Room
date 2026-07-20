using UnityEngine;
using System.Collections;

public class MeteorSpawner : MonoBehaviour
{
    [Header("生成")]
    public GameObject meteorPrefab;

    public float spawnInterval = 1.5f;

    [Header("生成範囲")]
    public Vector2 areaSize = new Vector2(20, 10);

    

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnMeteor();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMeteor()
    {
        Vector3 pos = transform.position;

        pos.x += Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        pos.y += Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);

        GameObject obj =
    Instantiate(
        meteorPrefab,
        pos,
        Quaternion.identity,
        transform.parent
    );

        // 毎回プレイヤーを探す
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Meteor meteor = obj.GetComponent<Meteor>();
            meteor.target = player.transform;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(areaSize.x, areaSize.y, 0));
    }
#endif
}