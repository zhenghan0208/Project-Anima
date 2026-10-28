using UnityEngine;

public class FallingIcicleSpawner : MonoBehaviour
{
    [Header("Icicle")]
    public GameObject iciclePrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public float spawnInterval = 1f;

    private float timer;

    void Start()
    {
        timer = spawnInterval;

        SpawnIcicle();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnIcicle();

            timer = spawnInterval;
        }
    }

    void SpawnIcicle()
    {
        if (iciclePrefab == null)
            return;

        Vector3 position =
            spawnPoint != null
                ? spawnPoint.position
                : transform.position;

        Instantiate(
            iciclePrefab,
            position,
            iciclePrefab.transform.rotation
        );
    }
}