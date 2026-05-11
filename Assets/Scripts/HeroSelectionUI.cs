using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroSelectionUI : MonoBehaviour
{
    [Header("UI Elements - Infos")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statsText;

    [Header("UI Elements - Génération")]
    public GameObject heroButtonPrefab;
    public Transform heroButtonContainer;

    [Header("3D Preview (Optionnel)")]
    public Transform previewSpawnPoint;
    public HeroDatabase heroDatabase; // Pour charger le modèle 3D du héros

    private GameObject currentPreviewModel;
    private List<PlayerData> availableHeroes;
    private Dictionary<string, Button> heroButtons = new Dictionary<string, Button>();

    private void Start()
    {
        // On attend un tout petit peu que le DataManager ait fini de charger les JSON
        Invoke(nameof(InitializeUI), 0.2f);
    }

    private void InitializeUI()
    {
        if (DataManager.Instance == null || DataManager.Instance.Heroes.Count == 0)
        {
            Debug.LogError("[HeroSelection] DataManager introuvable ou liste de héros vide !");
            return;
        }

        availableHeroes = DataManager.Instance.Heroes;

        // Créer un bouton pour chaque héros
        foreach (PlayerData hero in availableHeroes)
        {
            GameObject btnObj = Instantiate(heroButtonPrefab, heroButtonContainer);
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = hero.entityName;

            Button btn = btnObj.GetComponent<Button>();
            heroButtons[hero.entityName] = btn;

            PlayerData capturedHero = hero; // Capture pour la lambda
            btn.onClick.AddListener(() => OnHeroSelected(capturedHero));
        }

        // Sélectionner le premier héros par défaut
        OnHeroSelected(availableHeroes[0]);
    }

    private void OnHeroSelected(PlayerData hero)
    {
        // 1. Mettre à jour les textes
        nameText.text = hero.entityName;
        descriptionText.text = hero.description;
        
        statsText.text = $"PV Max : {hero.maxHealth}\n" +
                         $"Jets (Rolls) : {hero.rolls}\n" +
                         $"Précision (Main Stat) : {hero.mainStat}%\n" +
                         $"Dégâts : {hero.minAttackDamage} - {hero.maxAttackDamage}\n" +
                         $"Esquive : {hero.dodge}%\n" +
                         $"Vitesse : {hero.speed}";

        // Mettre à jour l'état visuel des boutons (le bouton sélectionné devient non-interactif)
        foreach (var kvp in heroButtons)
        {
            kvp.Value.interactable = (kvp.Key != hero.entityName);
        }

        // 2. Mettre à jour le GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSelectedHero(hero.entityName);
        }

        // 3. Mettre à jour l'aperçu 3D (Optionnel)
        UpdatePreviewModel(hero.entityName);
    }

    private void UpdatePreviewModel(string heroName)
    {
        if (previewSpawnPoint == null || heroDatabase == null) return;

        if (currentPreviewModel != null)
        {
            Destroy(currentPreviewModel);
        }

        GameObject prefab = heroDatabase.GetPrefab(heroName);
        if (prefab != null)
        {
            currentPreviewModel = Instantiate(prefab, previewSpawnPoint.position, previewSpawnPoint.rotation, previewSpawnPoint);
        }
    }

    // À lier au bouton "JOUER"
    public void OnPlayButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("[HeroSelection] GameManager introuvable ! Impossible de lancer le jeu.");
        }
    }
}
