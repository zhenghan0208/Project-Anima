using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public enum AbilityType
    {
        Weapon,
        DoubleJump,
        Dash
    }

    [Header("Ability")]
    public AbilityType abilityType;

    [Header("UI")]
    public AbilityPopupUI popupUI;

    [TextArea]
    public string popupMessage;

    [Header("Visual")]
    public GameObject pickupModel;

    [Header("Floating")]
    public float rotateSpeed = 50f;
    public float floatHeight = 0.3f;
    public float floatSpeed = 1.5f;

    private Vector3 startPosition;

    // TODO
    // AudioSource
    // Pickup Sound

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

        if (popupUI != null)
        {
            popupUI.ShowPopup(popupMessage);
        }

        // TODO
        // Play Pickup Sound

        if (pickupModel != null)
        {
            pickupModel.SetActive(true);
        }

        Destroy(gameObject);
    }
}