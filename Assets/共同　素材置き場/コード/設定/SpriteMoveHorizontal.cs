using UnityEngine;

public class SpriteMoveHorizontal : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float speed = 5f;

    [Header("右方向ならON")]
    [SerializeField] private bool moveRight = true;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // 移動方向に合わせて反転
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !moveRight;
        }
    }

    private void Update()
    {
        float direction = moveRight ? 1f : -1f;

        transform.position += 
            Vector3.right * direction * speed * Time.deltaTime;
    }
}