using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;

    [Header("Cooldown")]
    public float attackCooldown = 0.3f;

    private PlayerAbility playerAbility;
    private PlayerController playerController;

    private float attackTimer;

    void Start()
    {
        playerAbility = GetComponent<PlayerAbility>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (!playerAbility.hasWeapon)
            return;

        if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
        {
            Attack();
        }
    }

    void Attack()
    {
        attackTimer = attackCooldown;

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius,
            attackLayer);

        foreach (Collider hit in hits)
        {
            // Enemy
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1, transform.position);
            }

            // Breakable Block
            BreakableBlock breakableBlock = hit.GetComponent<BreakableBlock>();

            if (breakableBlock != null)
            {
                breakableBlock.TakeDamage(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}