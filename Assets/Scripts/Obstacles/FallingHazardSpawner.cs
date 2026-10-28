using UnityEngine;

public class FallingHazardSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject[] hazardPrefabs;

    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Timing")]
    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 1.5f;

    private float timer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnHazard();
            ResetTimer();
        }
    }

    void SpawnHazard()
    {
        if (hazardPrefabs == null || hazardPrefabs.Length == 0)
            return;

        float randomX = Random.Range(leftPoint.position.x, rightPoint.position.x);

        Vector3 spawnPos = new Vector3(
            randomX,
            transform.position.y,
            transform.position.z);

        GameObject randomPrefab = hazardPrefabs[
            Random.Range(0, hazardPrefabs.Length)];

        Instantiate(randomPrefab, spawnPos, Quaternion.identity);
    }

    void ResetTimer()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void OnDrawGizmos()
    {
        if (leftPoint == null || rightPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(leftPoint.position, 0.15f);
        Gizmos.DrawSphere(rightPoint.position, 0.15f);

        Gizmos.DrawLine(leftPoint.position, rightPoint.position);
    }
}