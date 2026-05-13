using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SaveData
{
    public string heroName;
    public int currentRoundCount;
    public int currentRoundId;
    public List<int> playedRoundIds = new List<int>();
    public List<InventoryItemSave> inventory = new List<InventoryItemSave>();
}

[System.Serializable]
public class InventoryItemSave
{
    public string itemName;
    public int quantity;
}

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SaveManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SaveManager");
                    _instance = go.AddComponent<SaveManager>();
                }
            }
            return _instance;
        }
    }

    private string saveFilePath;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        
        // 1. Sauvegarder le héros
        if (GameManager.Instance != null)
            data.heroName = GameManager.Instance.selectedHeroName;

        // 2. Sauvegarder la progression (Round)
        LevelGenerator generator = FindFirstObjectByType<LevelGenerator>();
        if (generator != null)
        {
            data.currentRoundCount = generator.GetCurrentRoundCount();
            data.currentRoundId = generator.GetCurrentRoundId();
            data.playedRoundIds = generator.GetPlayedRoundIds();
        }

        // 3. Sauvegarder l'inventaire
        if (InventoryManager.Instance != null)
        {
            data.inventory = InventoryManager.Instance.GetSaveData();
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"[SaveManager] Jeu sauvegardé dans : {saveFilePath}");
    }

    public SaveData LoadGame()
    {
        if (!HasSaveFile()) return null;

        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("[SaveManager] Données de sauvegarde chargées.");
        return data;
    }

    public void DeleteSave()
    {
        if (HasSaveFile())
        {
            File.Delete(saveFilePath);
        }
    }
}
