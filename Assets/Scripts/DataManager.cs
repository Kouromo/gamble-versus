using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<DataManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DataManager");
                    _instance = go.AddComponent<DataManager>();
                }
            }
            return _instance;
        }
    }

    public List<RoundData> Rounds { get; private set; } = new List<RoundData>();
    public List<PlayerData> Heroes { get; private set; } = new List<PlayerData>();
    public List<ItemData> Items { get; private set; } = new List<ItemData>();
    public GameConfig Config { get; private set; } = new GameConfig();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllData()
    {
        LoadConfig();
        LoadRounds();
        LoadHeroes();
        LoadItems();
    }

    private void LoadConfig()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "config.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Config = JsonUtility.FromJson<GameConfig>(json);
            Debug.Log($"[DataManager] Config chargée. Difficulty HP Multiplier: {Config.enemyHealthMultiplier}");
        }
        else
        {
            Config = new GameConfig();
            Debug.Log("[DataManager] Fichier config.json introuvable, utilisation de la difficulté par défaut.");
        }
    }

    private void LoadRounds()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "rounds.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            RoundDataWrapper wrapper = JsonUtility.FromJson<RoundDataWrapper>(json);
            if (wrapper != null) 
            {
                Rounds = wrapper.rounds;
                Debug.Log($"[DataManager] {Rounds.Count} rounds chargés.");
            }
        }
        else
        {
            Debug.LogError($"[DataManager] Fichier introuvable : {filePath}");
        }
    }

    private void LoadHeroes()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "heroes.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerDataWrapper wrapper = JsonUtility.FromJson<PlayerDataWrapper>(json);
            if (wrapper != null) 
            {
                Heroes = wrapper.heroes;
                Debug.Log($"[DataManager] {Heroes.Count} héros chargés.");
            }
        }
        else
        {
            Debug.LogError($"[DataManager] Fichier introuvable : {filePath}");
        }
    }

    private void LoadItems()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "items.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>(json);
            if (wrapper != null) 
            {
                Items = wrapper.items;
                Debug.Log($"[DataManager] {Items.Count} objets chargés.");
            }
        }
        else
        {
            Debug.LogWarning($"[DataManager] Fichier introuvable : {filePath} (Ignoré si non créé)");
        }
    }

    public RoundData GetRoundById(int id)
    {
        return Rounds.Find(r => r.roundId == id);
    }

    public PlayerData GetHeroByName(string name)
    {
        return Heroes.Find(h => h.entityName == name);
    }
}
