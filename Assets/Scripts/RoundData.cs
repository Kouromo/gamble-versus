using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DecorationData
{
    [Tooltip("Nom ou type de la décoration (ex: 'pot', 'column', 'barriere')")]
    public string decorationName;
    
    [Tooltip("ID de l'emplacement (slot) où la décoration doit apparaître dans la salle. Varie selon la taille de la salle.")]
    public int spawnSlotId;
    
    // On pourrait ajouter d'autres paramètres comme la rotation, la couleur (si applicable), etc.
}

[System.Serializable]
public class RoundData
{
    [Tooltip("ID unique du round de combat (0, 1, etc.)")]
    public int roundId;
    
    [Tooltip("Nom du type de salle physique pour retrouver l'arène (ex: 'floor', 'floorStraight')")]
    public string roomType;
    
    [Tooltip("Liste des décorations présentes dans ce round")]
    public List<DecorationData> decorations;
    
    [Tooltip("Liste des ennemis présents dans ce round")]
    public List<EnemyData> enemies;

    [Tooltip("Est-ce un round de Boss ?")]
    public bool isBoss;
}

[System.Serializable]
public class RoundDataWrapper
{
    public List<RoundData> rounds;
}
