using UnityEngine;

public class InsecticidePickup : MonoBehaviour
{
    [Header("Manager")]
    public InsecticideManager manager;

    private void OnTriggerEnter(Collider other)
    {
        InsecticideAbility ability =
            other.GetComponentInParent<InsecticideAbility>();

        if (ability == null)
            return;

        if (manager != null)
        {
            manager.TryCollect(ability);
        }
    }
}