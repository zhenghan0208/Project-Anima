
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ArenaManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Stage 1 - Normal Cockroaches")]
    public GameObject normalCockroachPrefab;
    public int normalCountPerSide = 3;

    [Header("Stage 2 - Big Cockroaches")]
    public GameObject bigCockroachPrefab;
    public int bigCockroachCount = 2;

    [Header("Stage 3 - Flying Cockroaches")]
    public GameObject flyingCockroachPrefab;
    public int flyingCountPerSide = 3;

    [Header("UI")]
    public TMP_Text stageText;
    public GameObject victoryPanel;

    [Header("Stage Settings")]
    public float nextStageDelay = 2f;

    [Header("Spawn Settings")]
    public float spawnInterval = 0.5f;
    public float spawnRandomOffsetX = 1.2f;

    private readonly HashSet<GameObject> activeEnemies =
        new HashSet<GameObject>();

    private int currentStage = 0;
    private bool stageStarted;
    private bool changingStage;
    private bool gameWon;

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        StartStage(1);
    }

    void StartStage(int stage)
    {
        currentStage = stage;
        stageStarted = true;
        changingStage = false;

        if (stageText != null)
            stageText.text = "Stage " + currentStage;

        switch (currentStage)
        {
            case 1:
                SpawnFromBothSides(normalCockroachPrefab, normalCountPerSide);
                break;

            case 2:
                SpawnMultiple(bigCockroachPrefab, bigCockroachCount);
                break;

            case 3:
                SpawnFromBothSides(flyingCockroachPrefab, flyingCountPerSide);
                break;

            default:
                WinGame();
                break;
        }

        CheckStageCleared();
    }

    void SpawnFromBothSides(GameObject prefab, int countPerSide)
    {
        StartCoroutine(SpawnFromBothSidesRoutine(prefab, countPerSide));
    }

    IEnumerator SpawnFromBothSidesRoutine(GameObject prefab, int countPerSide)
    {
        int total = countPerSide * 2;

        for (int i = 0; i < total; i++)
        {
            Transform spawnPoint = (i % 2 == 0)
                ? leftSpawnPoint
                : rightSpawnPoint;

            if (spawnPoint != null)
            {
                Vector3 position = spawnPoint.position;

                // Randomize the spawn position slightly along the X axis.
                position.x += Random.Range(
                    -spawnRandomOffsetX,
                    spawnRandomOffsetX
                );

                SpawnEnemy(prefab, spawnPoint, position);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMultiple(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform point = (i % 2 == 0)
                ? leftSpawnPoint
                : rightSpawnPoint;

            SpawnEnemy(prefab, point);
        }
    }

    public GameObject SpawnEnemy(GameObject prefab, Transform spawnPoint)
    {
        if (prefab == null || spawnPoint == null)
        {
            Debug.LogWarning("ArenaManager: Prefab or spawn point is missing.");
            return null;
        }

        GameObject enemy = Instantiate(
            prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        RegisterEnemy(enemy);

        return enemy;
    }

    public GameObject SpawnEnemy(GameObject prefab, Transform spawnPoint, Vector3 position)
    {
        if (prefab == null || spawnPoint == null)
            return null;

        GameObject enemy = Instantiate(
            prefab,
            position,
            spawnPoint.rotation
        );

        RegisterEnemy(enemy);

        return enemy;
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        activeEnemies.Add(enemy);

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();

        if (health != null)
            health.arenaManager = this;
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        activeEnemies.Remove(enemy);
        CheckStageCleared();
    }

    void CheckStageCleared()
    {
        if (!stageStarted || changingStage || gameWon)
            return;

        if (activeEnemies.Count > 0)
            return;

        changingStage = true;
        StartCoroutine(NextStage());
    }

    IEnumerator NextStage()
    {
        yield return new WaitForSeconds(nextStageDelay);

        if (gameWon)
            yield break;

        StartStage(currentStage + 1);
    }

    void WinGame()
    {
        stageStarted = false;
        gameWon = true;

        if (stageText != null)
            stageText.text = "Victory!";

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Debug.Log("Arena completed!");
    }
}