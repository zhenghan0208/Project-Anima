using UnityEngine;

public class TopDownAnimator : MonoBehaviour
{
    private Animator animator;
    private TopDownPlayerController controller;

    void Awake()
    {
        animator = GetComponent<Animator>();

        controller = GetComponentInParent<TopDownPlayerController>();
    }

    void Update()
    {
        animator.SetFloat(
            "Speed",
            controller.CurrentSpeed);
    }
}