using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;

    [Header("ジャンプ設定")]
    public float jumpForce = 10f;

    [Header("重力設定")]
    public float gravityScale = 3f;

    [Header("接地判定")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("見た目")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isControllable = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.linearDamping = 0f;
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        if (!isControllable) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        Flip(moveInput);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (!isControllable) return;

        CheckGround();
        Move();
    }

    void Move()
    {
        float targetSpeed = moveInput * moveSpeed;

        if (moveInput == 0)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void Flip(float inputX)
    {
        if (inputX > 0) spriteRenderer.flipX = false;
        else if (inputX < 0) spriteRenderer.flipX = true;
    }

    public void DisableControl()
    {
        isControllable = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}