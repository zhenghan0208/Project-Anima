using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    [Header("Next Level")]
    public string nextSceneName;
    public float displayTime = 5f;

    [Header("Audio")]
    public AudioClip stageClearMusic;

    private bool isShowing;

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

        if (AudioManager.Instance != null && stageClearMusic != null)
        {
            AudioManager.Instance.PlayMusic(stageClearMusic);
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.StartCelebration();
        }

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