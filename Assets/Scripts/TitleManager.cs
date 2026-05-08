using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Tooltip("Le nom exact de la scène de sélection des héros")]
    public string selectionSceneName = "ClassSelection";

    [Header("UI References")]
    public UnityEngine.UI.Button loadButton;

    private void Start()
    {
        // Désactiver le bouton Charger si aucune sauvegarde n'existe
        if (loadButton != null && SaveManager.Instance != null)
        {
            loadButton.interactable = SaveManager.Instance.HasSaveFile();
        }
    }

    // À lier au bouton "Nouvelle Partie" ou "Start"
    public void OnStartButtonClicked()
    {
        // Supprimer l'ancienne sauvegarde si on commence une nouvelle partie
        if (SaveManager.Instance != null) SaveManager.Instance.DeleteSave();
        
        Debug.Log("[TitleManager] Chargement de l'écran de sélection...");
        SceneManager.LoadScene(selectionSceneName);
    }

    // À lier au bouton "Charger Partie"
    public void OnLoadButtonClicked()
    {
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSaveFile()) return;

        SaveData data = SaveManager.Instance.LoadGame();
        if (data != null && GameManager.Instance != null)
        {
            GameManager.Instance.isLoadingSave = true;
            GameManager.Instance.loadedSaveData = data;
            GameManager.Instance.selectedHeroName = data.heroName;
            
            Debug.Log("[TitleManager] Reprise de la partie sauvegardée...");
            GameManager.Instance.StartGame(); // Lance directement la scène Main
        }
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
