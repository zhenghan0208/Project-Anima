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
    public float jumpForce = 14f;
    public float gravity = -35f;
    public float maxFallSpeed = -25f;

    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 3f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private GroundCheck groundCheck;

    private Vector3 velocity;

    private float coyoteCounter;
    private float jumpBufferCounter;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        groundCheck = GetComponent<GroundCheck>();

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        groundCheck.CheckGround();

        UpdateJumpTimers();

        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();

        ApplyGravity();

        velocity.y = Mathf.Max(
            velocity.y,
            maxFallSpeed);

        rb.linearVelocity = velocity;
    }

    void HandleMovement()
    {
        float targetSpeed = playerInput.MoveInput * moveSpeed;

        float acceleration;

        if (groundCheck.IsGrounded)
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.01f ?
                groundAcceleration :
                groundDeceleration;
        }
        else
        {
            acceleration = Mathf.Abs(targetSpeed) > 0.01f ?
                airAcceleration :
                airDeceleration;
        }

        velocity.x = Mathf.MoveTowards(
            velocity.x,
            targetSpeed,
            acceleration * Time.fixedDeltaTime);
    }

    void HandleJump()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            velocity.y = jumpForce;

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;

            playerInput.ResetJump();
        }

        if (!playerInput.JumpHeld && velocity.y > 0)
        {
            velocity.y += gravity *
                (lowJumpMultiplier - 1) *
                Time.deltaTime;
        }
    }

    void ApplyGravity()
    {
        if (velocity.y < 0)
        {
            velocity.y += gravity *
                fallMultiplier *
                Time.fixedDeltaTime;
        }
        else
        {
            velocity.y += gravity *
                Time.fixedDeltaTime;
        }
    }

    void UpdateJumpTimers()
    {
        if (groundCheck.IsGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (playerInput.JumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            transform.position,
            transform.position + velocity);
    }
}