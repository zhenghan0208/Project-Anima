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

    [Header("Ice Animation")]
    public Animator animator;

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

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
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

        // Freeze player movement
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

        // Play different animation for each hit
        if (animator != null)
        {
            if (currentHits == 1)
            {
                animator.Play("ice break 1");
            }
            else if (currentHits == 2)
            {
                animator.Play("ice break 2");
            }
            else if (currentHits >= 3)
            {
                animator.Play("ice break 3");
            }
        }

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

        // Hide hint UI
        if (hintUI != null)
        {
            hintUI.SetActive(false);
        }

        // Allow player to move again
        if (playerController != null)
        {
            playerController.SetFrozen(false);
        }

        PlayBreakSFX();

        // Automatically destroy after the final animation
        float animationLength = GetAnimationLength("ice break 3");

        if (animationLength > 0f)
        {
            Destroy(gameObject, animationLength);
        }
        else
        {
            // Fallback if animation cannot be found
            Destroy(gameObject);
        }
    }

    float GetAnimationLength(string stateName)
    {
        if (animator == null)
            return 0f;

        RuntimeAnimatorController controller =
            animator.runtimeAnimatorController;

        if (controller == null)
            return 0f;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == stateName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning(
            "Animation clip not found: " + stateName
        );

        return 0f;
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