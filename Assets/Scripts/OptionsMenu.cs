using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private bool isInitializing = true;

    private void Start()
    {
        isInitializing = true;

        // Charger les valeurs sauvegardées
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        if (musicSlider != null)
        {
            // Retirer temporairement les listeners pour ne pas déclencher d'événements
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Appliquer les volumes au démarrage (au cas où l'AudioManager ne l'a pas encore fait)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(savedMusic);
            AudioManager.Instance.SetSFXVolume(savedSFX);
        }

        isInitializing = false;
    }

    public void SetMusicVolume(float volume)
    {
        if (isInitializing) return; // Ignore les événements envoyés par Unity au démarrage

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
        
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (isInitializing) return; // Ignore les événements envoyés par Unity au démarrage

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
        
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}
