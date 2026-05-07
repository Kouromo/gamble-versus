using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public List<CombatEntity> combatants = new List<CombatEntity>();
    private int currentTurnIndex = 0;

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

    public void ClearCombatants()
    {
        combatants.Clear();
        currentTurnIndex = 0;
    }

    public void RegisterCombatant(CombatEntity entity)
    {
        if (!combatants.Contains(entity))
        {
            combatants.Add(entity);
        }
    }

    public void StartCombat()
    {
        if (combatants.Count == 0)
        {
            Debug.LogWarning("[TurnManager] Aucun combattant enregistré pour commencer le combat.");
            return;
        }

        // Trier les combattants par vitesse décroissante
        combatants = combatants.OrderByDescending(c => c.baseData.speed).ToList();
        
        CombatLogUI.Instance?.Clear();
        CombatLogUI.Instance?.Log("Début du combat !");

        currentTurnIndex = 0;
        StartTurn();
    }

    private void StartTurn()
    {
        // On s'assure que le combattant actuel est toujours en vie, sinon on passe au suivant
        while (currentTurnIndex < combatants.Count && combatants[currentTurnIndex].currentHealth <= 0)
        {
            currentTurnIndex++;
        }

        if (CheckWinOrLoseCondition())
        {
            return; // Fin du combat
        }

        if (currentTurnIndex >= combatants.Count)
        {
            // Tout le monde a joué, on recommence un round de combat
            currentTurnIndex = 0;
            StartTurn();
            return;
        }

        CombatEntity activeEntity = combatants[currentTurnIndex];
        CombatLogUI.Instance?.Log($"Tour de : {activeEntity.baseData.entityName}");

        if (activeEntity.isPlayer)
        {
            // C'est le tour du joueur. On attend qu'il utilise l'UI (qui appellera PlayerAttackTarget).
            CombatLogUI.Instance?.Log("En attente de l'action du joueur...");
            if (CombatUIController.Instance != null)
            {
                CombatUIController.Instance.ShowPlayerActions();
            }
        }
        else
        {
            // C'est le tour de l'ennemi. On lui fait jouer son tour automatiquement après un léger délai.
            Invoke(nameof(ExecuteEnemyTurn), 1.5f);
        }
    }

    private void ExecuteEnemyTurn()
    {
        CombatEntity activeEnemy = combatants[currentTurnIndex];
        
        // Logique IA simple : Trouver un joueur vivant et l'attaquer
        CombatEntity target = combatants.FirstOrDefault(c => c.isPlayer && c.currentHealth > 0);
        
        if (target != null)
        {
            activeEnemy.PerformAttack(target);
        }

        NextTurn();
    }

    // Méthode à appeler depuis l'UI par un bouton d'attaque
    public void PlayerAttackTarget(CombatEntity target)
    {
        CombatEntity activePlayer = combatants[currentTurnIndex];

        if (!activePlayer.isPlayer)
        {
            Debug.LogWarning("[TurnManager] Ce n'est pas le tour du joueur !");
            return;
        }

        activePlayer.PerformAttack(target);

        NextTurn();
    }

    public void PlayerUseItem(string itemName)
    {
        CombatEntity activePlayer = combatants[currentTurnIndex];

        if (!activePlayer.isPlayer)
        {
            Debug.LogWarning("[TurnManager] Ce n'est pas le tour du joueur !");
            return;
        }

        if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemName))
        {
            InventoryManager.Instance.UseItem(itemName, activePlayer);
            NextTurn();
        }
        else
        {
            CombatLogUI.Instance?.Log("Vous n'avez pas cet objet !");
        }
    }

    private void NextTurn()
    {
        if (CheckWinOrLoseCondition()) return;

        currentTurnIndex++;
        StartTurn();
    }

    private bool CheckWinOrLoseCondition()
    {
        bool isPlayerAlive = combatants.Any(c => c.isPlayer && c.currentHealth > 0);
        bool isEnemyAlive = combatants.Any(c => !c.isPlayer && c.currentHealth > 0);

        if (!isPlayerAlive)
        {
            CombatLogUI.Instance?.Log("Défaite ! Le joueur est mort.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadEndGameScene(false);
            }
            return true;
        }

        if (!isEnemyAlive)
        {
            CombatLogUI.Instance?.Log("Victoire ! Tous les ennemis sont vaincus.");
            
            // Système de récompense aléatoire
            GiveRandomReward();

            // Passer au round suivant après un petit délai pour laisser le joueur respirer
            LevelGenerator generator = FindFirstObjectByType<LevelGenerator>();
            if (generator != null)
            {
                generator.Invoke(nameof(LevelGenerator.LoadNextRound), 3f);
            }
            
            return true;
        }

        return false;
    }

    private void GiveRandomReward()
    {
        if (DataManager.Instance == null || DataManager.Instance.Items == null || DataManager.Instance.Items.Count == 0) return;

        // On a 70% de chance de trouver un objet
        if (Random.Range(0, 100) < 70)
        {
            // Choisir un objet au hasard dans la base de données
            int randomIndex = Random.Range(0, DataManager.Instance.Items.Count);
            ItemData reward = DataManager.Instance.Items[randomIndex];

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(reward.itemName, 1);
                CombatLogUI.Instance?.Log($"<color=yellow>Butin trouvé : {reward.displayName} !</color>");
            }
        }
        else
        {
            CombatLogUI.Instance?.Log("Pas de butin sur ces ennemis...");
        }
    }
}
