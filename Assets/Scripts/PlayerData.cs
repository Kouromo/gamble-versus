using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData : EntityData
{
    [Tooltip("Description textuelle du héros pour l'écran de sélection.")]
    public string description;
    
    // Propriétés spécifiques au joueur (par exemple: inventaire, niveau, expérience)
    // à définir selon l'évolution du jeu.
}

[System.Serializable]
public class PlayerDataWrapper
{
    public List<PlayerData> heroes;
}
