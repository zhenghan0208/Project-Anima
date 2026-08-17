using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 1f;

    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask groundLayer;

    [Header("Cooldown")]
    public float attackCooldown = 0.3f;

    private PlayerAbility playerAbility;
    private PlayerAnimator playerAnimator;

    private float attackTimer;

    void Start()
    {
        playerAbility = GetComponent<PlayerAbility>();

        playerAnimator = GetComponentInChildren<PlayerAnimator>();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (!playerAbility.hasWeapon)
            return;

        if (Input.GetMouseButtonDown(0) && attackTimer <= 0f)
        {
            Attack();
        }
    }

    void Attack()
    {
        attackTimer = attackCooldown;

        // Play attack animation
        if (playerAnimator != null)
        {
            playerAnimator.PlayAttack();
        }

        // Play attack sound
        PlayAttackSFX();

        // =========================
        // Enemy
        // =========================

        Collider[] enemyHits = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider hit in enemyHits)
        {
            EnemyHealth enemyHealth =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(
                    1,
                    transform.position
                );
            }
        }

        // =========================
        // Ground / Environment
        // =========================

        Collider[] groundHits = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius,
            groundLayer
        );

        foreach (Collider hit in groundHits)
        {
            // Breakable Block
            BreakableBlock breakableBlock =
                hit.GetComponentInParent<BreakableBlock>();

            if (breakableBlock != null)
            {
                breakableBlock.TakeDamage(1);
            }

            // L Shape Obstacle
            LShapeObstacle lShapeObstacle =
                hit.GetComponentInParent<LShapeObstacle>();

            if (lShapeObstacle != null)
            {
                lShapeObstacle.Hit(transform.position);
            }
        }
    }

    void PlayAttackSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.attackSFX
            );
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}