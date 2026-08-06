using UnityEngine;
using DG.Tweening;

public class BossAttack : MonoBehaviour
{
    [Header("この攻撃パターンの継続時間")]
    public float duration = 5f;


    void Start()
    {
        Invoke(nameof(EndAttack), duration);
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