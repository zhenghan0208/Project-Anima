
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ArenaManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Enemy Prefabs")]
    public GameObject normalCockroachPrefab;
    public GameObject bigCockroachPrefab;
    public GameObject flyingCockroachPrefab;

    [Header("Stage 1 - Normal Cockroaches")]
    public int normalCountPerSide = 3;

    [Header("Stage 2 - Normal + Big Cockroaches")]
    public int stage2NormalCountPerSide = 2;
    public int bigCockroachCount = 2;

    [Header("Stage 3 - All Cockroaches")]
    public int stage3NormalCountPerSide = 2;
    public int stage3BigCockroachCount = 2;
    public int flyingCountPerSide = 3;

    [Header("UI")]
    public TMP_Text stageText;
    public TMP_Text enemyCountText;
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

        UpdateEnemyCountUI();

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
                StartCoroutine(SpawnFromBothSidesRoutine(
                    normalCockroachPrefab,
                    normalCountPerSide
                ));
                break;

            case 2:
                StartCoroutine(SpawnStage2Routine());
                break;

            case 3:
                StartCoroutine(SpawnStage3Routine());
                break;

            default:
                WinGame();
                break;
        }

        UpdateEnemyCountUI();
        CheckStageCleared();
    }

    IEnumerator SpawnStage2Routine()
    {
        yield return StartCoroutine(
            SpawnFromBothSidesRoutine(
                normalCockroachPrefab,
                stage2NormalCountPerSide
            )
        );

        yield return StartCoroutine(
            SpawnMultipleRoutine(
                bigCockroachPrefab,
                bigCockroachCount
            )
        );
    }

    IEnumerator SpawnStage3Routine()
    {
        yield return StartCoroutine(
            SpawnFromBothSidesRoutine(
                normalCockroachPrefab,
                stage3NormalCountPerSide
            )
        );

        yield return StartCoroutine(
            SpawnMultipleRoutine(
                bigCockroachPrefab,
                stage3BigCockroachCount
            )
        );

        yield return StartCoroutine(
            SpawnFromBothSidesRoutine(
                flyingCockroachPrefab,
                flyingCountPerSide
            )
        );
    }

    IEnumerator SpawnFromBothSidesRoutine(
        GameObject prefab,
        int countPerSide
    )
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

                position.x += Random.Range(
                    -spawnRandomOffsetX,
                    spawnRandomOffsetX
                );

                SpawnEnemy(prefab, spawnPoint, position);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnMultipleRoutine(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform point = (i % 2 == 0)
                ? leftSpawnPoint
                : rightSpawnPoint;

            SpawnEnemy(prefab, point);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public GameObject SpawnEnemy(GameObject prefab, Transform spawnPoint)
    {
        if (prefab == null || spawnPoint == null)
        {
            Debug.LogWarning(
                "ArenaManager: Prefab or spawn point is missing."
            );
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

    public GameObject SpawnEnemy(
        GameObject prefab,
        Transform spawnPoint,
        Vector3 position
    )
    {
        if (prefab == null || spawnPoint == null)
        {
            Debug.LogWarning(
                "ArenaManager: Prefab or spawn point is missing."
            );
            return null;
        }

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

        UpdateEnemyCountUI();
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        activeEnemies.Remove(enemy);

        UpdateEnemyCountUI();

        CheckStageCleared();
    }

    void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text =
                "Enemies: " + activeEnemies.Count;
        }
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

        if (enemyCountText != null)
            enemyCountText.text = "Enemies: 0";

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Debug.Log("Arena completed!");
    }
}