using UnityEngine;

public class TutorialCurseSpawner : MonoBehaviour
{
    [Header("生成")]
    public GameObject cursePrefab;
    public GameObject angelPrefab;

    [Header("出現位置")]
    public Transform curseSpawnPoint;
    public Transform angelSpawnPoint;

    [Header("出現演出")]
    public GameObject spawnEffect;

    private GameObject currentCurse;
    private GameObject currentAngel;

    //==========================
    // 呪い出現
    //==========================
    public void SpawnCurse()
    {
        if (spawnEffect != null)
        {
            Instantiate(
                spawnEffect,
                curseSpawnPoint.position,
                Quaternion.identity
            );
        }

        currentCurse = Instantiate(
            cursePrefab,
            curseSpawnPoint.position,
            Quaternion.identity
        );
    }

    //==========================
    // 天使出現
    //==========================
    public void SpawnAngel()
    {
        if (spawnEffect != null)
        {
            Instantiate(
                spawnEffect,
                angelSpawnPoint.position,
                Quaternion.identity
            );
        }

        currentAngel = Instantiate(
            angelPrefab,
            angelSpawnPoint.position,
            Quaternion.identity
        );
    }

    //==========================
    // 全削除
    //==========================
    public void DestroyObjects()
    {
        if (currentCurse != null)
        {
            Destroy(currentCurse);
            currentCurse = null;
        }

        if (currentAngel != null)
        {
            Destroy(currentAngel);
            currentAngel = null;
        }
    }
}