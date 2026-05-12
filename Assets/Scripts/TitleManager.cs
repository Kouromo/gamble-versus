using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Tooltip("Le nom exact de la scène de sélection des héros")]
    public string selectionSceneName = "ClassSelection";

    [Header("UI References")]
    public UnityEngine.UI.Button loadButton;

    private RectTransform[] buttonRects;
    private float[] targetY;

    [SerializeField]
    private GameObject paticleParent;

    private class UIParticle
    {
        public RectTransform rect;
        public float pspeed;
        public float phase;
        public float drift;
        public float originalX;
    }

    private UIParticle[] particles;
    private RectTransform canvasRect;

    private void Start()
    {
        AudioManager.Instance?.PlayMenuMusic();
        
        // Si le bouton n'est pas assigné dans l'inspecteur, on essaie de le trouver
        if (loadButton == null)
        {
            GameObject loadObj = GameObject.Find("Charger Partie");
            if (loadObj != null)
            {
                loadButton = loadObj.GetComponent<UnityEngine.UI.Button>();
            }
        }

        // Désactiver le bouton Charger si aucune sauvegarde n'existe
        if (loadButton != null && SaveManager.Instance != null)
        {
            loadButton.interactable = SaveManager.Instance.HasSaveFile();
        }

        // Initialiser l'animation de flottement pour tous les boutons actifs
        UnityEngine.UI.Button[] allButtons = FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        buttonRects = new RectTransform[allButtons.Length];
        targetY = new float[allButtons.Length];
        
        for (int i = 0; i < allButtons.Length; i++)
        {
            buttonRects[i] = allButtons[i].GetComponent<RectTransform>();
            targetY[i] = buttonRects[i].anchoredPosition.y;
        }

        // --- Création du système de particules en arrière-plan ---
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
            float width = canvasRect.rect.width;
            float height = canvasRect.rect.height;

            particles = new UIParticle[50]; // 50 particules
            Color particleColor;
            ColorUtility.TryParseHtmlString("#672240", out particleColor);
            
            for (int i = 0; i < particles.Length; i++)
            {
                GameObject pObj = new GameObject("Particle_" + i);
                pObj.transform.SetParent(paticleParent.transform, false);
                pObj.transform.SetAsFirstSibling(); // Mettre en arrière-plan (derrière les boutons)
                
                UnityEngine.UI.Image img = pObj.AddComponent<UnityEngine.UI.Image>();
                img.color = particleColor;
                // Retire le raycast target pour ne pas bloquer les clics sur les boutons
                img.raycastTarget = false;
                
                RectTransform rect = pObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                
                float size = Random.Range(4f, 12f);
                rect.sizeDelta = new Vector2(size, size);
                
                UIParticle p = new UIParticle();
                p.rect = rect;
                p.pspeed = Random.Range(30f, 100f);
                p.phase = Random.Range(0f, Mathf.PI * 2f);
                p.drift = Random.Range(15f, 40f);
                
                p.originalX = Random.Range(-width / 2f, width / 2f);
                float startY = Random.Range(-height / 2f, height / 2f);
                
                rect.anchoredPosition = new Vector2(p.originalX, startY);
                particles[i] = p;
            }
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float time = Time.time;

        // Animation des boutons
        if (buttonRects != null)
        {
            for (int i = 0; i < buttonRects.Length; i++)
            {
                if (buttonRects[i] != null)
                {
                    Vector2 pos = buttonRects[i].anchoredPosition;
                    // Formule de flottement : target_y + math.sin(time * 1.5 + i * 0.8) * 3
                    pos.y = targetY[i] + Mathf.Sin(time * 1.5f + i * 0.8f) * 3f;
                    buttonRects[i].anchoredPosition = pos;
                }
            }
        }

        // Animation des particules
        if (particles != null && canvasRect != null)
        {
            float height = canvasRect.rect.height;
            float width = canvasRect.rect.width;

            for (int i = 0; i < particles.Length; i++)
            {
                UIParticle p = particles[i];
                if (p.rect == null) continue;

                Vector2 pos = p.rect.anchoredPosition;
                
                // Logique demandée :
                // p.y = p.y - pspeed * dt
                // p.x = p.x + math.sin(time + p.phase) * drift
                // (On utilise p.originalX comme base pour x pour éviter un décalage infini hors de l'écran)
                
                pos.y = pos.y - p.pspeed * dt;
                pos.x = p.originalX + Mathf.Sin(time + p.phase) * p.drift;

                // Si la particule sort de l'écran par le bas, on la remet en haut
                if (pos.y < -height / 2f - 50f)
                {
                    pos.y = height / 2f + 50f;
                    p.originalX = Random.Range(-width / 2f, width / 2f);
                }

                p.rect.anchoredPosition = pos;
            }
        }
    }

    // À lier au bouton "Nouvelle Partie" ou "Start"
    public void OnStartButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        
        // Supprimer l'ancienne sauvegarde si on commence une nouvelle partie
        if (SaveManager.Instance != null) SaveManager.Instance.DeleteSave();
        
        Debug.Log("[TitleManager] Chargement de l'écran de sélection...");
        SceneManager.LoadScene(selectionSceneName);
    }

    // À lier au bouton "Charger Partie"
    public void OnLoadButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        
        if (SaveManager.Instance == null || !SaveManager.Instance.HasSaveFile()) return;

        SaveData data = SaveManager.Instance.LoadGame();
        if (data != null && GameManager.Instance != null)
        {
            GameManager.Instance.isLoadingSave = true;
            GameManager.Instance.loadedSaveData = data;
            GameManager.Instance.selectedHeroName = data.heroName;
            
            Debug.Log("[TitleManager] Reprise de la partie sauvegardée...");
            GameManager.Instance.StartGame(); // Lance directement la scène Main
        }
    }

    // À lier à un bouton "Quitter" (Optionnel)
    public void OnQuitButtonClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        
        Debug.Log("[TitleManager] Fermeture du jeu.");
        Application.Quit();
        
        // Si on est dans l'éditeur Unity, on arrête la lecture pour tester que le bouton marche
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}