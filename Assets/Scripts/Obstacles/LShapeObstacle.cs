
using System.Collections;
using UnityEngine;

public class LShapeObstacle : MonoBehaviour
{
    [System.Serializable]
    public class TriggerSide
    {
        [Tooltip("This side's attack trigger")]
        public Collider trigger;

        [Tooltip("Rotation direction when this trigger is hit: -1 or 1")]
        public int rotationDirection = 1;
    }

    [Header("Attack Triggers")]
    public TriggerSide[] triggerSides;

    [Header("Physical Colliders")]
    [Tooltip("Actual solid colliders of the pillars, NOT the trigger colliders")]
    public BoxCollider[] solidColliders;

    [Header("Rotation")]
    public float rotationDuration = 0.2f;

    [Header("Blocking")]
    public LayerMask blockingLayers;
    public float collisionPadding = 0.02f;

    [Range(2f, 15f)]
    public float checkAngleStep = 5f;

    private bool isRotating;

    private void OnTriggerEnter(Collider other)
    {
        // Trigger colliders are handled individually below.
    }

    // Call this from PlayerAttack when an attack hits a trigger.
    public void Hit(Collider hitTrigger)
    {
        if (isRotating || hitTrigger == null) return;

        foreach (TriggerSide side in triggerSides)
        {
            if (side == null || side.trigger == null) continue;

            if (side.trigger != hitTrigger) continue;

            float angle = side.rotationDirection >= 0 ? 90f : -90f;

            if (CanRotate(angle))
            {
                StartCoroutine(RotateRoutine(angle));
            }

            return;
        }
    }

    private bool CanRotate(float angle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, 0f, angle);

        int steps = Mathf.CeilToInt(90f / checkAngleStep);

        for (int i = 1; i <= steps; i++)
        {
            float t = (float)i / steps;
            Quaternion testRotation =
                Quaternion.Slerp(startRotation, targetRotation, t);

            foreach (BoxCollider box in solidColliders)
            {
                if (box == null || !box.enabled) continue;

                if (IsBlocked(box, startRotation, testRotation))
                    return false;
            }
        }

        return true;
    }

    private bool IsBlocked(
        BoxCollider box,
        Quaternion startRotation,
        Quaternion testRotation)
    {
        Vector3 currentCenter = box.transform.TransformPoint(box.center);

        Quaternion rotationDelta =
            testRotation * Quaternion.Inverse(startRotation);

        Vector3 futureCenter =
            transform.position +
            rotationDelta * (currentCenter - transform.position);

        Vector3 scale = box.transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(
            box.size * 0.5f,
            new Vector3(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.y),
                Mathf.Abs(scale.z)
            )
        );

        halfExtents += Vector3.one * collisionPadding;

        Quaternion futureRotation =
            rotationDelta * box.transform.rotation;

        Collider[] hits = Physics.OverlapBox(
            futureCenter,
            halfExtents,
            futureRotation,
            blockingLayers,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;

            // Ignore this obstacle's own colliders.
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
                continue;

            return true;
        }

        return false;
    }

    private IEnumerator RotateRoutine(float angle)
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation =
            startRotation * Quaternion.Euler(0f, 0f, angle);

        float timer = 0f;

        while (timer < rotationDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / rotationDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.rotation =
                Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }
}
