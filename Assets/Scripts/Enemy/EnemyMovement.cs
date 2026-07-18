using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Knockback")]
    public float knockbackRecoverTime = 0.2f;
    private bool isKnockedBack;

    private Rigidbody rb;
    private int direction = 1;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
            return;

        bool hasGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (!hasGround)
        {
            direction *= -1;
            HandleFacing();
        }

        rb.linearVelocity = new Vector3(direction * moveSpeed, rb.linearVelocity.y, 0);
    }

    void HandleFacing()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void StartKnockback()
    {
        isKnockedBack = true;

        CancelInvoke(nameof(EndKnockback));
        Invoke(nameof(EndKnockback), knockbackRecoverTime);
    }

    void EndKnockback()
    {
        isKnockedBack = false;
    }
}