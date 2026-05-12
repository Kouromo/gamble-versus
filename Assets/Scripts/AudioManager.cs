using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip combatMusic;

    [Header("Default Clips")]
    public AudioClip buttonClickSFX;
    public AudioClip healSFX;
    public AudioClip deathSFX;

    [Header("Attack Specific Clips")]
    public AudioClip attackMeleeSFX;
    public AudioClip attackMagicSFX;
    public AudioClip attackMonsterSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad ne fonctionne que sur les objets à la racine !
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            // --- SÉCURITÉ : Vérifier qu'on a bien DEUX AudioSources séparés ---
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            
            if (musicSource == sfxSource)
            {
                Debug.LogWarning("[AudioManager] Le MÊME AudioSource était assigné à la musique et aux SFX ! Un deuxième a été créé automatiquement pour éviter que les volumes ne s'écrasent.");
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            // ------------------------------------------------------------------

            // Charger les volumes sauvegardés dès le réveil
            float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            
            Debug.Log($"[AudioManager] Volumes chargés au démarrage - Musique: {savedMusic}, SFX: {savedSFX}");
            
            SetMusicVolume(savedMusic);
            SetSFXVolume(savedSFX);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (menuMusic != null)
        {
            PlayMenuMusic();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null) musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        
        // Si la musique demandée est déjà en train d'être jouée, on ne la redémarre pas
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        // Force l'application du volume au cas où l'AudioSource vient de s'activer
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayCombatMusic() => PlayMusic(combatMusic);

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // Méthodes utilitaires pour appeler des sons spécifiques
    public void PlayMeleeAttackSound() => PlaySFX(attackMeleeSFX);
    public void PlayMagicAttackSound() => PlaySFX(attackMagicSFX);
    public void PlayMonsterAttackSound() => PlaySFX(attackMonsterSFX);
    public void PlayHealSound() => PlaySFX(healSFX);
    public void PlayDeathSound() => PlaySFX(deathSFX);
    public void PlayButtonClick() => PlaySFX(buttonClickSFX);
}
