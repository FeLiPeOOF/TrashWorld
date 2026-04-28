using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[PauseMenu] pausePanel is not assigned in the Inspector!");
        }
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pausePanel == null)
        {
            Debug.LogError("[PauseMenu] Cannot toggle pause: pausePanel is not assigned in the Inspector!");
            return;
        }

        isPaused = !isPaused;
        float startTime = Time.realtimeSinceStartup;
        Time.timeScale = isPaused ? 0f : 1f;
        pausePanel.SetActive(isPaused);
        float elapsed = Time.realtimeSinceStartup - startTime;
        Debug.Log($"[PauseMenu] TogglePause took {elapsed * 1000f:F2}ms - Time.timeScale = {Time.timeScale}");
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
