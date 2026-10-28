using System.Collections;
using UnityEngine;

public class TriggeredPlatform : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPosition;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Audio")]
    public AudioClip moveSFX;

    private Vector3 startPosition;
    private Coroutine moveCoroutine;

    void Start()
    {
        startPosition = transform.position;
    }

    public void Activate()
    {
        if (targetPosition == null)
            return;

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        PlayMoveSFX();

        moveCoroutine = StartCoroutine(
            MoveToPosition(targetPosition.position)
        );
    }

    public void Deactivate()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        PlayMoveSFX();

        moveCoroutine = StartCoroutine(
            MoveToPosition(startPosition)
        );
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;

        moveCoroutine = null;
    }

    void PlayMoveSFX()
    {
        if (AudioManager.Instance != null &&
            moveSFX != null)
        {
            AudioManager.Instance.PlayEnvironmentSFX(
                moveSFX,
                transform.position
            );
        }
    }
}