using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BossAttack : MonoBehaviour
{
    [Header("この攻撃パターンの継続時間")]
    public float duration = 5f;


    [Header("出現演出")]
    public GameObject spawnEffectPrefab;

    [Header("出現位置タグ")]
    public List<string> spawnEffectTags = new List<string>();


    void Start()
    {
        PlaySpawnEffects();

        Invoke(nameof(EndAttack), duration);
    }


    void PlaySpawnEffects()
    {
        if(spawnEffectPrefab == null)
            return;


        Transform[] children =
            GetComponentsInChildren<Transform>();


        foreach(Transform child in children)
        {
            if(spawnEffectTags.Contains(child.tag))
            {
                Instantiate(
                    spawnEffectPrefab,
                    child.position,
                    Quaternion.identity
                );
            }
        }
    }


    void EndAttack()
    {
        KillAllTween();

        Destroy(gameObject);
    }


    void KillAllTween()
    {
        Transform[] allChildren =
            GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            child.DOKill();
        }
    }


    private void OnDestroy()
    {
        KillAllTween();
    }
}