using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene")]
    public string lobbySceneName = "Lobby";

    [Header("UI")]
    public GameObject settingsPanel;

    void Start()
    {
        Time.timeScale = 1f;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(lobbySceneName);
    }

    public void Continue()
    {
        // TODO
        // Load Save Data

        SceneManager.LoadScene(lobbySceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}