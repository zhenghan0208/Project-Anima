using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    public int damage = 1;
    public float attackCooldown = 1f;

    private float attackTimer;

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (attackTimer > 0)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.TakeDamage(damage, transform.position);

        attackTimer = attackCooldown;
    }
}