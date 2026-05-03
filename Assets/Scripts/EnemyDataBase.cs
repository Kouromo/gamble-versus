using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "RollFight/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    [System.Serializable]
    public struct EnemyMapping
    {
        public string enemyName; // Doit correspondre exactement au JSON (ex: "Bunny")
        public GameObject prefab;
    }

    public List<EnemyMapping> enemiesList;

    // Fonction de recherche du prefab
    public GameObject GetPrefab(string name)
    {
        foreach (var enemy in enemiesList)
        {
            if (enemy.enemyName == name)
                return enemy.prefab;
        }
        Debug.LogError($"Aucun prefab trouvé pour l'ennemi : {name}");
        return null;
    }
}
