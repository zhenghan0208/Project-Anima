using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Platform")]
    public TriggeredPlatform triggeredPlatform;

    [Header("Pressure Plate")]
    public float pressDistance = 0.15f;
    public float pressSpeed = 5f;

    [Header("Ice Block")]
    public float meltTime = 3f;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;

    private HashSet<Collider> objectsOnPlate =
        new HashSet<Collider>();

    private IceBlock currentIceBlock;
    private Coroutine meltCoroutine;

    private bool isPressed;

    void Start()
    {
        originalPosition = transform.localPosition;

        pressedPosition =
            originalPosition + Vector3.down * pressDistance;
    }

    void Update()
    {
        Vector3 targetPosition =
            isPressed ? pressedPosition : originalPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            pressSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        // =========================
        // Player
        // =========================

        if (other.CompareTag("Player"))
        {
            AddObject(other);
            return;
        }

        // =========================
        // Ice Block
        // =========================

        IceBlock iceBlock =
            other.GetComponentInParent<IceBlock>();

        if (iceBlock != null)
        {
            AddObject(other);

            if (currentIceBlock == null)
            {
                currentIceBlock = iceBlock;

                if (meltCoroutine != null)
                {
                    StopCoroutine(meltCoroutine);
                }

                meltCoroutine =
                    StartCoroutine(MeltRoutine());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        RemoveObject(other);

        // If this was the current ice block
        IceBlock iceBlock =
            other.GetComponentInParent<IceBlock>();

        if (iceBlock != null &&
            iceBlock == currentIceBlock)
        {
            currentIceBlock = null;

            if (meltCoroutine != null)
            {
                StopCoroutine(meltCoroutine);
                meltCoroutine = null;
            }
        }
    }

    void AddObject(Collider other)
    {
        if (objectsOnPlate.Contains(other))
            return;

        objectsOnPlate.Add(other);

        if (!isPressed)
        {
            isPressed = true;

            Activate();
        }
    }

    void RemoveObject(Collider other)
    {
        if (!objectsOnPlate.Contains(other))
            return;

        objectsOnPlate.Remove(other);

        CheckRelease();
    }

    // =========================
    // Called when IceBlock melts
    // =========================

    public void RemoveIceBlock(IceBlock iceBlock)
    {
        if (iceBlock == null)
            return;

        // Find all colliders belonging to this IceBlock
        Collider[] colliders =
            iceBlock.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            objectsOnPlate.Remove(col);
        }

        if (currentIceBlock == iceBlock)
        {
            currentIceBlock = null;
        }

        CheckRelease();
    }

    void CheckRelease()
    {
        if (objectsOnPlate.Count == 0)
        {
            isPressed = false;

            Deactivate();
        }
    }

    IEnumerator MeltRoutine()
    {
        yield return new WaitForSeconds(meltTime);

        if (currentIceBlock != null)
        {
            IceBlock iceBlock = currentIceBlock;

            // Tell the pressure plate that the ice is disappearing
            RemoveIceBlock(iceBlock);

            // Then melt the ice
            iceBlock.StartMelting();
        }

        currentIceBlock = null;
        meltCoroutine = null;
    }

    void Activate()
    {
        if (triggeredPlatform != null)
        {
            triggeredPlatform.Activate();
        }
    }

    void Deactivate()
    {
        if (triggeredPlatform != null)
        {
            triggeredPlatform.Deactivate();
        }
    }
}