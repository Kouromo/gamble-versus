using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "RollFight/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    [System.Serializable]
    public struct EnemyMapping
    {
        public string enemyName; // Doit correspondre exactement au "entityName" dans le JSON
        
        [Tooltip("Tableau des prefabs pour cet ennemi. L'index 0 correspond au colorIndex 0, l'index 1 au colorIndex 1, etc.")]
        public GameObject[] coloredPrefabs;
    }

    public List<EnemyMapping> enemiesList;

    // Fonction de recherche du prefab spécifique à une couleur
    public GameObject GetPrefab(string name, int colorIndex)
    {
        foreach (var enemy in enemiesList)
        {
            if (enemy.enemyName == name)
            {
                if (enemy.coloredPrefabs == null || enemy.coloredPrefabs.Length == 0)
                {
                    Debug.LogError($"L'ennemi {name} n'a aucun prefab assigné dans la base de données.");
                    return null;
                }

                // Vérifie si l'index demandé existe, sinon on retourne le premier par défaut pour éviter un crash
                if (colorIndex >= 0 && colorIndex < enemy.coloredPrefabs.Length)
                {
                    if (enemy.coloredPrefabs[colorIndex] == null)
                    {
                        Debug.LogWarning($"Le prefab à l'index {colorIndex} est vide pour l'ennemi {name}. Chargement du premier prefab.");
                        return enemy.coloredPrefabs[0];
                    }
                    return enemy.coloredPrefabs[colorIndex];
                }
                else
                {
                    Debug.LogWarning($"L'index de couleur {colorIndex} est hors limites pour l'ennemi {name}. Chargement de la couleur par défaut (index 0).");
                    return enemy.coloredPrefabs[0];
                }
            }
        }
        Debug.LogError($"L'ennemi '{name}' n'existe pas dans la base de données.");
        return null;
    }
}
