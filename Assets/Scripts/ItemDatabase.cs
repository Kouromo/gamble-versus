using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "RollFight/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public struct ItemMapping
    {
        [Tooltip("Doit correspondre exactement au 'itemName' dans le JSON (ex: 'potion_health')")]
        public string itemName; 
        
        [Tooltip("Le modèle 3D de l'objet à faire apparaître dans la scène")]
        public GameObject prefab;
    }

    public List<ItemMapping> itemsList;

    public GameObject GetPrefab(string name)
    {
        foreach (var item in itemsList)
        {
            if (item.itemName == name)
                return item.prefab;
        }
        Debug.LogWarning($"Aucun prefab 3D trouvé pour l'objet : {name}");
        return null;
    }
}
