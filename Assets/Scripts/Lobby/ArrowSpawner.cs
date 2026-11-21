
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public enum ArrowDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    [System.Serializable]
    public class ArrowLane
    {
        public ArrowDirection direction;
        public RectTransform lane;
        public RectTransform spawnPoint;
        public Sprite arrowSprite;

        [HideInInspector]
        public FallingArrow lastArrow;
    }

    [Header("Arrow Prefab")]
    public FallingArrow arrowPrefab;

    [Header("Lanes")]
    public ArrowLane[] lanes = new ArrowLane[4];

    [Header("Spawn Settings")]
    public bool randomSequence = false;
    public float spawnInterval = 1.2f;
    public float fallSpeed = 300f;
    public float minSpawnSpacing = 120f;
    public int totalArrows = 20;

    [Header("Sequence")]
    public ArrowDirection[] sequence =
    {
        ArrowDirection.Up,
        ArrowDirection.Right,
        ArrowDirection.Down,
        ArrowDirection.Left,
        ArrowDirection.Up,
        ArrowDirection.Down,
        ArrowDirection.Right,
        ArrowDirection.Left
    };

    [Header("Destroy Settings")]
    public float bottomPadding = 50f;

    [Header("Game Manager")]
    public RhythmGameManager gameManager;

    private bool isSpawning;
    private Coroutine spawnCoroutine;

    void Start()
    {
        StartSequence();
    }

    public void StartSequence()
    {
        if (isSpawning)
            return;

        if (arrowPrefab == null || lanes == null || lanes.Length == 0)
        {
            Debug.LogWarning("ArrowSpawner: Please assign Arrow Prefab and Lanes.");
            return;
        }

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSequence()
    {
        isSpawning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < totalArrows && isSpawning; i++)
        {
            ArrowDirection direction = randomSequence
                ? GetRandomDirection()
                : sequence[i % sequence.Length];

            ArrowLane selectedLane = GetLane(direction);

            if (selectedLane == null ||
                selectedLane.lane == null ||
                selectedLane.spawnPoint == null)
            {
                Debug.LogWarning("ArrowSpawner: Lane reference missing for " + direction);
                continue;
            }

            // 确保同一条 Lane 的箭头不会太靠近
            while (selectedLane.lastArrow != null &&
                   selectedLane.lastArrow.CurrentY >
                   selectedLane.spawnPoint.anchoredPosition.y - minSpawnSpacing)
            {
                if (!isSpawning)
                    yield break;

                yield return null;
            }

            SpawnArrow(selectedLane);

            yield return new WaitForSeconds(spawnInterval);
        }

        if (gameManager != null)
            gameManager.FinishSpawning();

        isSpawning = false;
        spawnCoroutine = null;

        Debug.Log("Arrow sequence completed!");
    }

    void SpawnArrow(ArrowLane lane)
    {
        FallingArrow newArrow = Instantiate(
            arrowPrefab,
            lane.lane,
            false
        );

        RectTransform arrowRect = newArrow.GetComponent<RectTransform>();

        arrowRect.anchoredPosition = lane.spawnPoint.anchoredPosition;
        arrowRect.localRotation = Quaternion.identity;
        arrowRect.localScale = Vector3.one;

        float bottomY = -lane.lane.rect.height / 2f - bottomPadding;

        newArrow.Initialize(
            fallSpeed,
            bottomY,
            lane.arrowSprite,
            gameManager
        );

        if (gameManager != null)
            gameManager.RegisterSpawn();

        lane.lastArrow = newArrow;
    }

    ArrowDirection GetRandomDirection()
    {
        return (ArrowDirection)Random.Range(0, 4);
    }

    ArrowLane GetLane(ArrowDirection direction)
    {
        foreach (ArrowLane lane in lanes)
        {
            if (lane.direction == direction)
                return lane;
        }

        return null;
    }

    void OnDisable()
    {
        StopSequence();
    }
}