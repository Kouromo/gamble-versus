using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // Dictionnaire pour stocker les objets et leur quantité
    // Clé: itemName (du JSON), Valeur: Quantité
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // On donne quelques potions au démarrage pour le test
        AddItem("potion", 3);
    }

    public void AddItem(string itemName, int quantity = 1)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += quantity;
        }
        else
        {
            inventory[itemName] = quantity;
        }
        Debug.Log($"[Inventory] Ajouté {quantity} x {itemName}. Total: {inventory[itemName]}");
    }

    public bool HasItem(string itemName)
    {
        return inventory.ContainsKey(itemName) && inventory[itemName] > 0;
    }

    public void UseItem(string itemName, CombatEntity target)
    {
        if (!HasItem(itemName))
        {
            CombatLogUI.Instance?.Log($"Inventaire : Pas de {itemName} disponible !");
            return;
        }

        ItemData data = DataManager.Instance.Items.Find(i => i.itemName == itemName);
        if (data == null)
        {
            Debug.LogError($"[Inventory] Données introuvables pour l'objet : {itemName}");
            return;
        }

        // Appliquer l'effet (Soin pour l'instant)
        if (data.healAmount > 0)
        {
            target.Heal(data.healAmount);
            inventory[itemName]--;
            CombatLogUI.Instance?.Log($"{target.baseData.entityName} utilise {data.displayName} et récupère {data.healAmount} PV ! (Restant: {inventory[itemName]})");
        }
    }

    public int GetItemCount(string itemName)
    {
        return inventory.ContainsKey(itemName) ? inventory[itemName] : 0;
    }
}
