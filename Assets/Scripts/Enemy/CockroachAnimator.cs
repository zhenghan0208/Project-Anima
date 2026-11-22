
using System.Collections;
using UnityEngine;

public class CockroachAnimator : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform visual;

    [Header("Animation States")]
    public string walkingState = "Walking";
    public string hurtState = "Hurt";
    public string deadState = "Dead";

    [Header("Facing")]
    public bool startsFacingRight = true;

    private bool isDead;
    private bool facingRight;

    void Awake()
    {
        if (visual == null)
        {
            if (animator != null)
                visual = animator.transform;
        }

        if (animator == null && visual != null)
            animator = visual.GetComponent<Animator>();

        facingRight = startsFacingRight;
        UpdateVisualDirection();
    }

    public void SetFacingDirection(float direction)
    {
        if (direction > 0.01f)
            facingRight = true;
        else if (direction < -0.01f)
            facingRight = false;

        UpdateVisualDirection();
    }

    private void UpdateVisualDirection()
    {
        if (visual == null)
            return;

        Vector3 scale = visual.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f);
        visual.localScale = scale;
    }

    public void PlayWalking()
    {
        if (!isDead && animator != null)
            animator.CrossFadeInFixedTime(walkingState, 0.05f);
    }

    public void PlayHurt()
    {
        if (!isDead && animator != null)
            animator.CrossFadeInFixedTime(hurtState, 0.05f);
    }

    public IEnumerator PlayDeadAndWait()
    {
        if (animator == null)
            yield break;

        isDead = true;

        int deadHash = Animator.StringToHash(deadState);

        if (!animator.HasState(0, deadHash))
            yield break;

        animator.Play(deadHash, 0, 0f);
        yield return null;

        float timeout = animator.GetCurrentAnimatorStateInfo(0).length + 1f;
        float timer = 0f;

        while (timer < timeout)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.shortNameHash == deadHash &&
                state.normalizedTime >= 1f)
                break;

            timer += Time.deltaTime;
            yield return null;
        }
    }
}