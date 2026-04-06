using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MoveType
    {
        Loop,   // 往復
        OneWay  // 一回だけ
    }

    [Header("移動タイプ")]
    public MoveType moveType = MoveType.Loop;

    [Header("移動量（ローカル座標）")]
    public Vector3 localTargetOffset;

    [Header("速度")]
    public float speed = 2f;

    [Header("到達後待機時間（Loop用）")]
    public float waitTime = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 currentTarget;

    private bool goingToTarget = true;
    private float waitTimer = 0f;
    private bool hasFinished = false;

    void Start()
    {
        // 初期位置（ワールド）
        startPosition = transform.position;

        // ローカル → ワールドに変換
        targetPosition = startPosition + localTargetOffset;

        // 最初の目標
        currentTarget = targetPosition;
    }

    void Update()
    {
        if (moveType == MoveType.OneWay && hasFinished) return;

        Move();
    }

    void Move()
    {
        // 移動
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            speed * Time.deltaTime
        );

        // 到達判定
        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            OnReached();
        }
    }

    void OnReached()
    {
        if (moveType == MoveType.OneWay)
        {
            hasFinished = true;
            return;
        }

        // 待機処理
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitTime)
        {
            waitTimer = 0f;

            if (goingToTarget)
            {
                currentTarget = startPosition;
            }
            else
            {
                currentTarget = targetPosition;
            }

            goingToTarget = !goingToTarget;
        }
    }
}