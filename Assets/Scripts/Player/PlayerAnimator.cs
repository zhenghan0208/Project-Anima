using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerController controller;
    private GroundCheck groundCheck;

    void Awake()
    {
        animator = GetComponent<Animator>();

        controller = GetComponentInParent<PlayerController>();

        groundCheck = GetComponentInParent<GroundCheck>();
    }

    void Update()
    {
        animator.SetFloat(
            "Speed",
            Mathf.Abs(controller.GetMoveSpeed())
        );

        animator.SetBool(
            "Grounded",
            groundCheck.IsGrounded
        );
    }

    public void PlayHit()
    {
        animator.SetTrigger("Hit");
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}