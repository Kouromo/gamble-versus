using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Le composant Texte pour afficher 'Victoire !' ou 'Défaite...'")]
    public TextMeshProUGUI titleText;

    [Header("Settings")]
    [Tooltip("Nom de la scène du menu principal")]
    public string titleSceneName = "Title";

    private void Start()
    {
        // On récupère l'état de la partie depuis le GameManager
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isVictory)
            {
                if (titleText != null)
                {
                    titleText.text = "Victoire !";
                    titleText.color = Color.yellow;
                }
            }
            else
            {
                if (titleText != null)
                {
                    titleText.text = "Défaite...";
                    titleText.color = Color.red;
                }
            }
        }
        else
        {
            Debug.LogWarning("[EndGameUI] GameManager introuvable. Impossible de déterminer l'état de la partie.");
        }
    }

    // À lier au bouton "Retour au Menu"
    public void ReturnToTitle()
    {
        // Optionnel: nettoyer le GameManager ou DataManager pour réinitialiser la partie proprement
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
        if (DataManager.Instance != null)
        {
            // On peut détruire le DataManager s'il a besoin d'être rechargé au titre,
            // ou tu peux le garder s'il persiste bien.
            Destroy(DataManager.Instance.gameObject);
        }

        SceneManager.LoadScene(titleSceneName);
    }
}
