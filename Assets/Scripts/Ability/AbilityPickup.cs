using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    [Header("Ability")]
    public AbilityType abilityType;

    [Header("Visual")]
    public GameObject pickupModel;

    [Header("Floating")]
    public float rotateSpeed = 50f;
    public float floatHeight = 0.3f;
    public float floatSpeed = 1.5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerAbility playerAbility = other.GetComponent<PlayerAbility>();

        if (playerAbility == null)
            return;

        switch (abilityType)
        {
            case AbilityType.Weapon:
                playerAbility.UnlockWeapon();
                break;

            case AbilityType.DoubleJump:
                playerAbility.UnlockDoubleJump();
                break;

            case AbilityType.Dash:
                playerAbility.UnlockDash();
                break;
        }

        PlayPickUpSFX();

        StageClearUI stageClearUI = FindFirstObjectByType<StageClearUI>();

        if (stageClearUI != null)
        {
            stageClearUI.ShowStageClear(abilityType);
        }

        gameObject.SetActive(false);
    }

    void PlayPickUpSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.pickUpSFX);
        }
    }
}