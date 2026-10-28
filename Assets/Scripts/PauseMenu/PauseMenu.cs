using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public enum ExitType
    {
        QuitGame,
        LoadScene
    }

    [Header("UI")]
    public GameObject pausePanel;

    [Header("Exit")]
    public ExitType exitType = ExitType.LoadScene;
    public string exitSceneName;

    private bool isPaused;

    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;

        pausePanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;

        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void Exit()
    {
        Time.timeScale = 1f;

        if (exitType == ExitType.QuitGame)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            SceneManager.LoadScene(exitSceneName);
        }
    }
}