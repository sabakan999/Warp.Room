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

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip snapSE;

    [Header("エフェクト")]
    public GameObject snapEffect;

    private BossController bossController;

    private bool isMoving = true;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        if (animator == null)
            animator = GetComponent<Animator>();

        bossController = FindFirstObjectByType<BossController>();

        if (snapEffect != null)
            snapEffect.SetActive(false);

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

        float time = Random.Range(
            moveTime * 0.7f,
            moveTime * 1.3f
        );

        transform.DOMove(target, time)
            .SetEase(Ease.InOutSine)
            .OnComplete(MoveRandom);
    }

    //================================================
    // 攻撃開始
    //================================================
    public void BeginAttack()
    {
        isMoving = false;

        transform.DOKill();

        // 攻撃モーション開始
        if (animator != null)
        {
            animator.SetTrigger(attackTrigger);
        }

        // ★攻撃モーション開始時点で
        // ★攻撃Prefabを生成して警告状態にする
        if (bossController != null)
        {
            bossController.PrepareAttack();
        }
    }

    //================================================
    // Animation Event
    // 指パッチンの瞬間
    //================================================
    public void SpawnAttack()
    {
        // SE
        GameManager gm = FindFirstObjectByType<GameManager>();

        if (audioSource != null &&
            snapSE != null &&
            gm != null &&
            gm.isGameRunning)
        {
            audioSource.PlayOneShot(snapSE);
        }

        // エフェクト
        if (snapEffect != null)
        {
            snapEffect.SetActive(true);

            Animator effectAnimator =
                snapEffect.GetComponent<Animator>();

            if (effectAnimator != null)
            {
                effectAnimator.Rebind();
                effectAnimator.Update(0f);
                effectAnimator.Play(0, 0, 0f);
            }
        }

        // ★指パッチンの瞬間に攻撃を実体化
        if (bossController != null)
        {
            bossController.ActivateAttack();
        }
    }

    //================================================
    // Animation Event
    // エフェクト終了時
    //================================================
    public void HideEffect()
    {
        if (snapEffect != null)
            snapEffect.SetActive(false);
    }

    //================================================
    // Animation Event
    // 攻撃アニメーション終了
    //================================================
    public void EndAttack()
    {
        isMoving = true;

        MoveRandom();
    }

    //================================================
    // 会話開始
    //================================================
    public void MoveToTalkPosition()
    {
        isMoving = false;

        transform.DOKill();

        Vector3 target =
            talkPoint != null
                ? talkPoint.position
                : startPosition;

        transform.DOMove(target, returnTime)
            .SetEase(Ease.OutSine);
    }
}