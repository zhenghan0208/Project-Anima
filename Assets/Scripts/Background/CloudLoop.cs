using UnityEngine;

public class CloudLoop : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0.2f;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Offset")]
    public float spawnOffset = 2f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        float cameraLeft = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;

        float cameraRight = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;

        float cloudWidth = spriteRenderer.bounds.size.x;

        if (transform.position.x + cloudWidth * 0.5f < cameraLeft)
        {
            Vector3 pos = transform.position;

            pos.x = cameraRight + cloudWidth + spawnOffset;

            transform.position = pos;
        }
    }
}