using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Tooltip("Le nom exact de la scène de sélection des héros")]
    public string selectionSceneName = "ClassSelection";

    // À lier au bouton "Nouvelle Partie" ou "Start"
    public void OnStartButtonClicked()
    {
        Debug.Log("[TitleManager] Chargement de l'écran de sélection...");
        SceneManager.LoadScene(selectionSceneName);
    }

    // À lier à un bouton "Quitter" (Optionnel)
    public void OnQuitButtonClicked()
    {
        Debug.Log("[TitleManager] Fermeture du jeu.");
        Application.Quit();
        
        // Si on est dans l'éditeur Unity, on arrête la lecture pour tester que le bouton marche
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
