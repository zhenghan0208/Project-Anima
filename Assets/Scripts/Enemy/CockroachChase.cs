
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CockroachChase : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Rigidbody rb;
    public Collider bodyCollider;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public bool randomizeSpeed = true;
    public float minSpeed = 2.2f;
    public float maxSpeed = 3.0f;
    public float stopDistance = 0.5f;
    public float verticalStopDistance = 1.5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float jumpCooldown = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Ground Ahead Check")]
    public float groundAheadDistance = 0.5f;

    [Header("Platform Detection")]
    [Tooltip("检测前方较高的平台")]
    public float platformCheckDistance = 5.5f;
    public float platformCheckHeight = 5.5f;
    public float platformCheckRadius = 0.2f;
    public float minimumPlatformHeight = 5f;

    [Header("Obstacle Detection")]
    [Tooltip("检测正前方的障碍物，不需要高度条件")]
    public float obstacleCheckDistance = 2.5f;
    public float obstacleCheckRadius = 0.2f;

    [Header("Special Direction")]
    public float alignmentXThreshold = 0.3f;
    public float specialDirectionVerticalDistance = 1.5f;

    [Header("Knockback")]
    public float knockbackPauseDuration = 0.5f;

    private float jumpTimer;
    private float knockbackTimer;

    private bool facingRight = true;
    private CockroachAnimator cockroachAnimator;

    private bool specialDirectionActive;
    private float specialDirection;

    private float lastDirection = 1f;

    private void Awake()
    {
        cockroachAnimator = GetComponent<CockroachAnimator>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (bodyCollider == null)
            bodyCollider = GetComponent<Collider>();

        // Disable the old EnemyMovement script
        EnemyMovement oldMovement = GetComponent<EnemyMovement>();

        if (oldMovement != null)
            oldMovement.enabled = false;

        // Lock Z movement
        rb.constraints |= RigidbodyConstraints.FreezePositionZ;
    }

    private void Start()
    {
        FindPlayer();

        if (randomizeSpeed)
        {
            moveSpeed = Random.Range(minSpeed, maxSpeed);
        }

        if (player != null)
        {
            float differenceX = player.position.x - transform.position.x;
            lastDirection = differenceX >= 0f ? 1f : -1f;
        }
        else
        {
            lastDirection = transform.position.x < 0f ? 1f : -1f;
        }

        facingRight = lastDirection > 0f;

        if (cockroachAnimator != null)
        {
            cockroachAnimator.SetFacingDirection(lastDirection);
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        // Update timers
        if (jumpTimer > 0f)
            jumpTimer -= Time.fixedDeltaTime;

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        if (player == null)
            FindPlayer();

        if (player == null)
        {
            StopHorizontalMovement();
            return;
        }

        float differenceX = player.position.x - transform.position.x;
        float differenceY = player.position.y - transform.position.y;

        float absDifferenceX = Mathf.Abs(differenceX);
        float absDifferenceY = Mathf.Abs(differenceY);

        // Check whether enemy is not moving
        bool enemyIsStopped =
            Mathf.Abs(rb.linearVelocity.x) <= 0.1f;

        // Activate special direction when enemy is stopped
        if (!specialDirectionActive &&
            enemyIsStopped &&
            absDifferenceY > specialDirectionVerticalDistance)
        {
            specialDirectionActive = true;

            // Enemy left of world X = 0 moves right.
            // Enemy right of world X = 0 moves left.
            specialDirection = transform.position.x < 0f ? 1f : -1f;
        }

        // Select movement direction
        float direction;

        if (specialDirectionActive)
        {
            direction = specialDirection;
        }
        else
        {
            if (absDifferenceX <= stopDistance)
            {
                direction = 0f;
            }
            else
            {
                direction = differenceX > 0f ? 1f : -1f;
            }
        }

        if (direction == 0f)
        {
            StopHorizontalMovement();
            return;
        }

        lastDirection = direction;
        facingRight = direction > 0f;

        if (cockroachAnimator != null)
        {
            cockroachAnimator.SetFacingDirection(direction);
        }

        bool grounded = IsGrounded();

        // Ground Ahead Check
        bool hasGroundAhead = HasGroundAhead(direction);

        // Platform Detection
        bool hasPlatform = IsPlatformAhead(direction);

        // Obstacle Detection
        bool hasObstacle = IsObstacleAhead(direction);

        bool playerAbove = differenceY > verticalStopDistance;
        bool playerBelow = differenceY < -verticalStopDistance;

        // Jump conditions
        bool shouldJump =
            (!hasGroundAhead && !playerBelow) ||
            (playerAbove && hasPlatform) ||
            hasObstacle;

        if (grounded && shouldJump && jumpTimer <= 0f)
        {
            Jump();
        }

        // Horizontal movement
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private bool IsGrounded()
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

    private bool HasGroundAhead(float direction)
    {
        if (groundCheck == null || direction == 0f)
            return true;

        Vector3 checkPosition =
            groundCheck.position +
            Vector3.right * direction * groundAheadDistance;

        return Physics.CheckSphere(
            checkPosition,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    // Original Obstacle Detection logic, renamed to Platform Detection.
    // Detects raised platforms ahead of the enemy.
    private bool IsPlatformAhead(float direction)
    {
        if (direction == 0f)
            return false;

        float feetY = bodyCollider != null
            ? bodyCollider.bounds.min.y
            : transform.position.y;

        Vector3 origin = new Vector3(
            transform.position.x,
            feetY + platformCheckHeight,
            transform.position.z
        );

        Vector3 forward = Vector3.right * direction;

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            platformCheckRadius,
            forward,
            platformCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.root == transform.root)
                continue;

            if (hit.point.y > feetY + minimumPlatformHeight)
                return true;
        }

        return false;
    }

    // New Obstacle Detection.
    // Uses the same SphereCast method and Ground Layer,
    // but checks directly in front without a height condition.
    private bool IsObstacleAhead(float direction)
    {
        if (direction == 0f)
            return false;

        float bodyHeight = bodyCollider != null
            ? bodyCollider.bounds.center.y
            : transform.position.y;

        Vector3 origin = new Vector3(
            transform.position.x,
            bodyHeight,
            transform.position.z
        );

        Vector3 forward = Vector3.right * direction;

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            obstacleCheckRadius,
            forward,
            obstacleCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.root == transform.root)
                continue;

            return true;
        }

        return false;
    }

    private void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;

        jumpTimer = jumpCooldown;

        // Unlock special direction after jumping.
        specialDirectionActive = false;
    }

    private void StopHorizontalMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = 0f;
        rb.linearVelocity = velocity;
    }

    // Called by EnemyHealth when the cockroach receives knockback.
    public void StartKnockback()
    {
        knockbackTimer = knockbackPauseDuration;
    }

    private void OnDrawGizmosSelected()
    {
        // Ground Check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );

            // Ground Ahead Check
            Gizmos.color = Color.yellow;

            Vector3 aheadPosition =
                groundCheck.position +
                Vector3.right * lastDirection * groundAheadDistance;

            Gizmos.DrawWireSphere(
                aheadPosition,
                groundCheckRadius
            );
        }

        float feetY = bodyCollider != null
            ? bodyCollider.bounds.min.y
            : transform.position.y;

        float bodyHeight = bodyCollider != null
            ? bodyCollider.bounds.center.y
            : transform.position.y;

        // Platform Detection
        Gizmos.color = Color.cyan;

        Vector3 platformOrigin = new Vector3(
            transform.position.x,
            feetY + platformCheckHeight,
            transform.position.z
        );

        Vector3 platformEnd =
            platformOrigin +
            Vector3.right * lastDirection * platformCheckDistance;

        Gizmos.DrawWireSphere(platformOrigin, platformCheckRadius);
        Gizmos.DrawWireSphere(platformEnd, platformCheckRadius);
        Gizmos.DrawLine(platformOrigin, platformEnd);

        // Obstacle Detection
        Gizmos.color = Color.red;

        Vector3 obstacleOrigin = new Vector3(
            transform.position.x,
            bodyHeight,
            transform.position.z
        );

        Vector3 obstacleEnd =
            obstacleOrigin +
            Vector3.right * lastDirection * obstacleCheckDistance;

        Gizmos.DrawWireSphere(obstacleOrigin, obstacleCheckRadius);
        Gizmos.DrawWireSphere(obstacleEnd, obstacleCheckRadius);
        Gizmos.DrawLine(obstacleOrigin, obstacleEnd);
    }
}