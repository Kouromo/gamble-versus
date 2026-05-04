using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    [Tooltip("Nom de l'objet (doit correspondre à un ID ou nom interne)")]
    public string itemName;

    [Tooltip("Nom affiché dans l'interface (ex: 'Potion de Soin')")]
    public string displayName;

    [Tooltip("Description de l'effet de l'objet")]
    public string description;

    [Tooltip("Quantité de points de vie restaurés (0 si ce n'est pas un objet de soin)")]
    public int healAmount;
}

[System.Serializable]
public class ItemDataWrapper
{
    public List<ItemData> items;
}
