using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    // Le nom du héros choisi par le joueur dans le menu
    public string selectedHeroName = "Lopunny"; // Valeur par défaut au cas où

    public bool isVictory = false;

    // Données de chargement
    [HideInInspector] public bool isLoadingSave = false;
    [HideInInspector] public SaveData loadedSaveData;

    public void LoadEndGameScene(bool won)
    {
        isVictory = won;
        // Supprimer la sauvegarde si on gagne ou on perd (Roguelike style)
        if (SaveManager.Instance != null) SaveManager.Instance.DeleteSave();
        SceneManager.LoadScene("EndGame"); // Nom de la scène de fin (à créer)
    }

    private void Awake()
    {
        // Pattern Singleton avec persistance entre les scènes
        if (_instance == null)
        {
            _instance = this;
            transform.SetParent(null); // Obligatoire pour DontDestroyOnLoad
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
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
        AudioManager.Instance?.PlayCombatMusic();
        
        // Charge la scène de jeu (assure-toi qu'elle s'appelle bien "Main" ou change ce nom)
        SceneManager.LoadScene("Main");
    }
}
