using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    public EntityData baseData;
    public int currentHealth;
    public bool isPlayer;

    public void Initialize(EntityData data, bool isPlayerEntity)
    {
        baseData = data;
        currentHealth = data.maxHealth;
        isPlayer = isPlayerEntity;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        
        Debug.Log($"[Combat] {baseData.entityName} prend {damage} dégâts. PV restants: {currentHealth}/{baseData.maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[Combat] {baseData.entityName} est mort !");
        // Désactiver ou détruire l'objet, ou jouer une animation
        gameObject.SetActive(false);
    }
}
