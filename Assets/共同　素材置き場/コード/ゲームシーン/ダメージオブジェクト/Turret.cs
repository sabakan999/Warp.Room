using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject missilePrefab;

    [Header("発射間隔")]
    public float interval = 2f;

    [Header("初回発射までの待機時間（0で即発射）")]
    public float firstDelay = 0f;

    // インスペクターで選べる方向
    public enum FireDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    public FireDirection fireDirection = FireDirection.Right;

    float timer;
    bool hasFiredOnce = false; // 🔥 初回判定

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual();

        // 🔥 初回タイマー調整
        timer = -firstDelay;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // =========================
        // 🔥 発射条件
        // =========================
        if (!hasFiredOnce)
        {
            // 初回
            if (timer >= 0f)
            {
                Fire();
                hasFiredOnce = true;
                timer = 0f;
            }
        }
        else
        {
            // 2回目以降
            if (timer >= interval)
            {
                Fire();
                timer = 0f;
            }
        }
    }

    void Fire()
    {
        GameObject missile = Instantiate(
            missilePrefab,
            transform.position,
            Quaternion.identity,
            transform.parent
        );

        Vector2 dir = GetDirectionVector();

        missile.GetComponent<Missile>().SetDirection(dir);
    }

    Vector2 GetDirectionVector()
    {
        switch (fireDirection)
        {
            case FireDirection.Right: return Vector2.right;
            case FireDirection.Left:  return Vector2.left;
            case FireDirection.Up:    return Vector2.up;
            case FireDirection.Down:  return Vector2.down;
        }

        return Vector2.right;
    }

    // 見た目更新
    void UpdateVisual()
    {
        if (sr == null) return;

        switch (fireDirection)
        {
            case FireDirection.Right:
                sr.flipX = false;
                transform.rotation = Quaternion.identity;
                break;

            case FireDirection.Left:
                sr.flipX = true;
                transform.rotation = Quaternion.identity;
                break;

            case FireDirection.Up:
                sr.flipX = false;
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;

            case FireDirection.Down:
                sr.flipX = false;
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }
    }
}