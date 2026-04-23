using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A játék jelenet UI kezelését végző osztály.
/// Kezeli a paneleket, gombokat, timer-t és a játékmód specifikus UI beállításokat.
/// </summary>
public class GameSceneUIController : MonoBehaviour
{
    #region Konstansok

    private const string LevelTextFormat = "LEVEL {0}";
    private const string DailyChallengeText = "DAILY CHALLENGE";
    private const string OneMinuteChallengeText = "1 MINUTE CHALLENGE";
    private const string TimerFormat = "TIME: {0}s";
    private const string CompletedLevelsFormat = "COMPLETED: {0} LEVELS";
    private const string GameSceneName = "GameScene";
    private const string LevelSelectSceneName = "LevelSelectScene";
    private const string MainMenuSceneName = "MainMenuScene";

    #endregion

    #region Singleton

    public static GameSceneUIController Instance;

    #endregion

    #region Inspector Mezők - Panelek

    [Header("UI Panelek")]
    public GameObject hudPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject tutorialPanel;

    #endregion

    #region Inspector Mezők - UI Elemek

    [Header("Szöveg Elemek")]
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI timerText;

    [Header("Pause Menu Gombok")]
    public Button pauseMenuRestartButton;
    public Button pauseMenuNextLevelButton;

    [Header("Complete Menu Gombok")]
    public Button completeMenuRestartButton;
    public Button completeMenuNextLevelButton;

    #endregion

    #region Inspector Mezők - Audio

    [Header("Audio Beállítások")]
    public Image muteButtonImage;
    public Sprite audioOnSprite;
    public Sprite audioOffSprite;

    #endregion

    #region Privát Mezők

    private bool isMuted = false;

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Singleton példány inicializálása.
    /// </summary>
    void Awake()
    {
        InitializeSingleton();
    }

    /// <summary>
    /// UI beállítása a játékmód alapján.
    /// </summary>
    void Start()
    {
        PlayStartSFX();
        SetupUIByGamemode();
    }

    #endregion

