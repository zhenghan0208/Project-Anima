using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Input")]
    public float MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }

    public bool DashPressed { get; private set; }

    void Update()
    {
        MoveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpPressed = true;
        }

        JumpHeld = Input.GetKey(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashPressed = true;
        }
    }

    public void ResetJump()
    {
        JumpPressed = false;
    }

    public void ResetDash()
    {
        DashPressed = false;
    }
}