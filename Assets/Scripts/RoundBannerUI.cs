using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class RoundBannerUI : MonoBehaviour
{
    [Tooltip("Glissez ici le composant TextMeshPro qui affiche le numéro du round.")]
    public TextMeshProUGUI roundText;
    
    public float displayDuration = 2f;
    public float fadeDuration = 1f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Si le texte n'est pas assigné manuellement, on essaie de le trouver dans les enfants
        if (roundText == null)
        {
            roundText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // Cacher tout le groupe au démarrage
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        LevelGenerator.OnRoundStarted += ShowRoundBanner;
    }

    private void OnDisable()
    {
        LevelGenerator.OnRoundStarted -= ShowRoundBanner;
    }

    private void ShowRoundBanner(int roundNumber)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        if (roundText != null)
        {
            roundText.text = $"--- ROUND {roundNumber} ---";
        }
        fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // Fondu entrant (Apparition)
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Attente
        yield return new WaitForSeconds(displayDuration);

        // Fondu sortant (Disparition)
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
