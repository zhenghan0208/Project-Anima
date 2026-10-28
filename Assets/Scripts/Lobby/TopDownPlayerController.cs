using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TopDownPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;
    private Vector3 moveDirection;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float gravity = -25f;
    private float verticalVelocity;

    private CharacterController controller;

    public float CurrentSpeed { get; private set; }

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        ApplyGravity();

        HandleJump();

        HandleMovement();
    }

    void HandleMovement()
    {
        bool grounded = controller.isGrounded;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);

        if (input.sqrMagnitude > 1f)
            input.Normalize();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * input.z + right * input.x;

        Vector3 velocity = moveDirection * moveSpeed;

        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        CurrentSpeed = moveDirection.magnitude;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection) *
                Quaternion.Euler(0, 90, 0);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    void HandleJump()
    {
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            PlayJumpSFX();
        }
    }


    void PlayJumpSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }
    }
}