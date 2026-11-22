
using UnityEngine;
using System.Collections;

public class WaterHazardManager : MonoBehaviour
{
    [Header("Water Outlets")]
    public WaterHazardOutlet[] outlets;

    [Header("Timing")]
    public float initialDelay = 10f;
    public float warningDuration = 5f;
    public float pourDuration = 2f;
    public float cooldownDuration = 10f;

    private void Start()
    {
        StartCoroutine(WaterHazardRoutine());
    }

    private IEnumerator WaterHazardRoutine()
    {
        while (true)
        {
            // Wait before selecting an outlet.
            yield return new WaitForSeconds(initialDelay);

            if (outlets == null || outlets.Length == 0)
                yield break;

            // Randomly select one outlet.
            int randomIndex = Random.Range(0, outlets.Length);

            WaterHazardOutlet selectedOutlet = outlets[randomIndex];

            if (selectedOutlet == null)
                continue;

            // Play warning animation.
            selectedOutlet.PlayReady();

            yield return new WaitForSeconds(warningDuration);

            // Start pouring water.
            selectedOutlet.StartPour();

            yield return new WaitForSeconds(pourDuration);

            // Stop pouring water.
            selectedOutlet.StopPour();

            // Wait before the next random outlet.
            yield return new WaitForSeconds(cooldownDuration);
        }
    }
}