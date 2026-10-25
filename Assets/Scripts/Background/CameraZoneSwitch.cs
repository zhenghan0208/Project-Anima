
using UnityEngine;
using Unity.Cinemachine;

public class CameraZoneSwitch : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineCamera cameraZone1;
    public CinemachineCamera cameraZone2;

    [Header("This Trigger Activates")]
    public int activeZone = 1;

    [Header("Priority")]
    public int activePriority = 20;
    public int inactivePriority = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (activeZone == 1)
        {
            cameraZone1.Priority = activePriority;
            cameraZone2.Priority = inactivePriority;
        }
        else if (activeZone == 2)
        {
            cameraZone2.Priority = activePriority;
            cameraZone1.Priority = inactivePriority;
        }
    }
}
