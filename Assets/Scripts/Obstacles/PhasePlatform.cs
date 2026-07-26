using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class PhasePlatform : MonoBehaviour
{
    [Header("Materials")]
    public Material solidMaterial;
    public Material phaseMaterial;

    [Header("Timing")]
    public float solidTime = 1f;
    public float phaseTime = 1f;

    private Collider platformCollider;
    private Renderer platformRenderer;

    private bool isSolid = true;

    void Awake()
    {
        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<Renderer>();
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
        isSolid = solid;

        platformCollider.enabled = solid;

        if (platformRenderer != null)
        {
            platformRenderer.material = solid ? solidMaterial : phaseMaterial;
        }
    }
}