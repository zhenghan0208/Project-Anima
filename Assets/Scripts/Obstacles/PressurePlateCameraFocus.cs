using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PressurePlateCameraFocus : MonoBehaviour
{
    [Header("Cinemachine")]
    public CinemachineCamera cinemachineCamera;

    [Header("Camera Targets")]
    public Transform player;
    public Transform triggeredPlatform;
    public Transform icicle;

    [Header("Timing")]
    public float platformStayTime = 1f;
    public float icicleStayTime = 1f;

    private bool hasTriggered = false;
    private bool isRunning = false;

    void Start()
    {
        // Automatically find Cinemachine Camera
        if (cinemachineCamera == null)
        {
            cinemachineCamera =
                FindFirstObjectByType<CinemachineCamera>();
        }

        // Automatically find Player
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (isRunning)
            return;

        if (!other.CompareTag("Player"))
            return;

        StartCoroutine(CameraFocusRoutine());
    }

    IEnumerator CameraFocusRoutine()
    {
        hasTriggered = true;
        isRunning = true;

        if (cinemachineCamera == null)
        {
            isRunning = false;
            yield break;
        }

        // =========================
        // Focus on Triggered Platform
        // =========================

        if (triggeredPlatform != null)
        {
            cinemachineCamera.Follow = triggeredPlatform;

            yield return new WaitForSeconds(platformStayTime);
        }

        // =========================
        // Focus on Icicle
        // =========================

        if (icicle != null)
        {
            cinemachineCamera.Follow = icicle;

            yield return new WaitForSeconds(icicleStayTime);
        }

        // =========================
        // Return to Player
        // =========================

        if (player != null)
        {
            cinemachineCamera.Follow = player;
        }

        isRunning = false;
    }
}