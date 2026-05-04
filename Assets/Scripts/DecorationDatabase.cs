using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DecorationDatabase", menuName = "RollFight/Decoration Database")]
public class DecorationDatabase : ScriptableObject
{
    [System.Serializable]
    public struct DecorationMapping
    {
        [Tooltip("Doit correspondre exactement au 'decorationName' dans le JSON (ex: 'pot', 'Monstera')")]
        public string decorationName; 
        public GameObject prefab;
    }

    public List<DecorationMapping> decorationsList;

    public GameObject GetPrefab(string name)
    {
        foreach (var dec in decorationsList)
        {
            if (dec.decorationName == name)
                return dec.prefab;
        }
        Debug.LogError($"Aucun prefab trouvé pour la décoration : {name}");
        return null;
    }
}
