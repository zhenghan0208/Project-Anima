using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class FallingIcicle : MonoBehaviour
{
    [Header("Rigidbody")]
    public Rigidbody rb;

    [Header("Animation")]
    public Animator animator;

    [Tooltip("Animator Trigger parameter")]
    public string breakTrigger = "Break";

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    [Header("Audio")]
    public AudioClip breakSFX;

    private bool hasHitGround;

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Start()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitGround)
            return;

        // =========================
        // Player
        // =========================

        if (other.CompareTag("Player"))
        {
            PlayerHealth health =
                other.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(1, transform.position);
            }

            // Icicle disappears after hitting player
            Destroy(gameObject);

            return;
        }

        // =========================
        // Ground
        // =========================

        if ((groundLayer.value &
            (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        HitGround();
    }

    void HitGround()
    {
        hasHitGround = true;

        // Stop immediately
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Play break animation
        if (animator != null)
        {
            animator.ResetTrigger(breakTrigger);
            animator.SetTrigger(breakTrigger);
        }

        // Play break SFX
        PlayBreakSFX();

        // Destroy after break animation
        float animationLength =
            GetBreakAnimationLength();

        if (animationLength > 0f)
        {
            Destroy(gameObject, animationLength);
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }

    float GetBreakAnimationLength()
    {
        if (animator == null)
            return 0f;

        RuntimeAnimatorController controller =
            animator.runtimeAnimatorController;

        if (controller == null)
            return 0f;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name.ToLower().Contains("break"))
            {
                return clip.length;
            }
        }

        return 0f;
    }

    void PlayBreakSFX()
    {
        if (AudioManager.Instance != null &&
            breakSFX != null)
        {
            AudioManager.Instance.PlaySFX(breakSFX);
        }
    }
}