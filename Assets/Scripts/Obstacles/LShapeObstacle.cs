using System.Collections;
using UnityEngine;

public class LShapeObstacle : MonoBehaviour
{
    [Header("Child Colliders")]
    public BoxCollider colliderA;
    public BoxCollider colliderB;

    [Header("Rotation")]
    public float rotationDuration = 0.2f;

    [Header("Blocking")]
    public LayerMask blockingLayers;
    public float collisionPadding = 0.02f;

    private bool isRotating;

    private bool isVertical = true;

    private void Start()
    {
        DetectInitialOrientation();
    }

    void DetectInitialOrientation()
    {
        float z = transform.eulerAngles.z;

        z = Mathf.Repeat(z, 180f);

        if (Mathf.Abs(z) < 45f)
        {
            isVertical = true;
        }
        else
        {
            isVertical = false;
        }
    }

    /// <summary>
    /// Called by PlayerAttack.
    /// playerPosition = position of the attacking player.
    /// </summary>
    public void Hit(Vector3 playerPosition)
    {
        if (isRotating)
            return;

        float rotationAmount =
            CalculateRotationDirection(playerPosition);

        if (!CanRotate(rotationAmount))
            return;

        StartCoroutine(
            RotateRoutine(rotationAmount)
        );
    }

    float CalculateRotationDirection(
        Vector3 playerPosition)
    {
        /*
         * Convert player position into
         * L Shape local space.
         */

        Vector3 localPlayerPosition =
            transform.InverseTransformPoint(
                playerPosition
            );

        /*
         * Player is on the LEFT side.
         *
         * L rotates CLOCKWISE.
         */

        if (localPlayerPosition.x < 0f)
        {
            return -90f;
        }

        /*
         * Player is on the RIGHT side.
         *
         * L rotates COUNTER-CLOCKWISE.
         */

        return 90f;
    }

    bool CanRotate(float angle)
    {
        Quaternion targetRotation =
            transform.rotation *
            Quaternion.Euler(
                0f,
                0f,
                angle
            );

        bool oldA =
            colliderA != null &&
            colliderA.enabled;

        bool oldB =
            colliderB != null &&
            colliderB.enabled;

        if (colliderA != null)
            colliderA.enabled = false;

        if (colliderB != null)
            colliderB.enabled = false;

        bool blocked = false;

        if (colliderA != null)
        {
            if (CheckCollider(
                colliderA,
                targetRotation))
            {
                blocked = true;
            }
        }

        if (!blocked &&
            colliderB != null)
        {
            if (CheckCollider(
                colliderB,
                targetRotation))
            {
                blocked = true;
            }
        }

        if (colliderA != null)
            colliderA.enabled = oldA;

        if (colliderB != null)
            colliderB.enabled = oldB;

        return !blocked;
    }

    bool CheckCollider(
        BoxCollider boxCollider,
        Quaternion targetRootRotation)
    {
        Vector3 localCenter =
            boxCollider.center;

        Vector3 childLocalPosition =
            boxCollider.transform.localPosition;

        Vector3 offset =
            childLocalPosition +
            localCenter;

        Vector3 rotatedOffset =
            targetRootRotation *
            Quaternion.Inverse(
                transform.rotation
            ) *
            offset;

        Vector3 futureCenter =
            transform.position +
            rotatedOffset;

        Vector3 size =
            Vector3.Scale(
                boxCollider.size,
                boxCollider.transform.lossyScale
            );

        Quaternion futureRotation =
            targetRootRotation *
            boxCollider.transform.localRotation;

        Collider[] hits =
            Physics.OverlapBox(
                futureCenter,
                size * 0.5f +
                Vector3.one *
                collisionPadding,
                futureRotation,
                blockingLayers,
                QueryTriggerInteraction.Ignore
            );

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            if (hit.transform == transform)
                continue;

            if (hit.transform.IsChildOf(transform))
                continue;

            return true;
        }

        return false;
    }

    IEnumerator RotateRoutine(float angle)
    {
        isRotating = true;

        Quaternion startRotation =
            transform.rotation;

        Quaternion targetRotation =
            startRotation *
            Quaternion.Euler(
                0f,
                0f,
                angle
            );

        float timer = 0f;

        while (timer < rotationDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / rotationDuration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        transform.rotation =
            targetRotation;

        isVertical = !isVertical;

        isRotating = false;
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (colliderA == null &&
            colliderB == null)
            return;

        /*
         * Gizmo shows the next possible
         * rotation from the default LEFT-side
         * attack direction.
         */

        float previewAngle = -90f;

        Quaternion targetRotation =
            transform.rotation *
            Quaternion.Euler(
                0f,
                0f,
                previewAngle
            );

        if (colliderA != null)
        {
            DrawFutureCollider(
                colliderA,
                targetRotation
            );
        }

        if (colliderB != null)
        {
            DrawFutureCollider(
                colliderB,
                targetRotation
            );
        }
    }

    void DrawFutureCollider(
        BoxCollider boxCollider,
        Quaternion targetRootRotation)
    {
        Vector3 localCenter =
            boxCollider.center;

        Vector3 childLocalPosition =
            boxCollider.transform.localPosition;

        Vector3 offset =
            childLocalPosition +
            localCenter;

        Vector3 rotatedOffset =
            targetRootRotation *
            Quaternion.Inverse(
                transform.rotation
            ) *
            offset;

        Vector3 futureCenter =
            transform.position +
            rotatedOffset;

        Vector3 size =
            Vector3.Scale(
                boxCollider.size,
                boxCollider.transform.lossyScale
            );

        size +=
            Vector3.one *
            collisionPadding *
            2f;

        Quaternion futureRotation =
            targetRootRotation *
            boxCollider.transform.localRotation;

        Gizmos.color = Color.yellow;

        Matrix4x4 oldMatrix =
            Gizmos.matrix;

        Gizmos.matrix =
            Matrix4x4.TRS(
                futureCenter,
                futureRotation,
                Vector3.one
            );

        Gizmos.DrawWireCube(
            Vector3.zero,
            size
        );

        Gizmos.matrix =
            oldMatrix;
    }
}