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

    [Header("Génération des Cibles")]
    public GameObject targetButtonPrefab; // Un Prefab avec un Button et TextMeshProUGUI
    public Transform targetButtonContainer; // Le Layout Group (Vertical ou Horizontal)

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
    }

    public void HideAll()
    {
        if (actionPanel != null) actionPanel.SetActive(false);
        if (targetPanel != null) targetPanel.SetActive(false);
    }

    // À lier au OnClick() du bouton "Attaquer"
    public void OnAttackButtonClicked()
    {
        actionPanel.SetActive(false);
        ShowTargets();
    }

    // À lier au OnClick() du bouton "Potion"
    public void OnPotionButtonClicked()
    {
        TurnManager.Instance.PlayerUseItem("potion");
        HideAll();
    }

    // À lier au OnClick() d'un bouton "Retour" (Optionnel)
    public void OnBackButtonClicked()
    {
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

        // Générer un bouton pour chaque ennemi en vie
        foreach (CombatEntity entity in TurnManager.Instance.combatants)
        {
            if (!entity.isPlayer && entity.currentHealth > 0)
            {
                GameObject btnObj = Instantiate(targetButtonPrefab, targetButtonContainer);
                spawnedTargetButtons.Add(btnObj);

                // Mettre le nom de l'ennemi
                TextMeshProUGUI textComp = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = entity.baseData.entityName + $" ({entity.currentHealth} PV)";
                }

                // Configurer l'action du bouton
                Button btn = btnObj.GetComponent<Button>();
                CombatEntity targetToAttack = entity; // Capture pour la lambda
                
                btn.onClick.AddListener(() => 
                {
                    TurnManager.Instance.PlayerAttackTarget(targetToAttack);
                    HideAll();
                });
            }
        }
    }
}
