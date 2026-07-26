using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    [Header("Health")]
    public int health = 1;

    // TODO
    // Particle Effect

    // TODO
    // Sound Effect

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Break();
        }
    }

    void Break()
    {
        // TODO
        // Play Effect

        Destroy(gameObject);
    }
}