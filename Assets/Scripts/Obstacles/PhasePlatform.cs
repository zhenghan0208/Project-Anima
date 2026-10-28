using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class PhasePlatform : MonoBehaviour
{
    [Header("Timing")]
    public float solidTime = 1f;
    public float phaseTime = 1f;

    private Collider platformCollider;
    private Animator animator;

    void Awake()
    {
        platformCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        SetSolid(true);
        StartCoroutine(PhaseRoutine());
    }

    IEnumerator PhaseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(solidTime);

            SetSolid(false);

            yield return new WaitForSeconds(phaseTime);

            SetSolid(true);
        }
    }

    void SetSolid(bool solid)
    {
        platformCollider.enabled = solid;

        animator.SetBool("Solid", solid);
    }
}