using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

    // ========================================
    // カメラ初期状態
    // ========================================

    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;


    // ========================================
    // ボス戦開始
    // ========================================

    public void BeginBattle()
    {
        if (isBattle)
            return;

        Debug.Log("Boss Battle Start!");

        isBattle = true;

        // ボス戦開始時のカメラ位置を保存
        if (Camera.main != null)
        {
            initialCameraPosition =
                Camera.main.transform.position;

            initialCameraRotation =
                Camera.main.transform.rotation;
        }

        StartCoroutine(BossBattleRoutine());
    }


    // ========================================
    // ボス戦ループ
    // ========================================

    IEnumerator BossBattleRoutine()
    {
        float timer = 0f;

        while (timer < battleTime)
        {
            currentAttack = null;

            // ========================================
            // 攻撃モーション開始
            // ========================================

            if (bossVisual != null)
            {
                // 前回のカメラシェイクを解除
                RestoreCamera();

                // ボスの攻撃モーション開始
                //
                // BeginAttack() 内で
                // PrepareAttack() が呼ばれる
                //
                bossVisual.BeginAttack();
            }

            // ========================================
            // 攻撃Prefab生成待ち
            // ========================================

            yield return new WaitUntil(
                () => currentAttack != null
            );

            Debug.Log(
                "BossAttack spawned. Waiting for activation."
            );

            // ========================================
            // ⚠ 警告中
            //
            // ここではまだ攻撃しない
            //
            // 指パッチン時に
            // BossController.ActivateAttack()
            // が呼ばれる
            // ========================================

            yield return new WaitUntil(
                () =>
                    currentAttack == null ||
                    currentAttack.IsActivated
            );

            // ========================================
            // 攻撃が消えていた場合
            // ========================================

            if (currentAttack == null)
            {
                RestoreCamera();
                continue;
            }

            // ========================================
            // 実際の攻撃が終了するまで待機
            // ========================================

            float attackDuration =
                currentAttack.duration;

            yield return new WaitForSeconds(
                attackDuration
            );

            // ========================================
            // 攻撃終了
            // ========================================

            RestoreCamera();

            timer += attackDuration;

            currentAttack = null;
        }

        // ========================================
        // ボス戦終了
        // ========================================

        RestoreCamera();

        BossClear();
    }


    // ========================================
    // カメラを初期位置へ戻す
    // ========================================

    void RestoreCamera()
    {
        if (Camera.main == null)
            return;

        // 実行中のTween・カメラシェイクを停止
        Camera.main.transform.DOKill();

        // 保存した初期位置へ戻す
        Camera.main.transform.position =
            initialCameraPosition;

        Camera.main.transform.rotation =
            initialCameraRotation;
    }


    // ========================================
    // 攻撃準備
    //
    // BossVisualController
    // BeginAttack() から呼ばれる
    //
    // 攻撃Prefabを生成するだけ。
    // この時点では攻撃は発動しない。
    // ========================================

    public void PrepareAttack()
    {
        Debug.Log("PrepareAttack");

        if (attackPatterns.Count == 0)
        {
            Debug.LogWarning(
                "攻撃パターンが登録されていません"
            );

            return;
        }

        // ランダムな攻撃を選択
        GameObject pattern =
            attackPatterns[
                Random.Range(
                    0,
                    attackPatterns.Count
                )
            ];

        // 生成位置
        Vector3 spawnPos =
            attackSpawnPoint != null
                ? attackSpawnPoint.position
                : transform.position;

        // 攻撃Prefab生成
        GameObject obj =
            Instantiate(
                pattern,
                spawnPos,
                Quaternion.identity
            );

        // BossAttack取得
        currentAttack =
            obj.GetComponent<BossAttack>();

        if (currentAttack == null)
        {
            Debug.LogWarning(
                "生成された攻撃PrefabにBossAttackがありません"
            );

            Destroy(obj);
            return;
        }

        Debug.Log(
            "攻撃Prefab生成完了。警告状態で待機します。"
        );
    }


    // ========================================
    // 攻撃発動
    //
    // BossVisualController
    // 指パッチンAnimation Eventから呼ばれる
    //
    // ⚠ → 実際の攻撃
    // ========================================

    public void ActivateAttack()
    {
        if (currentAttack == null)
        {
            Debug.LogWarning(
                "ActivateAttack: 現在の攻撃がありません"
            );

            return;
        }

        Debug.Log(
            "ActivateAttack: 攻撃を発動します"
        );

        currentAttack.ActivateAttack();
    }


    // ========================================
    // ボスクリア
    // ========================================

    void BossClear()
    {
        Debug.Log(
            "Boss Survive Clear!"
        );

        isBattle = false;

        // ボス戦終了時にもカメラを確実に戻す
        RestoreCamera();

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


    // ========================================
    // 外部からBossVisualを設定
    // ========================================

    public void SetBossVisual(
        BossVisualController visual
    )
    {
        bossVisual = visual;
    }
}