using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DVDEnemy : MonoBehaviour
{
    [Header("移動")]
    public Vector2 startDirection = new Vector2(1, 1);
    public float speed = 5f;

    [Header("見た目")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private Rigidbody2D rb;
    private int spriteIndex = 0;

    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip bounceSE;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        


        rb.linearVelocity = startDirection.normalized * speed;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 最初のスプライト
        if (sprites.Length > 0 && spriteRenderer != null)
            spriteRenderer.sprite = sprites[0];
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeSprite();

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (audioSource != null && bounceSE != null && gm != null && gm.isGameRunning)
    {
        audioSource.PlayOneShot(bounceSE);
    }
    }

    void ChangeSprite()
    {
        if (spriteRenderer == null || sprites.Length == 0)
            return;

        spriteIndex++;

        if (spriteIndex >= sprites.Length)
            spriteIndex = 0;

        spriteRenderer.sprite = sprites[spriteIndex];
    }
}