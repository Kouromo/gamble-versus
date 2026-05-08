using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Default Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSFX;
    public AudioClip attackSFX;
    public AudioClip healSFX;
    public AudioClip deathSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Charger les volumes sauvegardés
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
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
        if (musicSource == null) return;
        
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // Méthodes utilitaires pour appeler des sons spécifiques
    public void PlayAttackSound() => PlaySFX(attackSFX);
    public void PlayHealSound() => PlaySFX(healSFX);
    public void PlayDeathSound() => PlaySFX(deathSFX);
    public void PlayButtonClick() => PlaySFX(buttonClickSFX);
}
