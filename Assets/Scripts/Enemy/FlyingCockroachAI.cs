
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlyingCockroachAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Flight")]
    public float flySpeed = 3f;
    public float stopDistance = 0.8f;

    [Header("Knockback")]
    public float knockbackRecoverTime = 0.25f;

    private Rigidbody rb;
    private bool isKnockedBack;
    private float knockbackTimer;
    private bool facingRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Flying enemies should not be controlled by ground movement.
        EnemyMovement oldMovement = GetComponent<EnemyMovement>();

        if (oldMovement != null)
            oldMovement.enabled = false;

        rb.useGravity = false;
        rb.constraints |= RigidbodyConstraints.FreezePositionZ;
    }

    void Start()
    {
        FindPlayer();
    }

    void FixedUpdate()
    {
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
                rb.linearVelocity = Vector3.zero;
                return;
            }
        }

        Vector3 difference = player.position - transform.position;
        difference.z = 0f;

        if (difference.magnitude <= stopDistance)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 direction = difference.normalized;

        rb.linearVelocity = direction * flySpeed;
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            rb.linearVelocity.y,
            0f
        );

        UpdateFacing(difference.x);
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    void UpdateFacing(float differenceX)
    {
        if (Mathf.Abs(differenceX) < 0.01f)
            return;

        bool shouldFaceRight = differenceX > 0f;

        if (shouldFaceRight == facingRight)
            return;

        facingRight = shouldFaceRight;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    public void StartKnockback()
    {
        isKnockedBack = true;
        knockbackTimer = knockbackRecoverTime;
    }
}