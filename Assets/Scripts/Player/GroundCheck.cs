using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    public bool IsGrounded { get; private set; }
    public MovingPlatform CurrentPlatform { get; private set; }

    public void CheckGround()
    {
        IsGrounded = false;
        CurrentPlatform = null;

        Collider[] hits = Physics.OverlapSphere(
            groundCheck.position,
            groundRadius,
            groundLayer);

        foreach (Collider hit in hits)
        {
            IsGrounded = true;

            CurrentPlatform = hit.GetComponent<MovingPlatform>();

            break;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundRadius);
    }
}