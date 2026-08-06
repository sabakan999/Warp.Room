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

    [Header("攻撃間隔")]
    public float attackInterval = 1f;

    [Header("攻撃演出")]
    public float attackMotionTime = 0.5f;

    [Header("見た目")]
    public BossVisualController bossVisual;

    [Header("クリア演出")]
    public BossClearSequence bossClearSequence;

    private bool isBattle = false;

    /// <summary>
    /// ボス戦開始
    /// </summary>
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
            yield return StartCoroutine(AttackRoutine());

            yield return new WaitForSeconds(attackInterval);

            timer += attackInterval;
        }

        BossClear();
    }

    IEnumerator AttackRoutine()
    {
        // 攻撃モーション開始
        if (bossVisual != null)
            bossVisual.BeginAttack();

         Debug.Log("AttackRoutine");

        // 指パッチン待ち
        yield return new WaitForSeconds(attackMotionTime);

        SpawnRandomAttack();

        // 漂い再開
        if (bossVisual != null)
            bossVisual.EndAttack();
    }

    void SpawnRandomAttack()
    {
        if (attackPatterns.Count == 0)
        {
            Debug.LogWarning("攻撃パターンが登録されていません");
            return;
        }

        GameObject pattern =
            attackPatterns[
                Random.Range(0, attackPatterns.Count)
            ];

        Vector3 spawnPos = transform.position;

        if (attackSpawnPoint != null)
        {
            spawnPos = attackSpawnPoint.position;
        }

        Instantiate(
            pattern,
            spawnPos,
            Quaternion.identity
        );
    }

    void BossClear()
    {
        Debug.Log("Boss Survive Clear!");

        isBattle = false;

        // 会話位置へ戻る
        if (bossVisual != null)
            bossVisual.MoveToTalkPosition();

        if (bossClearSequence != null)
        {
            bossClearSequence.StartClear();
        }
        else
        {
            Debug.LogWarning("BossClearSequenceが設定されていません");
        }
    }
    
    public void SetBossVisual(BossVisualController visual)
{
    bossVisual = visual;
}
}