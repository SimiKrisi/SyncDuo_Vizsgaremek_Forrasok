using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Constants
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const float DEFAULT_VOLUME = 1f;
    #endregion

    #region Singleton
    public static AudioManager Instance { get; private set; }
    #endregion

    #region Audio Sources
    [Header("Hangforrások")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip victorySFX;//
    public AudioClip stepSFX;//
    public AudioClip defeatSFX;//
    public AudioClip introSFX; //
    public AudioClip achievementClaimSFX; //
    public AudioClip roundStartsSFX;//
    public AudioClip enemyGotAngrySFX;//
    public AudioClip palmEntry;
    public AudioClip clickUI;

    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        InitializeSingleton();
    }

    void Start()
    {
        LoadAndApplySavedVolumes();
        StartBackgroundMusic();
    }
    #endregion

    #region Public API
    /// <summary>
    /// Megadja a háttérzene és az effekt hangerejét.
    /// </summary>

    public (float musicVolume, float sfxVolume) GetCurrentVolumes()
    {
        float musicVolume = musicSource != null ? musicSource.volume : DEFAULT_VOLUME;
        float sfxVolume = sfxSource != null ? sfxSource.volume : DEFAULT_VOLUME;
        return (musicVolume, sfxVolume);
    }

    /// <summary>
    /// Beállítja a háttérzene hangerejét és elmenti.
    /// </summary>

    
    public void SetMusicVolume(float volume, bool save)
    {
        if (musicSource == null) return;
        musicSource.volume = volume;
        if (save)
        {
            SaveVolume(MUSIC_KEY, volume);
        }
    }

    /// <summary>
    /// Beállítja a hangeffektek hangerejét és elmenti.
    /// </summary>
    public void SetSFXVolume(float volume, bool save)
    {
        if (sfxSource == null) return;

        sfxSource.volume = volume;
        if (save)
        {
            SaveVolume(SFX_KEY, volume);
        }
    }

    /// <summary>
    /// Lejátszik egy hangeffektet.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Lejátssza a gyõzelmi hangeffektet.
    /// </summary>
    public void PlayVictorySFX()
    {
        PlaySFX(victorySFX);
    }

    ///<summary>
    /// Lejátsza a játékkezdés hangeffektet.
    /// </summary>
    public void PlayRoundStartSFX()
    {
        PlaySFX(roundStartsSFX);
    }

    /// <summary>
    /// Lejátssza a léptetési hangeffektet.
    /// </summary>
    public void PlayStepSFX()
    {
        PlaySFX(stepSFX);
    }

    /// <summary>
    /// Lejátssza a vesztés hangeffektet.
    /// </summary>
    public void PlayDefeatSFX()
    {
        PlaySFX(defeatSFX);
    }

    ///<summary>
    /// Lejátsza  az ellenség mérgesedési hangeffektet.
    /// </summary>
    public void PlayEnemyGotAngrySFX()
    {
        PlaySFX(enemyGotAngrySFX);
    }
    

    /// <summary>
    /// Lejátssza az intro hangeffektet.
    /// </summary>
    public void PlayIntroSFX()
    {
        PlaySFX(introSFX);
    }

    /// <summary>
    /// Lejátssza az achievement megszerzési hangeffektet.
    /// </summary>
    public void PlayAchievementClaimSFX()
    {
        PlaySFX(achievementClaimSFX);
    }
    
    /// <summary>
    /// Lejátsza az klikkelés hangját
    /// </summary>
    public void PlayUIClicked()
    {
        PlaySFX(clickUI);
    }
    #endregion

    #region Private Methods
    private void InitializeSingleton()
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

    public void LoadAndApplySavedVolumes()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, DEFAULT_VOLUME);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFX_KEY, DEFAULT_VOLUME);

        #if UNITY_EDITOR || DEBUG
        Debug.Log($"ZENE volume betöltve: {savedMusicVolume}");
        #endif

        if (musicSource != null)
        {
            musicSource.volume = savedMusicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = savedSFXVolume;
        }
    }

    private void StartBackgroundMusic()
    {
        if (musicSource == null) return;

        if (!musicSource.isPlaying)
        {
            #if UNITY_EDITOR || DEBUG
            Debug.Log($"ZENE ELINDÍTVA: {musicSource.volume}");
            #endif

            musicSource.Play();
        }
    }

    private void SaveVolume(string key, float volume)
    {
        PlayerPrefs.SetFloat(key, volume);
        PlayerPrefs.Save();
    }
    #endregion
}
