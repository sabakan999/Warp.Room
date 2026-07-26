using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject missilePrefab;

    [Header("発射エフェクト")]
    public GameObject fireEffectPrefab;
    public float effectLifeTime = 0.5f;

    [Header("煙の発射位置")]
    public Transform effectRight;
    public Transform effectLeft;
    public Transform effectUp;
    public Transform effectDown;

    [Header("発射間隔")]
    public float interval = 2f;

    [Header("初回発射までの待機時間（0で即発射）")]
    public float firstDelay = 0f;

    [Header("SE")]
    public AudioClip fireSE;

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
    bool hasFiredOnce = false;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual();

        timer = -firstDelay;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!hasFiredOnce)
        {
            if (timer >= 0f)
            {
                Fire();
                hasFiredOnce = true;
                timer = 0f;
            }
        }
        else
        {
            if (timer >= interval)
            {
                Fire();
                timer = 0f;
            }
        }
    }

    void Fire()
    {

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (fireSE != null && gm != null && gm.isGameRunning)
        {
            MultiSEManager.Instance.PlaySE(fireSE);
        }
        // =========================
        // 発射エフェクト
        // =========================

        Transform effectPoint = GetEffectPoint();

        if (fireEffectPrefab != null && effectPoint != null)
        {
            GameObject effect = Instantiate(
                fireEffectPrefab,
                effectPoint.position,
                Quaternion.identity,
                transform.parent
            );

            Vector3 originalScale = effect.transform.localScale;

            switch (fireDirection)
            {
                case FireDirection.Right:
                    effect.transform.rotation = Quaternion.identity;
                    effect.transform.localScale = originalScale;
                    break;

                case FireDirection.Left:
                    effect.transform.rotation = Quaternion.identity;
                    effect.transform.localScale =
                        new Vector3(
                            -originalScale.x,
                            originalScale.y,
                            originalScale.z
                        );
                    break;

                case FireDirection.Up:
                    effect.transform.rotation =
                        Quaternion.Euler(0, 0, 90);
                    effect.transform.localScale = originalScale;
                    break;

                case FireDirection.Down:
                    effect.transform.rotation =
                        Quaternion.Euler(0, 0, -90);
                    effect.transform.localScale = originalScale;
                    break;
            }

            Animator anim = effect.GetComponent<Animator>();

            if (anim != null)
            {
                RuntimeAnimatorController controller =
                    anim.runtimeAnimatorController;

                if (controller != null &&
                    controller.animationClips.Length > 0)
                {
                    Destroy(effect,
                        controller.animationClips[0].length);
                }
                else
                {
                    Destroy(effect, effectLifeTime);
                }
            }
            else
            {
                Destroy(effect, effectLifeTime);
            }
        }

        // =========================
        // ミサイル生成（今まで通り中心）
        // =========================

        GameObject missile = Instantiate(
            missilePrefab,
            transform.position,
            Quaternion.identity,
            transform.parent
        );

        missile.GetComponent<Missile>()
            .SetDirection(GetDirectionVector());
    }

    Transform GetEffectPoint()
    {
        switch (fireDirection)
        {
            case FireDirection.Right:
                return effectRight;

            case FireDirection.Left:
                return effectLeft;

            case FireDirection.Up:
                return effectUp;

            case FireDirection.Down:
                return effectDown;
        }

        return effectRight;
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