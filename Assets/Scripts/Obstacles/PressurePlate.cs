using System.Collections;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Platform")]
    public TriggeredPlatform triggeredPlatform;

    [Header("Timing")]
    public float meltTime = 3f;

    private IceBlock currentIceBlock;
    private Coroutine meltCoroutine;

    void OnTriggerEnter(Collider other)
    {
        IceBlock iceBlock = other.GetComponentInParent<IceBlock>();

        if (iceBlock == null)
            return;

        if (currentIceBlock != null)
            return;

        currentIceBlock = iceBlock;

        Activate();

        meltCoroutine = StartCoroutine(MeltRoutine());
    }

    IEnumerator MeltRoutine()
    {
        yield return new WaitForSeconds(meltTime);

        if (currentIceBlock != null)
        {
            currentIceBlock.StartMelting();
        }

        Deactivate();

        currentIceBlock = null;
    }

    void Activate()
    {
        if (triggeredPlatform != null)
        {
            triggeredPlatform.Activate();
        }
    }

    void Deactivate()
    {
        if (triggeredPlatform != null)
        {
            triggeredPlatform.Deactivate();
        }
    }
}