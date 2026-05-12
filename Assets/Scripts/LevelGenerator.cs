using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Bases de Données (ScriptableObjects)")]
    [Tooltip("Glissez ici le fichier HeroDatabase de votre dossier Assets")]
    public HeroDatabase heroDatabase;
    [Tooltip("Glissez ici le fichier EnemyDatabase de votre dossier Assets")]
    public EnemyDatabase enemyDatabase;
    [Tooltip("Glissez ici le fichier DecorationDatabase de votre dossier Assets")]
    public DecorationDatabase decorationDatabase;

    [Header("Paramètres de la Caméra")]
    public Camera mainCamera;

    // Structure pour définir une "Arène" physique (Salle statique) dans l'éditeur
    [System.Serializable]
    public struct Arena
    {
        [Tooltip("Le type de l'arène (doit correspondre au 'roomType' du JSON, ex: 'floor', 'floorStraight')")]
        public string arenaType; 
        
        public Transform cameraPosition; // L'endroit où placer la caméra pour voir cette salle
        
        [Tooltip("Glissez ici les GameObjects vides qui serviront de points d'apparition pour les HÉROS")]
        public Transform[] heroSlots;

        [Tooltip("Glissez ici les GameObjects vides qui serviront de points d'apparition pour les ENNEMIS")]
        public Transform[] enemySlots;
        
        [Tooltip("Glissez ici les GameObjects vides qui serviront de points d'apparition pour les DÉCORATIONS")]
        public Transform[] decorationSlots;
    }

    [Header("Configuration des Arènes dans la Scène")]
    public List<Arena> arenas;

    [Header("UI Prefabs")]
    public GameObject healthBarPrefab;
    [Tooltip("Dossier parent (optionnel) pour ranger les barres de vie dans la scène")]
    public Transform healthBarContainer;

    // Liste pour garder une trace de ce qu'on a fait apparaître (pour pouvoir les supprimer)
    private List<GameObject> instantiatedObjects = new List<GameObject>();

    // Gestion de la progression
    private int currentRoundCount = 0;
    private const int TOTAL_ROUNDS_BEFORE_BOSS = 3; // Nombre de combats normaux avant le boss
    private List<int> playedRoundIds = new List<int>();

    private void Start()
    {
        // Si on a chargé une sauvegarde, le roundCount est déjà réglé par le GameManager
        if (GameManager.Instance != null && GameManager.Instance.isLoadingSave)
        {
            currentRoundCount = GameManager.Instance.loadedSaveData.currentRoundCount;
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.LoadSaveData(GameManager.Instance.loadedSaveData.inventory);
            }
            GameManager.Instance.isLoadingSave = false; // Reset pour la prochaine fois
            
            // On charge directement le round sans incrémenter
            Invoke(nameof(LoadCurrentRoundLogic), 0.1f);
        }
        else
        {
            // Nouvelle partie : on commence au round 0
            Invoke(nameof(LoadNextRound), 0.1f);
        }
    }

    public int GetCurrentRoundCount() => currentRoundCount;

    public void LoadCurrentRoundLogic()
    {
        // Cette méthode charge le round correspondant à currentRoundCount SANS l'incrémenter
        // Utile pour le chargement d'une sauvegarde
        DetermineAndLoadRound();
    }

    // Fonction à appeler quand le joueur gagne un combat pour passer à la salle suivante
    public void LoadNextRound()
    {
        // On incrémente AVANT de charger le nouveau round
        // Nouvelle partie : 0 -> 1 (Round 1)
        // Après Round 1 : 1 -> 2 (Round 2)
        // ...
        currentRoundCount++;
        if (GameManager.Instance != null) GameManager.Instance.stats.roomsCleared++;

        // Sauvegarder automatiquement après chaque round (quand on commence le nouveau)
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }

        DetermineAndLoadRound();
    }

    private void DetermineAndLoadRound()
    {
        if (currentRoundCount <= TOTAL_ROUNDS_BEFORE_BOSS)
        {
            // 1. Piocher un round normal au hasard qui n'est pas un boss
            List<RoundData> normalRounds = DataManager.Instance.Rounds.FindAll(r => !r.isBoss);
            
            if (normalRounds.Count == 0)
            {
                Debug.LogError("[LevelGenerator] Aucun round normal trouvé dans le DataManager !");
                return;
            }

            // Essayer de ne pas répéter le même round immédiatement si possible
            RoundData selectedRound = normalRounds[Random.Range(0, normalRounds.Count)];
            
            LoadRound(selectedRound);
        }
        else if (currentRoundCount == TOTAL_ROUNDS_BEFORE_BOSS + 1)
        {
            // 2. C'est l'heure du Boss !
            RoundData bossRound = DataManager.Instance.Rounds.Find(r => r.isBoss);
            
            if (bossRound != null)
            {
                CombatLogUI.Instance?.Log("<color=red><b>ALERTE : UN BOSS APPROCHE !</b></color>");
                LoadRound(bossRound);
            }
            else
            {
                Debug.LogWarning("[LevelGenerator] Aucun round de Boss défini. Fin de partie.");
                GameManager.Instance?.LoadEndGameScene(true);
            }
        }
        else
        {
            // 3. Le boss a été vaincu
            Debug.Log("[LevelGenerator] Boss vaincu ! Victoire finale !");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadEndGameScene(true);
            }
        }
    }

    private void Update()
    {
        // Pour tester rapidement le passage au round suivant avec la touche N
        if (UnityEngine.InputSystem.Keyboard.current != null)
            if (UnityEngine.InputSystem.Keyboard.current.nKey.wasPressedThisFrame) LoadNextRound();
    }

    public void LoadRound(RoundData roundData)
    {
        if (roundData == null) return;

        // 2. Trouver l'arène physique correspondante en utilisant le "roomType"
        Arena currentArena = arenas.Find(a => a.arenaType == roundData.roomType);
        
        if (currentArena.cameraPosition == null)
        {
            Debug.LogError($"[LevelGenerator] L'arène de type '{roundData.roomType}' (demandée par le round {roundData.roundId}) n'est pas configurée dans l'inspecteur !");
            return;
        }

        // 3. Nettoyer les restes du round précédent
        ClearRoom();
        if (TurnManager.Instance != null) TurnManager.Instance.ClearCombatants();

        // 4. Déplacer la caméra
        mainCamera.transform.position = currentArena.cameraPosition.position;
        mainCamera.transform.rotation = currentArena.cameraPosition.rotation;

        // Préparer le conteneur des barres de vie s'il n'existe pas
        if (healthBarPrefab != null && healthBarContainer == null)
        {
            GameObject container = new GameObject("HealthBars_Container");
            healthBarContainer = container.transform;
        }

        // 5. Instancier les décorations
        foreach (DecorationData decData in roundData.decorations)
        {
            GameObject prefab = decorationDatabase.GetPrefab(decData.decorationName);
            if (prefab != null && decData.spawnSlotId < currentArena.decorationSlots.Length)
            {
                Transform spawnPoint = currentArena.decorationSlots[decData.spawnSlotId];
                
                // On conserve la rotation d'origine du Prefab (offset) relative au Slot
                Quaternion finalRotation = spawnPoint.rotation * prefab.transform.rotation;
                
                GameObject spawnedDec = Instantiate(prefab, spawnPoint.position, finalRotation, spawnPoint);
                instantiatedObjects.Add(spawnedDec);
            }
        }

        // 6. Instancier les ennemis
        foreach (EnemyData enemyData in roundData.enemies)
        {
            GameObject prefab = enemyDatabase.GetPrefab(enemyData.entityName, enemyData.colorIndex);
            if (prefab != null && enemyData.spawnSlotId < currentArena.enemySlots.Length)
            {
                Transform spawnPoint = currentArena.enemySlots[enemyData.spawnSlotId];
                
                // On applique une rotation de 180° sur l'axe Y pour qu'ils fassent face aux héros
                Quaternion faceRotation = spawnPoint.rotation * Quaternion.Euler(0, 180, 0);
                
                GameObject spawnedEnemy = Instantiate(prefab, spawnPoint.position, faceRotation, spawnPoint);
                instantiatedObjects.Add(spawnedEnemy);

                // Ajout du composant CombatEntity et initialisation
                CombatEntity combatEntity = spawnedEnemy.GetComponent<CombatEntity>();
                if (combatEntity == null) combatEntity = spawnedEnemy.AddComponent<CombatEntity>();

                // Instanciation de la barre de vie
                if (healthBarPrefab != null)
                {
                    GameObject hbObj = Instantiate(healthBarPrefab, healthBarContainer);
                    instantiatedObjects.Add(hbObj);
                    combatEntity.healthBar = hbObj.GetComponent<HealthBar>();
                }

                // Appliquer les multiplicateurs de difficulté
                EnemyData enemyDataCopy = new EnemyData
                {
                    entityName = enemyData.entityName,
                    maxHealth = Mathf.RoundToInt(enemyData.maxHealth * DataManager.Instance.Config.enemyHealthMultiplier),
                    rolls = enemyData.rolls,
                    mainStat = enemyData.mainStat,
                    minAttackDamage = Mathf.RoundToInt(enemyData.minAttackDamage * DataManager.Instance.Config.enemyDamageMultiplier),
                    maxAttackDamage = Mathf.RoundToInt(enemyData.maxAttackDamage * DataManager.Instance.Config.enemyDamageMultiplier),
                    dodge = enemyData.dodge,
                    speed = enemyData.speed,
                    spawnSlotId = enemyData.spawnSlotId,
                    colorIndex = enemyData.colorIndex
                };

                combatEntity.Initialize(enemyDataCopy, false);
                
                if (TurnManager.Instance != null) TurnManager.Instance.RegisterCombatant(combatEntity);
            }
        }

        // 7. Instancier le joueur (TEST)
        if (DataManager.Instance.Heroes != null && DataManager.Instance.Heroes.Count > 0)
        {
            // Récupérer le héros choisi via le GameManager (ou le premier par défaut)
            string chosenHeroName = GameManager.Instance != null ? GameManager.Instance.selectedHeroName : DataManager.Instance.Heroes[0].entityName;
            PlayerData selectedHeroData = DataManager.Instance.GetHeroByName(chosenHeroName);

            if (selectedHeroData == null)
            {
                Debug.LogWarning($"[LevelGenerator] Héros '{chosenHeroName}' introuvable dans le DataManager. Utilisation du héros par défaut.");
                selectedHeroData = DataManager.Instance.Heroes[0];
            }

            GameObject heroPrefab = heroDatabase != null ? heroDatabase.GetPrefab(selectedHeroData.entityName) : null;
            
            if (heroPrefab != null && currentArena.heroSlots != null && currentArena.heroSlots.Length > 0)
            {
                Transform spawnPoint = currentArena.heroSlots[0];
                
                // On conserve la rotation d'origine du Prefab (offset) relative au Slot
                Quaternion finalRotation = spawnPoint.rotation * heroPrefab.transform.rotation;
                
                GameObject spawnedHero = Instantiate(heroPrefab, spawnPoint.position, finalRotation, spawnPoint);
                instantiatedObjects.Add(spawnedHero);

                // Ajout du composant CombatEntity et initialisation
                CombatEntity combatEntity = spawnedHero.GetComponent<CombatEntity>();
                if (combatEntity == null) combatEntity = spawnedHero.AddComponent<CombatEntity>();

                // Instanciation de la barre de vie
                if (healthBarPrefab != null)
                {
                    GameObject hbObj = Instantiate(healthBarPrefab, healthBarContainer);
                    instantiatedObjects.Add(hbObj);
                    combatEntity.healthBar = hbObj.GetComponent<HealthBar>();
                }

                combatEntity.Initialize(selectedHeroData, true);

                // Appliquer la santé persistante du GameManager si elle a été initialisée
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.currentHeroHealth != -1)
                    {
                        combatEntity.currentHealth = GameManager.Instance.currentHeroHealth;
                        if (combatEntity.healthBar != null) combatEntity.healthBar.UpdateHealth();
                    }
                    else
                    {
                        // Première fois : on initialise la santé persistante avec la santé max du héros
                        GameManager.Instance.currentHeroHealth = combatEntity.currentHealth;
                    }
                }
                
                if (TurnManager.Instance != null) TurnManager.Instance.RegisterCombatant(combatEntity);
                
                Debug.Log($"[LevelGenerator] Héros '{selectedHeroData.entityName}' instancié.");
            }
            else if (heroPrefab == null)
            {
                Debug.LogWarning("[LevelGenerator] Impossible d'instancier le héros : prefab introuvable dans HeroDatabase.");
            }
            else if (currentArena.heroSlots == null || currentArena.heroSlots.Length == 0)
            {
                Debug.LogWarning($"[LevelGenerator] Impossible d'instancier le héros : aucun heroSlot configuré pour l'arène {currentArena.arenaType}.");
            }
        }

        Debug.Log($"[LevelGenerator] Round {roundData.roundId} généré sur l'arène '{roundData.roomType}' !");
        
        // Lancer le combat
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.StartCombat();
        }
    }

    private void ClearRoom()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            if (obj != null) 
            {
                // On désactive l'objet avant de le détruire.
                // Cela empêche l'éditeur Unity de crasher s'il essayait d'afficher cet objet dans l'inspecteur au moment de sa destruction.
                obj.SetActive(false);
                Destroy(obj);
            }
        }
        instantiatedObjects.Clear();
    }
}
