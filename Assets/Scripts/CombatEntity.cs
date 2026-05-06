using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    public EntityData baseData;
    public int currentHealth;
    public bool isPlayer;
    public HealthBar healthBar;

    public void Initialize(EntityData data, bool isPlayerEntity)
    {
        baseData = data;
        currentHealth = data.maxHealth;
        isPlayer = isPlayerEntity;

        if (healthBar != null)
        {
            healthBar.SetTarget(this);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        
        CombatLogUI.Instance?.Log($"{baseData.entityName} prend {damage} dégâts. ({currentHealth}/{baseData.maxHealth})");

        if (healthBar != null)
        {
            healthBar.UpdateHealth();
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > baseData.maxHealth)
        {
            currentHealth = baseData.maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealth();
        }
    }

    public void PerformAttack(CombatEntity target)
    {
        // 1. Esquive (Dodge)
        if (Random.Range(0, 100) < target.baseData.dodge)
        {
            CombatLogUI.Instance?.Log($"{target.baseData.entityName} a esquivé l'attaque de {baseData.entityName} !");
            return;
        }

        // 2. Jets de réussite (Rolls)
        int successes = 0;
        for (int i = 0; i < baseData.rolls; i++)
        {
            if (Random.Range(0, 100) < baseData.mainStat)
            {
                successes++;
            }
        }

        // 3. Calcul des dégâts proportionnels
        if (successes == 0)
        {
            CombatLogUI.Instance?.Log($"{baseData.entityName} a complètement raté son attaque (0/{baseData.rolls} succès).");
            return;
        }

        float potentialDamage = Random.Range(baseData.minAttackDamage, baseData.maxAttackDamage + 1);
        int finalDamage = Mathf.RoundToInt(potentialDamage * ((float)successes / baseData.rolls));

        CombatLogUI.Instance?.Log($"{baseData.entityName} attaque {target.baseData.entityName} : {successes}/{baseData.rolls} succès ! Dégâts : {finalDamage}");
        target.TakeDamage(finalDamage);
    }

    private void Die()
    {
        CombatLogUI.Instance?.Log($"{baseData.entityName} est mort !");
        // Désactiver ou détruire l'objet, ou jouer une animation
        gameObject.SetActive(false);
    }
}
