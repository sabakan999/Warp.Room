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

    // 🔥 ブラックホール影響
    [Header("ブラックホール影響")]

     [Header("行動禁止系フラグ")]
    private bool canJump = true;
    private bool canMove = true;

     
    public bool isPulled = false;
    public float pulledControlPower = 0.5f;

    // 🔊 追加：ジャンプSE
    [Header("SE")]
    public AudioSource audioSource;
    public AudioClip jumpSE;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isControllable = true;
    public bool CanControl => isControllable;

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
        if (Time.timeScale == 0f)
            return;
        if (!isControllable)
            return;

        moveInput = Input.GetAxisRaw("Horizontal");
        Flip(moveInput);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;
        if (!isControllable)
            return;

        CheckGround();
        Move();
    }

    void Move()
    {

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        float control = isPulled ? pulledControlPower : 1f;
        float targetSpeed = moveInput * moveSpeed * control;

        if (!isPulled)
        {
            if (moveInput == 0)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        }
        else
        {
            float speedDiff = targetSpeed - rb.linearVelocity.x;
            rb.AddForce(new Vector2(speedDiff, 0f), ForceMode2D.Force);
        }
    }

    void Jump()
    {

        if (!canJump)
        {

            return;
        }
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // 🔊 ジャンプSE
        if (audioSource != null && jumpSE != null)
        {
            audioSource.PlayOneShot(jumpSE);
        }
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
    }

        public void EnableControl()
    {
        isControllable = true;
        
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
    //禁止系の受け付け

    public void SetJumpEnabled(bool enable)
{
    canJump = enable;
}

public void SetMoveEnabled(bool enable)
{
    canMove = enable;
}
}