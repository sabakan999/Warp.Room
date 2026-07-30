using System.Collections;
using UnityEngine;

public class TutorialEnemySpawner : MonoBehaviour
{
    [Header("生成")]
    public GameObject enemyPrefab;

    [Header("出現位置")]
    public Transform spawnPoint;

    [Header("出現演出")]
    public GameObject spawnEffect;

    [Header("演出時間")]
    public float spawnDelay = 1.0f;

    private GameObject currentEnemy;

    //==========================
    // 敵出現
    //==========================
    

  public void SpawnEnemy()
{
    if (spawnEffect != null)
    {
        Instantiate(
            spawnEffect,
            spawnPoint.position,
            Quaternion.identity
        );
    }

    currentEnemy = Instantiate(
        enemyPrefab,
        spawnPoint.position,
        Quaternion.identity
    );
}

    //==========================
    // 敵削除
    //==========================
    public void DestroyEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }
    }
}