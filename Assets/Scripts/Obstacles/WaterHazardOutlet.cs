
using UnityEngine;
using System.Collections.Generic;

public class WaterHazardOutlet : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Collider waterDamageZone;

    [Header("Animation State Names")]
    public string idleState = "Idle";
    public string readyState = "Ready";
    public string pourState = "Pour";

    [Header("Damage")]
    public int damage = 1;

    private HashSet<PlayerHealth> hitPlayers =
        new HashSet<PlayerHealth>();

    private bool isPouring;

    private void Start()
    {
        if (waterDamageZone != null)
            waterDamageZone.enabled = false;

        if (animator != null)
            animator.Play(idleState);
    }

    public void PlayReady()
    {
        if (animator != null)
            animator.Play(readyState);
    }

    public void StartPour()
    {
        isPouring = true;

        hitPlayers.Clear();

        if (animator != null)
            animator.Play(pourState);

        if (waterDamageZone != null)
            waterDamageZone.enabled = true;
    }

    public void StopPour()
    {
        isPouring = false;

        if (waterDamageZone != null)
            waterDamageZone.enabled = false;

        if (animator != null)
            animator.Play(idleState);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider other)
    {
        if (!isPouring)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        // Only damage each player once per water pour.
        if (hitPlayers.Contains(playerHealth))
            return;

        hitPlayers.Add(playerHealth);

        // Use the water damage zone position as the hit point.
        Vector3 hitPoint = waterDamageZone != null
            ? waterDamageZone.bounds.center
            : transform.position;

        playerHealth.TakeDamage(damage, hitPoint);
    }

    private void OnDisable()
    {
        isPouring = false;

        if (waterDamageZone != null)
            waterDamageZone.enabled = false;
    }
}