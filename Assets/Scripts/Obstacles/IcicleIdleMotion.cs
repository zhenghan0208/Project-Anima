using UnityEngine;

public class IcicleIdleMotion : MonoBehaviour
{
    [Header("Sway")]
    public float swayAngle = 4f;
    public float swaySpeed = 1.5f;

    [Header("Pause Between Sways")]
    public float minPause = 1f;
    public float maxPause = 3f;

    private Quaternion originalRotation;
    private float targetAngle;
    private float timer;
    private bool swaying;

    void Start()
    {
        originalRotation = transform.localRotation;

        SetNextSway();
    }

    void Update()
    {
        if (swaying)
        {
            Quaternion targetRotation =
                originalRotation * Quaternion.Euler(0f, 0f, targetAngle);

            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                swaySpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.2f)
            {
                swaying = false;
                timer = Random.Range(minPause, maxPause);
            }
        }
        else
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                SetNextSway();
            }
        }
    }

    void SetNextSway()
    {
        targetAngle = Random.value > 0.5f
            ? swayAngle
            : -swayAngle;

        swaying = true;
    }
}