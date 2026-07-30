using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text abilityText;
    private bool isShowing;

    [Header("Next Level")]
    public string nextSceneName;
    public float displayTime = 5f;

    [Header("Audio")]
    public AudioClip stageClearMusic;

    void Start()
    {
        panel.SetActive(false);
    }

    public void ShowStageClear(AbilityType ability)
    {
        if (isShowing)
            return;

        isShowing = true;

        panel.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(stageClearMusic);
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.StartCelebration();
        }

        string abilityName = "";

        switch (ability)
        {
            case AbilityType.Weapon:
                abilityName = "Weapon";
                break;

            case AbilityType.DoubleJump:
                abilityName = "Double Jump";
                break;

            case AbilityType.Dash:
                abilityName = "Dash";
                break;
        }

        abilityText.text =
            "Stage Clear!\n\n" +
            "------------------------\n\n" +
            "New Ability Unlocked!\n\n" +
            abilityName +
            "\n\n------------------------";

        StartCoroutine(StageClearRoutine());
    }

    IEnumerator StageClearRoutine()
    {
        yield return new WaitForSecondsRealtime(displayTime);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}