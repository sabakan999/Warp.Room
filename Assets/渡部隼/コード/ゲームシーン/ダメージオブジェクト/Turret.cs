using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject missilePrefab;
    public float interval = 2f;

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

    SpriteRenderer sr; // 🔽 追加

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual(); // 🔽 初期向き設定
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            Fire();
            timer = 0f;
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
            case FireDirection.Right:
                return Vector2.right;
            case FireDirection.Left:
                return Vector2.left;
            case FireDirection.Up:
                return Vector2.up;
            case FireDirection.Down:
                return Vector2.down;
        }

        return Vector2.right;
    }

    // 🔽 見た目更新（ここが本体）
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