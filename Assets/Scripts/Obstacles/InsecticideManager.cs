using System.Collections;
using UnityEngine;

public class InsecticideManager : MonoBehaviour
{
    [Header("Pickup Objects")]
    public GameObject[] pickupObjects = new GameObject[3];

    [Header("Respawn Settings")]
    public float respawnDelay = 10f;

    private bool hasActivePickup;
    private bool isRespawning;

    void Start()
    {
        SpawnRandomPickup();
    }

    public void TryCollect(InsecticideAbility ability)
    {
        if (!hasActivePickup || ability == null)
            return;

        if (!ability.TryPickup())
            return;

        // Hide all pickup objects
        foreach (GameObject pickup in pickupObjects)
        {
            if (pickup != null)
                pickup.SetActive(false);
        }

        hasActivePickup = false;

        // Connect manager to player ability
        ability.SetManager(this);

        Debug.Log("Insecticide Collected!");
    }

    public void StartRespawnCooldown()
    {
        if (isRespawning)
            return;

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        SpawnRandomPickup();

        isRespawning = false;
    }

    void SpawnRandomPickup()
    {
        if (pickupObjects == null || pickupObjects.Length == 0)
        {
            Debug.LogWarning("No Insecticide Pickup Objects assigned!");
            return;
        }

        // Hide all three first
        foreach (GameObject pickup in pickupObjects)
        {
            if (pickup != null)
                pickup.SetActive(false);
        }

        // Randomly select one
        int randomIndex = Random.Range(0, pickupObjects.Length);

        if (pickupObjects[randomIndex] != null)
        {
            pickupObjects[randomIndex].SetActive(true);
            hasActivePickup = true;

            Debug.Log("Insecticide spawned at position: " + randomIndex);
        }
    }
}