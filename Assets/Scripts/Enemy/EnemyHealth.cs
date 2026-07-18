using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 2;
    public int currentHealth;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    private Rigidbody rb;

    [Header("Visual")]
    public MeshRenderer[] meshRenderers;
    public Color hitColor = Color.red;
    public float hitFlashTime = 0.1f;
    private MaterialPropertyBlock propertyBlock;
    private Color[] originalColors;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;

        propertyBlock = new MaterialPropertyBlock();

        originalColors = new Color[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].GetPropertyBlock(propertyBlock);

            originalColors[i] = meshRenderers[i].sharedMaterial.GetColor("_BaseColor");
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        currentHealth -= damage;

        ApplyKnockback(hitPoint);

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ApplyKnockback(Vector3 hitPoint)
    {
        Vector3 direction = (transform.position - hitPoint).normalized;

        direction.y = 0;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        EnemyMovement movement = GetComponent<EnemyMovement>();

        if (movement != null)
        {
            movement.StartKnockback();
        }
    }

    IEnumerator HitFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            propertyBlock.Clear();

            meshRenderers[i].GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor("_BaseColor", hitColor);

            meshRenderers[i].SetPropertyBlock(propertyBlock);
        }

        yield return new WaitForSeconds(hitFlashTime);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            propertyBlock.Clear();

            meshRenderers[i].GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor("_BaseColor", originalColors[i]);

            meshRenderers[i].SetPropertyBlock(propertyBlock);
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}