    #region Audio Kezelés
    private void PlayStartSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRoundStartSFX();
        }
        
    }
    /// <summary>
    /// Lejátssza a klikkelés hangeffektet.
    /// </summary>
    private void PlayButtonSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClicked();
        }
    }
    #endregion

    #region Singleton Kezelés

    /// <summary>
    /// Inicializálja a singleton példányt.
    /// </summary>
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region UI Inicializálás

    /// <summary>
    /// Beállítja a UI elemeket a játékmód alapján.
    /// </summary>
    private void SetupUIByGamemode()
    {
        string gamemode = LevelContext.Gamemode;
        SetMuteButtonBySourceVolume();
        if (gamemode == "normal")
        {
            SetupNormalModeUI();
        }
        else if (gamemode == "dailychallenge")
        {
            SetupDailyChallengeUI();
        }
        else if (gamemode == "1minchallenge")
        {
            SetupOneMinuteChallengeUI();
        }
        else
        {
            HideTimerText();
        }
    }

    /// <summary>
    /// Beállítja a UI-t normal játékmódhoz.
    /// </summary>
    private void SetupNormalModeUI()
    {
        SetupTutorial();
        EnableAllButtons();
        SetLevelText();
        HideTimerText();
    }

    /// <summary>
    /// Beállítja a UI-t daily challenge módhoz.
    /// </summary>
    private void SetupDailyChallengeUI()
    {
        DisableNavigationButtons();
        SetCurrentLevelText(DailyChallengeText);
        HideTimerText();
    }

    /// <summary>
    /// Beállítja a UI-t 1 perces kihívás módhoz.
    /// </summary>
    private void SetupOneMinuteChallengeUI()
    {
        DisableRestartButtons();
        EnableNextLevelButton();
        SetCurrentLevelText(OneMinuteChallengeText);
        ShowTimerText();
    }

    /// <summary>
    /// Engedélyezi az összes navigációs gombot.
    /// </summary>
    private void EnableAllButtons()
    {
        SetButtonActive(pauseMenuRestartButton, true);
        SetButtonActive(pauseMenuNextLevelButton, true);
        SetButtonActive(completeMenuRestartButton, true);
        SetButtonActive(completeMenuNextLevelButton, true);
    }

    /// <summary>
    /// Letiltja az összes navigációs gombot.
    /// </summary>
    private void DisableNavigationButtons()
    {
        SetButtonActive(pauseMenuRestartButton, false);
        SetButtonActive(pauseMenuNextLevelButton, false);
        SetButtonActive(completeMenuRestartButton, false);
        SetButtonActive(completeMenuNextLevelButton, false);
    }

    /// <summary>
    /// Letiltja az újraindítás gombokat.
    /// </summary>
    private void DisableRestartButtons()
    {
        SetButtonActive(pauseMenuRestartButton, false);
        SetButtonActive(pauseMenuNextLevelButton, false);
        SetButtonActive(completeMenuRestartButton, false);
    }

    /// <summary>
    /// Engedélyezi a következő pálya gombot.
    /// </summary>
    private void EnableNextLevelButton()
    {
        SetButtonActive(completeMenuNextLevelButton, true);
    }

    /// <summary>
    /// Beállít egy gomb aktív állapotát.
    /// </summary>
    private void SetButtonActive(Button button, bool isActive)
    {
        if (button != null)
        {
            button.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// Beállítja a szint szövegét a LevelContext alapján.
    /// </summary>
    private void SetLevelText()
    {
        if (currentLevelText != null)
        {
            int visualLevelNumber = LevelContext.CurrentLevelIndex + 1;
            currentLevelText.text = string.Format(LevelTextFormat, visualLevelNumber);
        }
    }

    /// <summary>
    /// Beállítja a jelenlegi szint szövegét.
    /// </summary>
    private void SetCurrentLevelText(string text)
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = text;
        }
    }

    /// <summary>
    /// Megjeleníti a timer szöveget.
    /// </summary>
    private void ShowTimerText()
    {
        SetTimerTextActive(true);
    }

    /// <summary>
    /// Elrejti a timer szöveget.
    /// </summary>
    private void HideTimerText()
    {
        SetTimerTextActive(false);
    }

    /// <summary>
    /// Beállítja a timer szöveg aktív állapotát.
    /// </summary>
    private void SetTimerTextActive(bool isActive)
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(isActive);
        }
    }

    #endregion

    #region Tutorial Kezelés

    /// <summary>
    /// Beállítja a tutorial megjelenítését az első pályán.
    /// </summary>
    private void SetupTutorial()
    {
        if (tutorialPanel == null)
            return;

        bool isFirstLevel = LevelContext.CurrentLevelIndex == 0;
        tutorialPanel.SetActive(isFirstLevel);
    }

    /// <summary>
    /// Elrejti a tutorial panelt.
    /// </summary>
    public void HideTutorial()
    {
        if (tutorialPanel != null && tutorialPanel.activeSelf)
        {
            tutorialPanel.SetActive(false);
        }
    }

    #endregion

    #region Game Over Kezelés

    /// <summary>
    /// Elindítja a game over szekvenciát.
    /// </summary>
    public void TriggerGameOver()
    {
        ExecuteGameOverSequence();
    }

    /// <summary>
    /// Végrehajtja a game over szekvenciát (ellenségek, játékosok megállítása, UI).
    /// </summary>
    private void ExecuteGameOverSequence()
    {
        UnityEngine.Debug.Log("Játékos elkapva! Azonnali leállítás...");

        StopAllEnemies();
        StopAllPlayers();
        FreezeGameAndShowGameOverUI();

        UnityEngine.Debug.Log("Game Over UI Activated (Immediate)");
    }

    /// <summary>
    /// Leállítja az összes ellenséget.
    /// </summary>
    private void StopAllEnemies()
    {
        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.StopEnemy();
            }
        }
    }

    /// <summary>
    /// Leállítja az összes játékost.
    /// </summary>
    private void StopAllPlayers()
    {
        PlayerController[] allPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        foreach (PlayerController player in allPlayers)
        {
            if (player != null)
            {
                DisablePlayerController(player);
                StopPlayerPhysics(player);
            }
        }
    }

    /// <summary>
    /// Letiltja a játékos controller-ét.
    /// </summary>
    private void DisablePlayerController(PlayerController player)
    {
        player.enabled = false;
    }

    /// <summary>
    /// Megállítja a játékos fizikáját.
    /// </summary>
    private void StopPlayerPhysics(PlayerController player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Fagyasztja a játékot és megjeleníti a game over UI-t.
    /// </summary>
    private void FreezeGameAndShowGameOverUI()
    {
        Time.timeScale = 0f;

        ShowGameOverPanel();
        HideHUDPanel();
        HidePauseMenuPanel();
    }

    /// <summary>
    /// Megjeleníti a game over panelt.
    /// </summary>
    private void ShowGameOverPanel()
    {
        SetPanelActive(gameOverPanel, true);
    }

    /// <summary>
    /// Elrejti a HUD panelt.
    /// </summary>
    private void HideHUDPanel()
    {
        SetPanelActive(hudPanel, false);
    }

    /// <summary>
    /// Elrejti a pause menu panelt.
    /// </summary>
    private void HidePauseMenuPanel()
    {
        SetPanelActive(pauseMenuPanel, false);
    }

    /// <summary>
    /// Beállít egy panel aktív állapotát.
    /// </summary>
    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }

    #endregion

    #region Pause/Resume Kezelés

    /// <summary>
    /// Megállítja a játékot és megjeleníti a pause menüt.
    /// </summary>
    public void PauseGame()
    {
        PlayButtonSFX();
        ShowPauseMenu();
        HideHUD();
        FreezeTime();
    }

    /// <summary>
    /// Folytatja a játékot és elrejti a pause menüt.
    /// </summary>
    public void ResumeGame()
    {
        PlayButtonSFX();
        HidePauseMenu();
        ShowHUD();
        UnfreezeTime();
    }

    /// <summary>
    /// Megjeleníti a pause menüt.
    /// </summary>
    private void ShowPauseMenu()
    {
        SetPanelActive(pauseMenuPanel, true);
    }

    /// <summary>
    /// Elrejti a pause menüt.
    /// </summary>
    private void HidePauseMenu()
    {
        SetPanelActive(pauseMenuPanel, false);
    }

    /// <summary>
    /// Megjeleníti a HUD-ot.
    /// </summary>
    private void ShowHUD()
    {
        SetPanelActive(hudPanel, true);
    }

    /// <summary>
    /// Elrejti a HUD-ot.
    /// </summary>
    private void HideHUD()
    {
        SetPanelActive(hudPanel, false);
    }

    /// <summary>
    /// Fagyasztja az időt.
    /// </summary>
    private void FreezeTime()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Feloldja az idő fagyasztást.
    /// </summary>
    private void UnfreezeTime()
    {
        Time.timeScale = 1f;
    }

    #endregion

    #region Scene Navigáció

    /// <summary>
    /// Újraindítja a jelenlegi pályát.
    /// </summary>
    public void RestartLevel()
    {
        PlayButtonSFX();
        UnfreezeTime();
        ReloadCurrentScene();
    }

    /// <summary>
    /// Betölti a következő pályát vagy újraindítja a kihívást.
    /// </summary>
    public void LoadNextLevel()
    {
        PlayButtonSFX();
        UnfreezeTime();

        if (IsOneMinuteChallenge())
        {
            RestartChallenge();
        }
        else
        {
            AdvanceToNextLevel();
        }
    }

    /// <summary>
    /// Visszatér a pálya választóhoz vagy a főmenübe.
    /// </summary>
    public void ReturnToLevelSelector()
    {
        PlayButtonSFX();
        UnfreezeTime();

        if (IsNormalMode())
        {
            LoadLevelSelectScene();
        }
        else
        {
            LoadMainMenuScene();
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy 1 perces kihívás módban vagyunk-e.
    /// </summary>
    private bool IsOneMinuteChallenge()
    {
        return LevelContext.Gamemode == "1minchallenge";
    }

    /// <summary>
    /// Ellenőrzi, hogy normal módban vagyunk-e.
    /// </summary>
    private bool IsNormalMode()
    {
        return LevelContext.Gamemode == "normal";
    }

    /// <summary>
    /// Újraindítja az 1 perces kihívást.
    /// </summary>
    private void RestartChallenge()
    {
        SceneManager.LoadScene(GameSceneName);
    }

    /// <summary>
    /// Továbblép a következő pályára.
    /// </summary>
    private void AdvanceToNextLevel()
    {
        LevelContext.CurrentLevelIndex += 1;
        SceneManager.LoadScene(GameSceneName);
    }

    /// <summary>
    /// Újratölti a jelenlegi scene-t.
    /// </summary>
    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Betölti a pálya választó scene-t.
    /// </summary>
    private void LoadLevelSelectScene()
    {
        SceneManager.LoadScene(LevelSelectSceneName);
    }

    /// <summary>
    /// Betölti a főmenü scene-t.
    /// </summary>
    private void LoadMainMenuScene()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }

    #endregion

    #region Audio Kezelés
    /// <summary>
    /// Sets the mute button state based on the current source volume level.
    /// </summary>
    /// <remarks>This method adjusts the mute button's state dynamically according to the volume level of the
    /// audio source. It is important to call this method whenever the source volume changes to ensure the mute button
    /// reflects the correct state.</remarks>
    public void SetMuteButtonBySourceVolume()
    {
        isMuted = AudioManager.Instance != null && (AudioManager.Instance.musicSource.volume == 0f && AudioManager.Instance.sfxSource.volume == 0f);
        UpdateMuteButtonSprite();

    }

    /// <summary>
    /// Váltja a némítás állapotát.
    /// </summary>
    public void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateMuteButtonSprite();
        UnityEngine.Debug.Log("Játék némítva: " + isMuted);
    }

    /// <summary>
    /// Frissíti a némítás gomb sprite-ját.
    /// </summary>
    private void UpdateMuteButtonSprite()
    {
        if (CanUpdateMuteSprite())
        {
            muteButtonImage.sprite = isMuted ? audioOffSprite : audioOnSprite;
        }
        UpdateAudioVolumes();
    }
    /// <summary>
    /// Updates the audio volumes for music and sound effects based on the current mute state.
    /// </summary>
    /// <remarks>If audio is not muted, both music and sound effects volumes are set to their maximum levels.
    /// If muted, both volumes are set to zero. This method should be called whenever the mute state changes to ensure
    /// audio output matches user preferences.</remarks>
    private void UpdateAudioVolumes()
    {
        if (!isMuted)
        {
            AudioManager.Instance.LoadAndApplySavedVolumes();
            AudioManager.Instance.LoadAndApplySavedVolumes();
        }
        else
        {
            AudioManager.Instance.SetMusicVolume(0f, false);
            AudioManager.Instance.SetSFXVolume(0f, false);
        }
    }
    /// <summary>
    /// Ellenőrzi, hogy frissíthető-e a némítás sprite.
    /// </summary>
    private bool CanUpdateMuteSprite()
    {
        return muteButtonImage != null && audioOnSprite != null && audioOffSprite != null;
    }

    #endregion

    #region 1 Perces Kihívás UI

    /// <summary>
    /// Frissíti a timer szöveget.
    /// </summary>
    public void UpdateTimer(float remainingTime)
    {
        if (ShouldUpdateTimer())
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = string.Format(TimerFormat, seconds);
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy frissíteni kell-e a timer-t.
    /// </summary>
    private bool ShouldUpdateTimer()
    {
        return timerText != null && IsOneMinuteChallenge();
    }

    /// <summary>
    /// Megjeleníti az 1 perces kihívás eredményeit.
    /// </summary>
    public void Show1MinChallengeResults(int levelsCompleted)
    {
        FreezeTime();
        DisplayCompletedLevels(levelsCompleted);
        LogChallengeResults(levelsCompleted);
    }

    /// <summary>
    /// Megjeleníti a teljesített pályák számát.
    /// </summary>
    private void DisplayCompletedLevels(int levelsCompleted)
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = string.Format(CompletedLevelsFormat, levelsCompleted);
        }
    }

    /// <summary>
    /// Logolja az 1 perces kihívás eredményeit.
    /// </summary>
    private void LogChallengeResults(int levelsCompleted)
    {
        Debug.Log($"⏱️ 1 perc lejárt! Teljesített pályák: {levelsCompleted}");
    }

    #endregion
}