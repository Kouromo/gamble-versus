using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;

    public static bool IsOptionsOpen { get; private set; } = false;

    /// <summary>
    /// Masque le menu pause et affiche les options.
    /// À lier au bouton "Options" du menu Pause.
    /// </summary>
    public void ShowOptions()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(true);
        IsOptionsOpen = true;
    }

    /// <summary>
    /// Masque les options et revient au menu pause.
    /// À lier au bouton "Retour" du menu Options.
    /// </summary>
    public void ShowPauseMenu()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        IsOptionsOpen = false;
    }
}
