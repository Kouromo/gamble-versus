using UnityEngine;
using System.Collections;

public class CombatEntity : MonoBehaviour
{
    public EntityData baseData;

    [HideInInspector]
    public int currentHealth;
    
    [HideInInspector]
    public bool isPlayer;
    
    [HideInInspector]
    public HealthBar healthBar;

    [HideInInspector]
    public Animator animator;

    [Header("Visual Effects")]
    [Tooltip("Particule jouée sur SOI quand on prend un coup (générique)")]
    public GameObject defaultHitParticlePrefab; 
    
    [Tooltip("Particule jouée sur la CIBLE quand on réussit une attaque (dépend de notre arme)")]
    public GameObject weaponHitParticlePrefab;
    
    private Vector3 originalPosition;
    private Renderer[] renderers;
    private Color[] originalColors;

    public void Initialize(EntityData data, bool isPlayerEntity)
    {
        baseData = data;
        currentHealth = data.maxHealth;
        isPlayer = isPlayerEntity;
        originalPosition = transform.localPosition; // On utilise localPosition au cas où il est dans un parent

        // Sauvegarder les couleurs d'origine pour l'effet de dégâts
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        if (healthBar != null)
        {
            healthBar.SetTarget(this);
        }
    }

    public void TakeDamage(int damage, GameObject customHitEffect = null)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        
        CombatLogUI.Instance?.Log($"{baseData.entityName} prend {damage} dégâts. ({currentHealth}/{baseData.maxHealth})");
        DamagePopup.Create(transform.position + Vector3.up * 1.5f, damage, false);

        if (healthBar != null)
        {
            healthBar.UpdateHealth();
        }

        if (animator != null) animator.SetTrigger("Hit");

        // Lancer les effets visuels de dégâts
        StartCoroutine(DamageVisualRoutine(customHitEffect));
        
        // Jouer un son d'attaque (impact)
        AudioManager.Instance?.PlayAttackSound();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageVisualRoutine(GameObject customHitEffect)
    {
        GameObject effectToPlay = customHitEffect != null ? customHitEffect : defaultHitParticlePrefab;

        // 1. Instancier des particules si on en a
        if (effectToPlay != null)
        {
            // On le place légèrement au-dessus du centre du personnage
            Instantiate(effectToPlay, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        // 2. Faire clignoter le modèle en rouge
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = Color.red;
            }
        }

        // 3. Petit effet de tremblement (Shake)
        Vector3 startPos = transform.localPosition;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-0.1f, 0.1f);
            float offsetZ = Random.Range(-0.1f, 0.1f);
            transform.localPosition = startPos + new Vector3(offsetX, 0, offsetZ);
            yield return null;
        }

        transform.localPosition = startPos; // Remettre à la position avant le tremblement

        // 4. Remettre les couleurs normales
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = originalColors[i];
            }
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

        // Jouer le son de soin
        AudioManager.Instance?.PlayHealSound();
        DamagePopup.Create(transform.position + Vector3.up * 1.5f, amount, true);

        // Petit effet visuel pour le soin (Clignotement Vert)
        StartCoroutine(HealVisualRoutine());
    }

    private IEnumerator HealVisualRoutine()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = Color.green;
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
        }
    }

    public void PerformAttack(CombatEntity target)
    {
        // Lancer l'animation d'attaque (le calcul des dégâts se fera au milieu de l'animation)
        StartCoroutine(AttackVisualRoutine(target));
    }

    private IEnumerator AttackVisualRoutine(CombatEntity target)
    {
        Vector3 startPos = transform.localPosition;
        // Direction vers la cible
        Vector3 targetPos = target.transform.position;
        // Position d'arrivée du "bond" : à mi-chemin vers la cible
        Vector3 attackPos = transform.position + (targetPos - transform.position).normalized * 1f;

        float bumpDuration = 0.15f; // Très rapide pour donner de l'impact
        float elapsed = 0f;

        // 1. Bondir en avant
        while (elapsed < bumpDuration)
        {
            transform.position = Vector3.Lerp(transform.position, attackPos, elapsed / bumpDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. Déclencher l'animation une fois arrivé au corps à corps
        if (animator != null) 
        {
            animator.SetTrigger("Attack");
        }

        // Attendre un peu que le coup parte dans l'animation (ex: 0.4s)
        yield return new WaitForSeconds(0.4f);

        // === CALCUL DES DÉGÂTS ICI (au moment de l'impact) ===
        ApplyDamageLogic(target);

        // Attendre la fin de l'animation d'attaque (ex: 0.6s supplémentaires)
        yield return new WaitForSeconds(0.6f);

        // 3. Revenir à la position de départ
        elapsed = 0f;
        float returnDuration = 0.2f;
        while (elapsed < returnDuration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
    }

    private void ApplyDamageLogic(CombatEntity target)
    {
        // 1. Esquive (Dodge)
        if (Random.Range(0, 100) < target.baseData.dodge)
        {
            CombatLogUI.Instance?.Log($"{target.baseData.entityName} a esquivé l'attaque de {baseData.entityName} !");
            DamagePopup.CreateText(target.transform.position + Vector3.up * 1.5f, "Esquive !", Color.cyan);
            return;
        }

        // 2. Jets de réussite (Rolls)
        int successes = 0;
        string rollVisual = "";
        for (int i = 0; i < baseData.rolls; i++)
        {
            if (Random.Range(0, 100) < baseData.mainStat)
            {
                successes++;
                rollVisual += "<color=yellow>■</color> ";
            }
            else
            {
                rollVisual += "<color=grey>□</color> ";
            }
        }

        DamagePopup.CreateText(transform.position + Vector3.up * 2f, rollVisual.Trim(), Color.white);

        // 3. Calcul des dégâts proportionnels
        if (successes == 0)
        {
            CombatLogUI.Instance?.Log($"{baseData.entityName} a complètement raté son attaque (0/{baseData.rolls} succès).");
            DamagePopup.CreateText(transform.position + Vector3.up * 1.5f, "Raté !", Color.gray);
            return;
        }

        float potentialDamage = Random.Range(baseData.minAttackDamage, baseData.maxAttackDamage + 1);
        int finalDamage = Mathf.RoundToInt(potentialDamage * ((float)successes / baseData.rolls));

        CombatLogUI.Instance?.Log($"{baseData.entityName} attaque {target.baseData.entityName} : {successes}/{baseData.rolls} succès ! Dégâts : {finalDamage}");
        target.TakeDamage(finalDamage, weaponHitParticlePrefab);
    }

    private void Die()
    {
        CombatLogUI.Instance?.Log($"{baseData.entityName} est mort !");
        
        if (animator != null) animator.SetTrigger("Die");

        // Jouer le son de mort
        AudioManager.Instance?.PlayDeathSound();

        // Petit effet avant de mourir : s'enfoncer dans le sol ou disparaître
        StartCoroutine(DeathVisualRoutine());
    }

    private IEnumerator DeathVisualRoutine()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, startPos + Vector3.down * 2f, elapsed / duration);
            // Réduire la taille
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
