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

    // Liste pour garder une trace de ce qu'on a fait apparaître (pour pouvoir les supprimer)
    private List<GameObject> instantiatedObjects = new List<GameObject>();

    // Index du round actuellement joué
    private int currentRoundIndex = 0;

    private void Start()
    {
        // Chargeons le round actuel (0 au début) au démarrage
        Invoke(nameof(LoadCurrentRound), 0.1f);
    }

    public void LoadCurrentRound()
    {
        LoadRound(currentRoundIndex);
    }

    // Fonction à appeler quand le joueur gagne un combat pour passer à la salle suivante
    public void LoadNextRound()
    {
        currentRoundIndex++;
        
        // On vérifie s'il reste des rounds dans le DataManager
        if (DataManager.Instance.GetRoundById(currentRoundIndex) != null)
        {
            LoadRound(currentRoundIndex);
        }
        else
        {
            Debug.Log("[LevelGenerator] Plus de rounds disponibles ! Vous avez gagné !");
            // Ici on pourrait charger l'écran de victoire
        }
    }

    private void Update()
    {
        // Pour tester rapidement le passage au round suivant avec la touche N
        if (UnityEngine.InputSystem.Keyboard.current != null)
            if (UnityEngine.InputSystem.Keyboard.current.nKey.wasPressedThisFrame) LoadNextRound();
    }

    public void LoadRound(int roundId)
    {
        // 1. Récupérer les données du Round
        RoundData roundData = DataManager.Instance.GetRoundById(roundId);
        if (roundData == null)
        {
            Debug.LogError($"[LevelGenerator] Impossible de trouver les données pour le round ID {roundId}");
            return;
        }

        // 2. Trouver l'arène physique correspondante en utilisant le "roomType"
        Arena currentArena = arenas.Find(a => a.arenaType == roundData.roomType);
        
        if (currentArena.cameraPosition == null)
        {
            Debug.LogError($"[LevelGenerator] L'arène de type '{roundData.roomType}' (demandée par le round {roundId}) n'est pas configurée dans l'inspecteur !");
            return;
        }

        // 3. Nettoyer les restes du round précédent
        ClearRoom();
        if (TurnManager.Instance != null) TurnManager.Instance.ClearCombatants();

        // 4. Déplacer la caméra
        mainCamera.transform.position = currentArena.cameraPosition.position;
        mainCamera.transform.rotation = currentArena.cameraPosition.rotation;

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

                // Ajout du composant CombatEntity s'il n'est pas déjà présent et initialisation
                CombatEntity combatEntity = spawnedEnemy.GetComponent<CombatEntity>();
                if (combatEntity == null) combatEntity = spawnedEnemy.AddComponent<CombatEntity>();
                combatEntity.Initialize(enemyData, false);
                
                if (TurnManager.Instance != null) TurnManager.Instance.RegisterCombatant(combatEntity);
            }
        }

        // 7. Instancier le joueur (TEST)
        if (DataManager.Instance.Heroes != null && DataManager.Instance.Heroes.Count > 0)
        {
            // On prend le premier héros pour le test
            PlayerData testHero = DataManager.Instance.Heroes[0]; 
            GameObject heroPrefab = heroDatabase != null ? heroDatabase.GetPrefab(testHero.entityName) : null;
            
            if (heroPrefab != null && currentArena.heroSlots != null && currentArena.heroSlots.Length > 0)
            {
                Transform spawnPoint = currentArena.heroSlots[0];
                
                // On conserve la rotation d'origine du Prefab (offset) relative au Slot
                Quaternion finalRotation = spawnPoint.rotation * heroPrefab.transform.rotation;
                
                GameObject spawnedHero = Instantiate(heroPrefab, spawnPoint.position, finalRotation, spawnPoint);
                instantiatedObjects.Add(spawnedHero);

                // Ajout du composant CombatEntity s'il n'est pas déjà présent et initialisation
                CombatEntity combatEntity = spawnedHero.GetComponent<CombatEntity>();
                if (combatEntity == null) combatEntity = spawnedHero.AddComponent<CombatEntity>();
                combatEntity.Initialize(testHero, true);
                
                if (TurnManager.Instance != null) TurnManager.Instance.RegisterCombatant(combatEntity);
                
                Debug.Log($"[LevelGenerator] Héros de test '{testHero.entityName}' instancié.");
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

        Debug.Log($"[LevelGenerator] Round {roundId} généré sur l'arène '{roundData.roomType}' !");
        
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
