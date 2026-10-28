using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [Header("Abilities")]
    public bool hasWeapon;
    public bool hasDoubleJump;
    public bool hasDash;

    public void UnlockWeapon()
    {
        if (hasWeapon)
            return;

        hasWeapon = true;
    }

    public void UnlockDoubleJump()
    {
        if (hasDoubleJump)
            return;

        hasDoubleJump = true;
    }

    public void UnlockDash()
    {
        if (hasDash)
            return;

        hasDash = true;
    }
}