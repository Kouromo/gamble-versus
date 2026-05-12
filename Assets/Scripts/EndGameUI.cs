using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("L'image de fond ou d'illustration qui change selon le résultat")]
    public Image resultImage;

    [Tooltip("Le composant Texte pour afficher les statistiques")]
    public TextMeshProUGUI statsText;

    [Header("Assets")]
    public Sprite victorySprite;
    public Sprite defeatSprite;

    [Header("Settings")]
    [Tooltip("Nom de la scène du menu principal (ex: 'Title' ou 'MainMenu')")]
    public string titleSceneName = "Title";

    private void Start()
    {
        // On remet la musique du menu pour l'écran de fin
        AudioManager.Instance?.PlayMenuMusic();

        // On récupère l'état de la partie depuis le GameManager
        if (GameManager.Instance != null)
        {
            bool victory = GameManager.Instance.isVictory;
            
            if (victory)
            {
                if (resultImage != null && victorySprite != null)
                {
                    resultImage.sprite = victorySprite;
                }
            }
            else
            {
                if (resultImage != null && defeatSprite != null)
                {
                    resultImage.sprite = defeatSprite;
                }
            }

            // Affichage des statistiques
            if (statsText != null)
            {
                GameStatistics stats = GameManager.Instance.stats;
                statsText.text = $"<b>STATISTIQUES DE LA PARTIE</b>\n\n" +
                                 $"Salles parcourues : {stats.roomsCleared}\n" +
                                 $"Coups portés : {stats.hitsLanded}\n" +
                                 $"Coups ratés : {stats.hitsMissed}\n";
                
                // Calcul du taux de précision
                int totalAttacks = stats.hitsLanded + stats.hitsMissed;
                if (totalAttacks > 0)
                {
                    float accuracy = (float)stats.hitsLanded / totalAttacks * 100f;
                    statsText.text += $"Précision : {accuracy:F1}%";
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
            Destroy(DataManager.Instance.gameObject);
        }

        SceneManager.LoadScene(titleSceneName);
    }
}
