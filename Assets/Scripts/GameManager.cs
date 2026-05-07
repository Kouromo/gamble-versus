using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Le nom du héros choisi par le joueur dans le menu
    public string selectedHeroName = "Lopunny"; // Valeur par défaut au cas où

    public bool isVictory = false;

    public void LoadEndGameScene(bool won)
    {
        isVictory = won;
        SceneManager.LoadScene("EndGame"); // Nom de la scène de fin (à créer)
    }

    private void Awake()
    {
        // Pattern Singleton avec persistance entre les scènes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedHero(string heroName)
    {
        selectedHeroName = heroName;
        Debug.Log($"[GameManager] Héros sélectionné : {selectedHeroName}");
    }

    public void StartGame()
    {
        // Charge la scène de jeu (assure-toi qu'elle s'appelle bien "Main" ou change ce nom)
        SceneManager.LoadScene("Main");
    }
}
