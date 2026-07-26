using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Target")]
    public Transform cameraTransform;

    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxMultiplier = 0.5f;

    [Header("Auto Scroll")]
    public bool autoScroll;

    public float scrollSpeed = 0.2f;

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(
            deltaMovement.x * parallaxMultiplier,
            deltaMovement.y * parallaxMultiplier,
            0f);

        if (autoScroll)
        {
            transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }

        lastCameraPosition = cameraTransform.position;
    }
}