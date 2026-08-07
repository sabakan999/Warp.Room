using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    [Header("攻撃パターン")]
    public List<GameObject> attackPatterns = new List<GameObject>();

    [Header("攻撃生成位置")]
    public Transform attackSpawnPoint;

    [Header("ボス戦設定")]
    public float battleTime = 60f;


    [Header("見た目")]
    public BossVisualController bossVisual;


    [Header("クリア演出")]
    public BossClearSequence bossClearSequence;



    private bool isBattle = false;


    // 現在発生中の攻撃
    private BossAttack currentAttack;



    public void BeginBattle()
    {
        if (isBattle) return;


        Debug.Log("Boss Battle Start!");


        isBattle = true;


        StartCoroutine(BossBattleRoutine());
    }



    IEnumerator BossBattleRoutine()
    {
        float timer = 0f;


        while (timer < battleTime)
        {
            currentAttack = null;


            // ボス攻撃モーション開始
            if (bossVisual != null)
            {
                bossVisual.BeginAttack();
            }


            // Animation Eventで攻撃生成されるまで待機
            yield return new WaitUntil(
                () => currentAttack != null
            );


            // 攻撃終了まで待機
            yield return new WaitForSeconds(
                currentAttack.duration
            );


            timer += currentAttack.duration;


            currentAttack = null;
        }


        BossClear();
    }





    //================================================
    // Animation Eventから呼ばれる
    //================================================
    public void SpawnRandomAttack()
    {
        Debug.Log("SpawnRandomAttack");


        if (attackPatterns.Count == 0)
        {
            Debug.LogWarning(
                "攻撃パターンが登録されていません"
            );

            return;
        }



        GameObject pattern =
            attackPatterns[
                Random.Range(0, attackPatterns.Count)
            ];



        Vector3 spawnPos =
            attackSpawnPoint != null
            ? attackSpawnPoint.position
            : transform.position;



        GameObject obj = Instantiate(
            pattern,
            spawnPos,
            Quaternion.identity
        );



        currentAttack =
            obj.GetComponent<BossAttack>();


        if(currentAttack == null)
        {
            Debug.LogWarning(
                "生成された攻撃PrefabにBossAttackがありません"
            );
        }
    }





    void BossClear()
    {
        Debug.Log("Boss Survive Clear!");


        isBattle = false;



        if (bossVisual != null)
            bossVisual.MoveToTalkPosition();



        if (bossClearSequence != null)
        {
            bossClearSequence.StartClear();
        }
        else
        {
            Debug.LogWarning(
                "BossClearSequenceが設定されていません"
            );
        }
    }




    public void SetBossVisual(BossVisualController visual)
    {
        bossVisual = visual;
    }
}