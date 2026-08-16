using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BossAttack : MonoBehaviour
{
    [Header("この攻撃パターンの継続時間")]
    public float duration = 5f;

    [Header("予告時間")]
    public float warningDuration = 1f;

    [Header("出現演出")]
    public GameObject spawnEffectPrefab;

    [Header("出現位置タグ")]
    public List<string> spawnEffectTags = new List<string>();

    [Header("警告マーク")]
    public GameObject warningMarkPrefab;

    [Header("攻撃本体")]
    public GameObject attackBody;

    // 実際に攻撃が開始されたか
    public bool IsActivated = false;

    // 生成した警告マーク
    private List<GameObject> warningMarks =
        new List<GameObject>();


    void Start()
    {
        // ========================================
        // 最初は攻撃本体を停止
        // ========================================

        SetAttackActive(false);

        // ========================================
        // ⚠ 警告マーク表示
        // ========================================

        CreateWarningMarks();

        // ========================================
        // 予告後に攻撃開始
        // ========================================

        StartCoroutine(WarningRoutine());
    }


    IEnumerator WarningRoutine()
    {
        yield return new WaitForSeconds(warningDuration);

        ActivateAttack();
    }


    //================================================
    // ⚠ 警告マーク生成
    //================================================

  void CreateWarningMarks()
{
    if (warningMarkPrefab == null)
    {
        Debug.LogWarning(
            $"[{gameObject.name}] warningMarkPrefab が設定されていません"
        );

        return;
    }

    Transform[] children =
        GetComponentsInChildren<Transform>(true);

    Debug.Log(
        $"[{gameObject.name}] 警告マーク生成開始。対象Transform数 = {children.Length}"
    );

    foreach (Transform child in children)
    {
        Debug.Log(
            $"[{gameObject.name}] 子Transform: {child.name} / Tag: {child.tag} / Active: {child.gameObject.activeInHierarchy}"
        );

        if (spawnEffectTags.Contains(child.tag))
        {
            Debug.Log(
                $"[{gameObject.name}] ★警告対象発見: {child.name} / Tag: {child.tag}"
            );

            GameObject warning =
                Instantiate(
                    warningMarkPrefab,
                    child.position,
                    Quaternion.identity
                );

            warningMarks.Add(warning);
        }
    }

    Debug.Log(
        $"[{gameObject.name}] 警告マーク生成完了。生成数 = {warningMarks.Count}"
    );
}


    //================================================
    // 🔥 攻撃開始
    //================================================

    public void ActivateAttack()
    {
        // すでに攻撃開始済みなら何もしない
        if (IsActivated)
            return;

        // 攻撃開始済みにする
        IsActivated = true;

        // ========================================
        // ⚠ 警告マーク削除
        // ========================================

        foreach (GameObject warning in warningMarks)
        {
            if (warning != null)
                Destroy(warning);
        }

        warningMarks.Clear();

        // ========================================
        // 🔥 攻撃本体ON
        // ========================================

        SetAttackActive(true);

        // ========================================
        // ✨ 出現エフェクト
        // ========================================

        PlaySpawnEffects();

        // ========================================
        // 攻撃終了
        // ========================================

        Invoke(nameof(EndAttack), duration);
    }


    //================================================
    // 攻撃本体 ON / OFF
    //================================================

    void SetAttackActive(bool active)
    {
        // 攻撃本体のGameObject
        if (attackBody != null)
        {
            attackBody.SetActive(active);
        }

        // 攻撃Prefab自身以下のColliderを制御
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D col in colliders)
        {
            col.enabled = active;
        }
    }


    //================================================
    // ✨ 出現エフェクト
    //================================================

    void PlaySpawnEffects()
    {
        if (spawnEffectPrefab == null)
            return;

        Transform[] children =
            GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            // 警告マークと同じタグを使用
            if (spawnEffectTags.Contains(child.tag))
            {
                Instantiate(
                    spawnEffectPrefab,
                    child.position,
                    Quaternion.identity
                );
            }
        }
    }


    //================================================
    // 攻撃終了
    //================================================

    void EndAttack()
    {
        KillAllTween();

        Destroy(gameObject);
    }


    //================================================
    // Tween停止
    //================================================

    void KillAllTween()
    {
        Transform[] allChildren =
            GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            child.DOKill();
        }
    }


    //================================================
    // Destroy
    //================================================

    private void OnDestroy()
    {
        KillAllTween();

        // 警告マークが残っていたら削除
        foreach (GameObject warning in warningMarks)
        {
            if (warning != null)
                Destroy(warning);
        }

        warningMarks.Clear();
    }
}