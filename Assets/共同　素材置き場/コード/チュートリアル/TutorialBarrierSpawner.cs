using UnityEngine;

public class TutorialBarrierSpawner : MonoBehaviour
{
    [Header("生成")]
    public GameObject barrierPrefab;

    [Header("出現位置")]
    public Transform spawnPoint;

    [Header("出現演出")]
    public GameObject spawnEffect;

    private GameObject currentBarrier;

    //==========================
    // バリア出現
    //==========================
    public void SpawnBarrier()
    {
        if (spawnEffect != null)
        {
            Instantiate(
                spawnEffect,
                spawnPoint.position,
                Quaternion.identity
            );
        }

        currentBarrier = Instantiate(
            barrierPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }

    //==========================
    // バリア削除
    //==========================
    public void DestroyBarrier()
    {
        if (currentBarrier != null)
        {
            Destroy(currentBarrier);
            currentBarrier = null;
        }
    }
}