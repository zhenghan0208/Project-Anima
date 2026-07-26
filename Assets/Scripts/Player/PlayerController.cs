using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(GroundCheck))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float groundAcceleration = 80f;
    public float groundDeceleration = 100f;
    public float airAcceleration = 45f;
    public float airDeceleration = 50f;

    [Header("Jump")]
    public float jumpForce = 18f;
    public float gravity = -35f;
    public float maxFallSpeed = -18f;

    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 3f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.12f;
    private float coyoteCounter;

    [Header("Double Jump")]
    private bool canDoubleJump;

    [Header("Facing")]
    public bool facingRight = true;

    [Header("Ceiling Check")]
    public Transform ceilingCheck;
    public float ceilingRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    public float knockbackUpForce = 10f;
    public float knockbackRecoverSpeed = 25f;
    private bool isKnockback;
    private float knockbackTimer;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private GroundCheck groundCheck;
    private Vector3 velocity;
    private PlayerAbility playerAbility;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        groundCheck = GetComponent<GroundCheck>();
        playerAbility = GetComponent<PlayerAbility>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        groundCheck.CheckGround();

        UpdateCoyoteTime();

        HandleJump();

        HandleFacing();

        HandleDash();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            Dash();

            return;
        }

        if (isKnockback)
        {
            knockbackTimer -= Time.fixedDeltaTime;

            velocity.x = Mathf.MoveTowards(velocity.x, 0, knockbackRecoverSpeed * Time.fixedDeltaTime);

            if (knockbackTimer <= 0)
            {
                isKnockback = false;
            }
        }
        else
        {
            HandleMovement();
        }

        CheckCeiling();

        if (!isDashing)
        {
            ApplyGravity();
        }

        velocity.y = Mathf.Max(velocity.y, maxFallSpeed);

        rb.linearVelocity = velocity;
    }

    void HandleMovement()
    {
        if (isDashing)
            return;

        float targetSpeed = playerInput.MoveInput * moveSpeed;

        float acceleration;

        if (groundCheck.IsGrounded)
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.01f ? groundAcceleration : groundDeceleration;
        }
        else
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.01f ? airAcceleration : airDeceleration;
        }

        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
    }

    public void ApplyKnockback(Vector3 hitPoint, float duration = 0.2f)
    {
        Vector3 direction = (transform.position - hitPoint).normalized;

        direction.y = 0;

        velocity.x = direction.x * knockbackForce;
        velocity.y = knockbackUpForce;

        isKnockback = true;
        knockbackTimer = duration;
    }

    void HandleFacing()
    {
        if (playerInput.MoveInput > 0.01f)
        {
            facingRight = true;
        }
        else if (playerInput.MoveInput < -0.01f)
        {
            facingRight = false;
        }

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);

        transform.localScale = scale;
    }

    void HandleJump()
    {
        if (playerInput.JumpPressed)
        {
            if (coyoteCounter > 0f)
            {
                velocity.y = jumpForce;

                coyoteCounter = 0f;

                playerInput.ResetJump();
            }
            else if (playerAbility.hasDoubleJump && canDoubleJump)
            {
                velocity.y = jumpForce;

                canDoubleJump = false;

                playerInput.ResetJump();
            }
        }

        if (!playerInput.JumpHeld && velocity.y > 0)
        {
            velocity.y += gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void ApplyGravity()
    {
        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
    }

    void UpdateCoyoteTime()
    {
        if (groundCheck.IsGrounded)
        {
            coyoteCounter = coyoteTime;

            canDoubleJump = true;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    void CheckCeiling()
    {
        bool hitCeiling = Physics.CheckSphere(ceilingCheck.position, ceilingRadius, groundLayer);

        if (hitCeiling && velocity.y > 0)
        {
            velocity.y = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + velocity);

        if (ceilingCheck != null)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingRadius);
        }
    }

    void HandleDash()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (!playerAbility.hasDash)
            return;

        if (!playerInput.DashPressed)
            return;

        if (isDashing)
            return;

        if (dashCooldownTimer > 0)
            return;

        isDashing = true;

        dashTimer = dashDuration;

        dashCooldownTimer = dashCooldown;

        playerInput.ResetDash();
    }

    void Dash()
    {
        dashTimer -= Time.fixedDeltaTime;

        float direction = facingRight ? 1f : -1f;

        velocity.x = direction * dashSpeed;
        velocity.y = 0;

        if (dashTimer <= 0)
        {
            isDashing = false;
        }
    }

    public float GetMoveSpeed()
    {
        return velocity.x;
    }
}