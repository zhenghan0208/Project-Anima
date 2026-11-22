
using UnityEngine;

public class BigCockroachSplit : MonoBehaviour
{
    [Header("Small Cockroach")]
    public GameObject smallCockroachPrefab;
    public int smallCockroachCount = 4;
    public float spawnRadius = 1f;

    private bool hasSplit;

    public void SpawnSmallCockroaches()
    {
        if (hasSplit)
            return;

        hasSplit = true;

        if (smallCockroachPrefab == null)
        {
            Debug.LogWarning("BigCockroachSplit: Small prefab is missing.");
            return;
        }

        EnemyHealth parentHealth = GetComponent<EnemyHealth>();
        ArenaManager manager = parentHealth != null
            ? parentHealth.arenaManager
            : null;

        for (int i = 0; i < smallCockroachCount; i++)
        {
            float angle = i * (360f / smallCockroachCount);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * spawnRadius;

            Vector3 spawnPosition = transform.position + offset;

            GameObject small = Instantiate(
                smallCockroachPrefab,
                spawnPosition,
                Quaternion.identity
            );

            if (manager != null)
                manager.RegisterEnemy(small);
        }
    }
}