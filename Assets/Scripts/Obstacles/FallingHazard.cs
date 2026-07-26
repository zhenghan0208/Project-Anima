using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    public float destroyHeight = -5f;

    void Update()
    {
        if (transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(1, transform.position);
        }

        Destroy(gameObject);
    }
}