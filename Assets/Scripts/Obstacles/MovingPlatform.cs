using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed = 2f;

    private Transform target;
    private Vector3 lastPosition;

    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        target = pointB;
        lastPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            target = target == pointA ? pointB : pointA;
        }

        PlatformVelocity = (transform.position - lastPosition) / Time.deltaTime;

        lastPosition = transform.position;
    }
}