using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;

    public static bool GameIsPaused = false;

    private void Awake()
    {
        // S'assurer que le menu est caché au lancement et que le temps s'écoule normalement
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Update()
    {
        // On ne gère pas Échap si le menu des options est ouvert
        if (MenuSwitcher.IsOptionsOpen) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused)
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
        AudioManager.Instance?.PlayButtonClick();
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadTitle()
    {
        AudioManager.Instance?.PlayButtonClick();
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Title"); // S'assure que la scène titre s'appelle bien "Title"
    }

    public void QuitGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
