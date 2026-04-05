using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private PlayerController player;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        bool isGrounded = player.IsGrounded();
        float inputX = Input.GetAxisRaw("Horizontal");

        bool isWalking = isGrounded && Mathf.Abs(inputX) > 0.1f;

        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
    }
}