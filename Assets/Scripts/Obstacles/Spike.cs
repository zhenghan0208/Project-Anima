using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, transform.position);
        }
    }
}