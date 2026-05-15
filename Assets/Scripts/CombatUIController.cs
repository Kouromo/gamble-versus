using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CombatUIController : MonoBehaviour
{
    public static CombatUIController Instance { get; private set; }

    [Header("Panneaux UI")]
    public GameObject actionPanel; // Contient les boutons Attaquer et Potion
    public GameObject targetPanel; // Contient les boutons pour cibler les ennemis

    [Header("Polices et Styles")]
    public TMP_FontAsset damagePopupFont; // Police utilisée pour les nombres de dégâts flottants

    [Header("Génération des Cibles")]
    public GameObject targetButtonPrefab; // Un Prefab avec un Button et TextMeshProUGUI
    public Transform targetButtonContainer; // Le Layout Group (Vertical ou Horizontal)

    [Header("Boutons")]
    public Button potionButton;

    private List<GameObject> spawnedTargetButtons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            HideAll();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Appelé par le TurnManager quand c'est le tour du joueur
    public void ShowPlayerActions()
    {
        actionPanel.SetActive(true);
        targetPanel.SetActive(false);

        // Mise à jour de l'état du bouton Potion
        if (potionButton != null && InventoryManager.Instance != null)
        {
            int potionCount = InventoryManager.Instance.GetItemCount("potion_health");
            potionButton.interactable = potionCount > 0;
            
            TextMeshProUGUI potionText = potionButton.GetComponentInChildren<TextMeshProUGUI>();
            if (potionText != null)
            {
                potionText.text = $"Potion ({potionCount})";
            }
        }
    }

    public void HideAll()
    {
        if (actionPanel != null) actionPanel.SetActive(false);
        if (targetPanel != null) targetPanel.SetActive(false);
    }

    // À lier au OnClick() du bouton "Attaquer"
    public void OnAttackButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        actionPanel.SetActive(false);
        ShowTargets();
    }

    // À lier au OnClick() du bouton "Potion"
    public void OnPotionButtonClicked()
    {
        if (!InventoryManager.Instance.HasItem("potion_health"))
        {
            CombatLogUI.Instance?.Log("Vous n'avez pas de potion !");
            return;
        }
        AudioManager.Instance?.PlayButtonClick();
        TurnManager.Instance.PlayerUseItem("potion_health");
        HideAll();
    }

    // À lier au OnClick() d'un bouton "Retour" (Optionnel)
    public void OnBackButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        ShowPlayerActions();
    }

    private void ShowTargets()
    {
        targetPanel.SetActive(true);

        // Nettoyer les anciens boutons
        foreach (GameObject btn in spawnedTargetButtons)
        {
            Destroy(btn);
        }
        spawnedTargetButtons.Clear();

        // 1. Récupérer tous les ennemis en vie
        List<CombatEntity> livingEnemies = new List<CombatEntity>();
        foreach (CombatEntity entity in TurnManager.Instance.combatants)
        {
            if (!entity.isPlayer && entity.currentHealth > 0)
            {
                livingEnemies.Add(entity);
            }
        }

        // 2. Les trier de gauche à droite par rapport à l'écran (caméra)
        if (Camera.main != null)
        {
            livingEnemies.Sort((a, b) => 
                Camera.main.WorldToScreenPoint(a.transform.position).x.CompareTo(
                Camera.main.WorldToScreenPoint(b.transform.position).x)
            );
        }

        // 3. Générer les boutons triés et numérotés
        int targetIndex = 1;
        foreach (CombatEntity entity in livingEnemies)
        {
            GameObject btnObj = Instantiate(targetButtonPrefab, targetButtonContainer);
            spawnedTargetButtons.Add(btnObj);

            // Mettre le nom de l'ennemi avec son numéro (ex: "1. Bat (10 PV)")
            TextMeshProUGUI textComp = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = $"{targetIndex}. {entity.baseData.entityName} ({entity.currentHealth} PV)";
            }

            // Configurer l'action du bouton
            Button btn = btnObj.GetComponent<Button>();
            CombatEntity targetToAttack = entity; // Capture pour la lambda
            
            btn.onClick.AddListener(() => 
            {
                AudioManager.Instance?.PlayButtonClick();
                TurnManager.Instance.PlayerAttackTarget(targetToAttack);
                HideAll();
            });

            targetIndex++;
        }
    }
}
