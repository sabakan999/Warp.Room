using UnityEngine;
using System.Collections.Generic;

public class AttackSpawnEffect : MonoBehaviour
{
    [Header("出現演出Prefab")]
    public GameObject spawnEffectPrefab;


    [Header("対象タグ")]
    public List<string> spawnTags = new List<string>();


    void Start()
    {
        SpawnEffect();
    }


    void SpawnEffect()
    {
        if(spawnEffectPrefab == null)
            return;


        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if(spawnTags.Contains(child.tag))
            {
                Instantiate(
                    spawnEffectPrefab,
                    child.position,
                    Quaternion.identity
                );
            }
        }
    }
}