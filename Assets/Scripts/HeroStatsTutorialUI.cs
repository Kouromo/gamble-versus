using UnityEngine;
using TMPro;

public class HeroStatsTutorialUI : MonoBehaviour
{
    [Header("Panneau du Tutoriel")]
    [Tooltip("Glissez ici le GameObject qui sert de panneau de fond pour le tutoriel.")]
    public GameObject tutorialPanel;

    private void Start()
    {
        // On s'assure que le panneau est caché au démarrage
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    // Fonction à lier à un bouton "?" ou "Aide"
    public void OpenTutorial()
    {
        if (tutorialPanel != null)
        {
            AudioManager.Instance?.PlayButtonClick();
            tutorialPanel.SetActive(true);
        }
    }

    // Fonction à lier à un bouton "Fermer" (X) dans le panneau de tutoriel
    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            AudioManager.Instance?.PlayButtonClick();
            tutorialPanel.SetActive(false);
        }
    }
}
