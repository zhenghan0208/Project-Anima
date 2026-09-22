using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float breakDelay = 3f;
    public float breakAnimationTime = 0.6f;
    public float respawnTime = 3f;

    [Header("Animation")]
    public Animator animator;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    private Collider platformCollider;

    private bool isTriggered;

    void Awake()
    {
        platformCollider = GetComponent<Collider>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTriggered)
            return;

        if (!collision.collider.CompareTag("Player"))
            return;

        StartCoroutine(FallingRoutine());
    }

    IEnumerator FallingRoutine()
    {
        isTriggered = true;

        // StepOn animation
        if (animator != null)
        {
            animator.SetTrigger("StepOn");
            Debug.Log("StepOn Trigger!");
        }

        // Wait before breaking
        yield return new WaitForSeconds(breakDelay);

        // Disable Collider FIRST
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
            Debug.Log("Collider OFF!");
        }

        // Play Break animation
        if (animator != null)
        {
            animator.SetTrigger("Break");
            Debug.Log("Break Trigger!");
        }

        // Wait for Break animation
        yield return new WaitForSeconds(breakAnimationTime);

        // Disable Child SpriteRenderer
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            Debug.Log("SpriteRenderer OFF!");
        }

        // Wait before respawn
        yield return new WaitForSeconds(respawnTime);

        // Enable Child SpriteRenderer
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // Enable Collider
        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }

        isTriggered = false;
    }
}