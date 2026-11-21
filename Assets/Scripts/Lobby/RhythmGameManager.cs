
using UnityEngine;
using TMPro;

public class RhythmGameManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int scorePerHit = 100;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public GameObject finalScorePanel;

    private int currentScore = 0;
    private int totalSpawned = 0;
    private int totalResolved = 0;
    private bool spawningComplete = false;
    private bool gameFinished = false;

    void Start()
    {
        currentScore = 0;

        if (scoreText != null)
            scoreText.text = "Score: 0";

        if (finalScorePanel != null)
            finalScorePanel.SetActive(false);
    }

    public void RegisterSpawn()
    {
        totalSpawned++;
    }

    public void RegisterHit()
    {
        if (gameFinished)
            return;

        currentScore += scorePerHit;
        totalResolved++;

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        UpdateScore();
        CheckGameFinished();
    }

    public void RegisterMiss()
    {
        if (gameFinished)
            return;

        totalResolved++;
        CheckGameFinished();
    }

    public void FinishSpawning()
    {
        spawningComplete = true;
        CheckGameFinished();
    }

    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    void CheckGameFinished()
    {
        if (gameFinished || !spawningComplete)
            return;

        if (totalResolved >= totalSpawned)
        {
            gameFinished = true;

            if (finalScorePanel != null)
                finalScorePanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + currentScore;
        }
    }
}