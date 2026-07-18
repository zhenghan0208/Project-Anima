using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask enemyLayer;

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

        Collider[] enemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1, transform.position);
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