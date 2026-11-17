using System.Collections.Generic;
using UnityEngine;

public class InsecticideAbility : MonoBehaviour
{
    [Header("Insecticide Settings")]
    public KeyCode useKey = KeyCode.F;
    public float attackRange = 8f;
    [Range(0f, 180f)]
    public float attackAngle = 90f;
    public int damage = 5;

    [Header("Facing Direction")]
    public bool facingRight = true;

    private bool hasInsecticide;
    private InsecticideManager manager;

    void Update()
    {
        if (!hasInsecticide)
            return;

        if (Input.GetKeyDown(useKey))
        {
            UseInsecticide();
        }
    }

    public bool TryPickup()
    {
        if (hasInsecticide)
            return false;

        hasInsecticide = true;

        Debug.Log("Press F to use Insecticide!");

        return true;
    }

    public void SetManager(InsecticideManager newManager)
    {
        manager = newManager;
    }

    // Set facing direction from PlayerController
    public void SetFacingDirection(bool isFacingRight)
    {
        facingRight = isFacingRight;
    }

    Vector3 GetAttackDirection()
    {
        return facingRight ? Vector3.right : Vector3.left;
    }

    void UseInsecticide()
    {
        if (!hasInsecticide)
            return;

        hasInsecticide = false;

        Vector3 attackDirection = GetAttackDirection();

        Collider[] hitColliders =
            Physics.OverlapSphere(transform.position, attackRange);

        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider hit in hitColliders)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemy == null || damagedEnemies.Contains(enemy))
                continue;

            Vector3 directionToEnemy =
                enemy.transform.position - transform.position;

            // 2D: Only calculate X and Y directions
            directionToEnemy.z = 0f;

            if (directionToEnemy.sqrMagnitude > attackRange * attackRange)
                continue;

            directionToEnemy.Normalize();

            float angle = Vector3.Angle(
                attackDirection,
                directionToEnemy
            );

            if (angle <= attackAngle / 2f)
            {
                damagedEnemies.Add(enemy);

                enemy.TakeDamage(damage, transform.position);
            }
        }

        Debug.Log("Insecticide Used!");

        if (manager != null)
            manager.StartRespawnCooldown();
    }

    void OnDrawGizmosSelected()
    {
        Vector3 attackDirection = GetAttackDirection();

        Vector3 leftDirection = Quaternion.Euler(
            0f, 0f, attackAngle / 2f
        ) * attackDirection;

        Vector3 rightDirection = Quaternion.Euler(
            0f, 0f, -attackAngle / 2f
        ) * attackDirection;

        Gizmos.color = Color.green;

        Gizmos.DrawRay(
            transform.position,
            leftDirection * attackRange
        );

        Gizmos.DrawRay(
            transform.position,
            rightDirection * attackRange
        );

        // Draw the arc
        int segments = 30;

        Vector3 previousPoint =
            transform.position + leftDirection * attackRange;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(
                attackAngle / 2f,
                -attackAngle / 2f,
                i / (float)segments
            );

            Vector3 direction = Quaternion.Euler(
                0f, 0f,
                facingRight ? -angle : 180f + angle
            ) * Vector3.right;

            Vector3 currentPoint =
                transform.position + direction * attackRange;

            Gizmos.DrawLine(previousPoint, currentPoint);

            previousPoint = currentPoint;
        }
    }
}