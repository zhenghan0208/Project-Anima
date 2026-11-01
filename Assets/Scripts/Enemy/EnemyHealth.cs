
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

    [Header("Arena")]
    public ArenaManager arenaManager;

    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        propertyBlock = new MaterialPropertyBlock();

        if (meshRenderers == null)
            meshRenderers = new MeshRenderer[0];

        originalColors = new Color[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] == null)
                continue;

            Material material = meshRenderers[i].sharedMaterial;

            if (material != null && material.HasProperty("_BaseColor"))
                originalColors[i] = material.GetColor("_BaseColor");
            else
                originalColors[i] = Color.white;
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        ApplyKnockback(hitPoint);

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    void ApplyKnockback(Vector3 hitPoint)
    {
        if (rb == null)
            return;

        Vector3 direction = (transform.position - hitPoint).normalized;
        direction.y = 0;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        EnemyMovement movement = GetComponent<EnemyMovement>();

        if (movement != null)
            movement.StartKnockback();

        CockroachChase chase = GetComponent<CockroachChase>();

        if (chase != null)
        {
            chase.StartKnockback();
        }

        FlyingCockroachAI flyingAI = GetComponent<FlyingCockroachAI>();

        if (flyingAI != null)
        {
            flyingAI.StartKnockback();
        }
    }

    IEnumerator HitFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] == null)
                continue;

            meshRenderers[i].GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", hitColor);
            meshRenderers[i].SetPropertyBlock(propertyBlock);
        }

        yield return new WaitForSeconds(hitFlashTime);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] == null)
                continue;

            meshRenderers[i].GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", originalColors[i]);
            meshRenderers[i].SetPropertyBlock(propertyBlock);
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Big cockroach spawns small cockroaches before it is removed.
        BigCockroachSplit split = GetComponent<BigCockroachSplit>();

        if (split != null)
            split.SpawnSmallCockroaches();

        // Remove this enemy from the arena's alive count.
        if (arenaManager != null)
            arenaManager.UnregisterEnemy(gameObject);

        Destroy(gameObject);
    }
}
