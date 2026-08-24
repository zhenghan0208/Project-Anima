using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IceBlock : MonoBehaviour
{
    [Header("Rigidbody")]
    public Rigidbody rb;

    [Header("Respawn")]
    public GameObject iceBlockPrefab;
    public Transform respawnPoint;

    private bool isReleased;
    private bool isMelting;

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void TakeDamage()
    {
        if (isReleased || isMelting)
            return;

        isReleased = true;

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void StartMelting()
    {
        if (isMelting)
            return;

        isMelting = true;

        if (iceBlockPrefab != null && respawnPoint != null)
        {
            Instantiate(iceBlockPrefab, respawnPoint.position, respawnPoint.rotation);
        }

        Destroy(gameObject);
    }
}