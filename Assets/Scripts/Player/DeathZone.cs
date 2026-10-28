using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Death Height")]
    public float deathHeight = -5f;

    private PlayerHealth playerHealth;
    private bool hasTriggered;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (hasTriggered)
            return;

        if (transform.position.y < deathHeight)
        {
            hasTriggered = true;
            playerHealth.Kill();
        }
    }
}