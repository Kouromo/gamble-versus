using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyData : EntityData
{
    [Header("Spawn Configuration")]
    [Tooltip("Emplacement (slot) dans la salle (0, 1, ou 2)")]
    public int spawnSlotId;

    [Header("Visual Configuration")]
    [Tooltip("Index de la couleur de l'ennemi (0, 1, 2, 3...) car chaque ennemi a ses propres variantes.")]
    public int colorIndex;
}

// Wrapper nécessaire pour que JsonUtility puisse désérialiser une liste (ou un tableau) JSON
[System.Serializable]
public class EnemyDataWrapper
{
    public List<EnemyData> enemies;
}
