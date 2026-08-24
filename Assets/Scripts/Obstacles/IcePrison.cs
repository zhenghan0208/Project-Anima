using UnityEngine;
using TMPro;

public class IcePrison : MonoBehaviour
{
    [Header("Health")]
    public int requiredHits = 3;
    private int currentHits;

    [Header("Player")]
    public PlayerController playerController;

    [Header("UI")]
    public GameObject hintUI;
    public TMP_Text hintText;

    [Header("Ice")]
    public GameObject iceVisual;

    [Header("Audio")]
    public AudioClip breakSFX;

    private bool playerTrapped;
    private bool isBroken;

    void Start()
    {
        if (hintUI != null)
        {
            hintUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isBroken)
            return;

        if (!other.CompareTag("Player"))
            return;

        playerController =
            other.GetComponent<PlayerController>();

        if (playerController == null)
            return;

        TrapPlayer();
    }

    void TrapPlayer()
    {
        playerTrapped = true;
        currentHits = 0;

        if (hintUI != null)
        {
            hintUI.SetActive(true);
        }

        UpdateHintUI();

        // Stop player movement
        playerController.SetFrozen(true);
    }

    public void TakeDamage()
    {
        if (!playerTrapped)
            return;

        if (isBroken)
            return;

        currentHits++;

        UpdateHintUI();

        if (currentHits >= requiredHits)
        {
            BreakIce();
        }
    }

    void UpdateHintUI()
    {
        if (hintText == null)
            return;

        int remainingHits =
            requiredHits - currentHits;

        hintText.text =
            "Attack to escape!\n" +
            "Hits remaining: " +
            remainingHits;
    }

    void BreakIce()
    {
        isBroken = true;
        playerTrapped = false;

        if (hintUI != null)
        {
            hintUI.SetActive(false);
        }

        if (playerController != null)
        {
            playerController.SetFrozen(false);
        }

        PlayBreakSFX();

        if (iceVisual != null)
        {
            Destroy(iceVisual);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void PlayBreakSFX()
    {
        if (AudioManager.Instance != null &&
            breakSFX != null)
        {
            AudioManager.Instance.PlaySFX(breakSFX);
        }
    }
}