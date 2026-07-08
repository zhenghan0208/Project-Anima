using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    public void CheckGround()
    {
        IsGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer);
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