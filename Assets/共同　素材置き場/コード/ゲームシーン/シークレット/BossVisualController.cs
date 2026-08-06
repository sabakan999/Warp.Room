using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class BossVisualController : MonoBehaviour
{
    [Header("移動範囲")]
    public Vector2 minPosition = new Vector2(-7f, -3f);
    public Vector2 maxPosition = new Vector2(7f, 4f);

    [Header("移動")]
    public float moveTime = 2f;

    [Header("アニメーション")]
    public Animator animator;
    public string attackTrigger = "Attack";

    [Header("会話位置")]
    public Transform talkPoint;
    public float returnTime = 1f;

    bool isMoving = true;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        if (animator == null)
            animator = GetComponent<Animator>();

        MoveRandom();
    }

    void MoveRandom()
    {
        if (!isMoving)
            return;

        Vector3 target = new Vector3(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y),
            transform.position.z
        );

        float time = Random.Range(moveTime * 0.7f, moveTime * 1.3f);

        transform.DOMove(target, time)
            .SetEase(Ease.InOutSine)
            .OnComplete(MoveRandom);
    }

    //========================
    // 攻撃開始
    //========================
    public void BeginAttack()
    {
         Debug.Log("BeginAttack");

        isMoving = false;

        transform.DOKill();

        if (animator != null)
            animator.SetTrigger(attackTrigger);
    }

    //========================
    // 攻撃終了
    //========================
    public void EndAttack()
    {
        isMoving = true;

        MoveRandom();
    }

    //========================
    // 会話開始
    //========================
    public void MoveToTalkPosition()
    {
        isMoving = false;

        transform.DOKill();

        Vector3 target =
            talkPoint != null ?
            talkPoint.position :
            startPosition;

        transform.DOMove(target, returnTime)
            .SetEase(Ease.OutSine);
    }
}