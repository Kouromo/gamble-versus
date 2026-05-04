using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public List<RoundData> Rounds { get; private set; } = new List<RoundData>();
    public List<PlayerData> Heroes { get; private set; } = new List<PlayerData>();
    public List<ItemData> Items { get; private set; } = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllData()
    {
        LoadRounds();
        LoadHeroes();
        LoadItems();
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
