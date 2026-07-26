using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float disappearDelay = 0.8f;
    public float respawnTime = 3f;

    private Collider platformCollider;
    private Renderer[] renderers;

    private bool isTriggered;

    void Awake()
    {
        platformCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTriggered)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(DisappearRoutine());
        }
    }

    IEnumerator DisappearRoutine()
    {
        isTriggered = true;

        yield return new WaitForSeconds(disappearDelay);

        // Hide platform
        platformCollider.enabled = false;

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        yield return new WaitForSeconds(respawnTime);

        // Show platform
        platformCollider.enabled = true;

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        isTriggered = false;
    }
}