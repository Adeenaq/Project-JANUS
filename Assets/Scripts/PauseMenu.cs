using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign in Inspector

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause game time
        isPaused = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Ensure game time is normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart current scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Ensure game time is normal
        // Load Main Menu scene or quit application
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
        // Application.Quit(); // Use this for standalone builds
    }
}
