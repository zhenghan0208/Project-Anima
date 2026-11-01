
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CockroachChase : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float stopDistance = 0.8f;

    [Header("Jump")]
    public float jumpForce = 18f;
    public float jumpCooldown = 0.8f;
    public float jumpHeightThreshold = 6f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Obstacle Detection")]
    public float obstacleCheckDistance = 0.7f;
    public float obstacleCheckHeight = 0.5f;
    public float obstacleCheckRadius = 0.2f;

    [Header("Knockback")]
    public float knockbackRecoverTime = 0.25f;

    private Rigidbody rb;
    private bool isKnockedBack;
    private float knockbackTimer;
    private float jumpTimer;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Prevent the old movement script from controlling this Rigidbody.
        EnemyMovement oldMovement = GetComponent<EnemyMovement>();

        if (oldMovement != null)
            oldMovement.enabled = false;

        rb.constraints |= RigidbodyConstraints.FreezePositionZ;
    }

    void Start()
    {
        FindPlayer();

        if (player != null)
            UpdateFacing(player.position.x - transform.position.x);
    }

    void FixedUpdate()
    {
        jumpTimer -= Time.fixedDeltaTime;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;

            if (knockbackTimer <= 0f)
                isKnockedBack = false;
            else
                return;
        }

        if (player == null)
        {
            FindPlayer();

            if (player == null)
            {
                rb.linearVelocity = new Vector3(
                    0f, rb.linearVelocity.y, 0f
                );
                return;
            }
        }

        float differenceX = player.position.x - transform.position.x;
        float differenceY = player.position.y - transform.position.y;
        float direction = Mathf.Abs(differenceX) > 0.05f
            ? Mathf.Sign(differenceX)
            : 0f;

        UpdateFacing(differenceX);

        bool grounded = IsGrounded();

        // Move towards the player horizontally.
        float horizontalSpeed = Mathf.Abs(differenceX) > stopDistance
            ? direction * moveSpeed
            : 0f;

        rb.linearVelocity = new Vector3(
            horizontalSpeed,
            rb.linearVelocity.y,
            0f
        );

        if (!grounded || jumpTimer > 0f)
            return;

        bool playerAbove = differenceY > jumpHeightThreshold;
        bool obstacleAhead = IsObstacleAhead(direction);

        // Jump up to reach the player or get over an obstacle.
        if ((playerAbove || obstacleAhead) && direction != 0f)
        {
            Jump();
        }
    }

    bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    bool IsObstacleAhead(float direction)
    {
        if (direction == 0f)
            return false;

        Vector3 origin = transform.position
            + Vector3.up * obstacleCheckHeight;

        Vector3 forward = Vector3.right * direction;

        if (Physics.SphereCast(
            origin,
            obstacleCheckRadius,
            forward,
            out RaycastHit hit,
            obstacleCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore))
        {
            // Ignore ground directly underneath the enemy.
            return hit.collider != null;
        }

        return false;
    }

    void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;

        jumpTimer = jumpCooldown;
    }

    void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    void UpdateFacing(float difference)
    {
        if (Mathf.Abs(difference) < 0.01f)
            return;

        facingRight = difference > 0f;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    public void StartKnockback()
    {
        isKnockedBack = true;
        knockbackTimer = knockbackRecoverTime;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        Gizmos.color = Color.red;

        float direction = facingRight ? 1f : -1f;

        Vector3 origin = transform.position
            + Vector3.up * obstacleCheckHeight;

        Gizmos.DrawWireSphere(
            origin + Vector3.right * direction * obstacleCheckDistance,
            obstacleCheckRadius
        );
    }
}