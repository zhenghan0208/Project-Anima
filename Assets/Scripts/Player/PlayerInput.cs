using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Input")]
    public float MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }

    void Update()
    {
        MoveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpPressed = true;
        }

        JumpHeld = Input.GetKey(KeyCode.Space);
    }

    public void ResetJump()
    {
        JumpPressed = false;
    }
}