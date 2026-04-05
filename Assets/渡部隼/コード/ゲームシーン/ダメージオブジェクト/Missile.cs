using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 5f;

    // 外から指定する進行方向
    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        // 方向が未設定なら「向き」に従う
        if (direction == Vector2.zero)
        {
            direction = transform.right;
        }

        // 🔽 向きに合わせて回転
        UpdateRotation();
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // 砲台から呼ぶ用
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // 🔽 方向更新時にも回転
        UpdateRotation();
    }

    // 🔽 追加：見た目を進行方向に合わせる
    void UpdateRotation()
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}