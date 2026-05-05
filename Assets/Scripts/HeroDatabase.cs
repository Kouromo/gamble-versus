using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "HeroDatabase", menuName = "RollFight/Hero Database")]
public class HeroDatabase : ScriptableObject
{
    [System.Serializable]
    public struct HeroMapping
    {
        public string heroName; // Doit correspondre au "entityName" dans heroes.json
        public GameObject prefab;
    }

    public List<HeroMapping> heroesList;

    public GameObject GetPrefab(string name)
    {
        foreach (var hero in heroesList)
        {
            if (hero.heroName == name)
            {
                if (hero.prefab == null)
                {
                    Debug.LogWarning($"Le prefab est vide pour le héros {name}.");
                }
                return hero.prefab;
            }
        }
        Debug.LogError($"Le héros '{name}' n'existe pas dans la base de données.");
        return null;
    }
}
