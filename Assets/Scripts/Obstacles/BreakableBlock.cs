using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    [Header("Health")]
    public int health = 1;

    [Header("Particle Effect")]
    public GameObject destroyParticle;

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
        PlayDestroySFX();

        PlayDestroyParticle();

        Destroy(gameObject);
    }

    void PlayDestroySFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.destroySFX
            );
        }
    }

    void PlayDestroyParticle()
    {
        if (destroyParticle == null)
            return;

        Instantiate(
            destroyParticle,
            transform.position,
            Quaternion.identity
        );
    }
}