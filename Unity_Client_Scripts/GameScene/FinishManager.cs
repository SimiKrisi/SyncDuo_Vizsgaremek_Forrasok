using UnityEngine;
using System.Collections;

/// <summary>
/// A pálya teljesítésének kezelését végző osztály. 
/// Figyeli a játékosok célba érkezését, kezeli a mentést és az ellenségek leállítását.
/// </summary>
public class FinishManager : MonoBehaviour
{
    #region Konstansok

    private const int Player1Id = 1;
    private const int Player2Id = 2;
    private const float VictoryValidationDelay = 0.2f;

    #endregion

    #region Singleton

    public static FinishManager Instance;

    #endregion

    #region Inspector Mezők

    [Header("UI Panelek")]
    public GameObject levelCompletePanel;
    public GameObject HUDPanel;

    #endregion

    #region Privát Mezők - Állapot

    private bool hasPlayer1Finished = false;
    private bool hasPlayer2Finished = false;
    private bool isLevelComplete = false;
    private float levelStartTime;
    private Coroutine pendingVictoryValidationCoroutine;

    #endregion

    #region Publikus Property-k

    /// <summary>
    /// Visszaadja, hogy a pálya teljesítve van-e.
    /// </summary>
    public bool gameFinished => isLevelComplete;

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Singleton példány inicializálása.
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Időmérés indítása a pálya teljesítéséhez.
    /// </summary>
    void Start()
    {
        levelStartTime = Time.time;
    }

    #endregion

    #region Győzelem Detekció

    /// <summary>
    /// Játékos célba érkezésének kezelése.
    /// </summary>
    /// <param name="playerId">A játékos azonosítója (1 vagy 2).</param>
    public void PlayerReachedFinish(int playerId)
    {
        if (isLevelComplete)
            return;

        MarkPlayerAsFinished(playerId);

        if (HaveBothPlayersFinished())
        {
            StartVictoryValidation();
        }
    }

    /// <summary>
    /// Elindítja a késleltetett győzelem validációt.
    /// </summary>
    private void StartVictoryValidation()
    {
        StopPendingVictoryValidation();
        pendingVictoryValidationCoroutine = StartCoroutine(ValidateVictoryAfterDelay());
    }

    /// <summary>
    /// Késleltetés után újraellenőrzi, hogy mindkét játékos célon áll-e.
    /// </summary>
    private IEnumerator ValidateVictoryAfterDelay()
    {
        yield return new WaitForSeconds(VictoryValidationDelay);

        pendingVictoryValidationCoroutine = null;

        if (isLevelComplete)
            yield break;

        if (HaveBothPlayersFinished())
        {
            HandleLevelCompletion();
        }
    }

    /// <summary>
    /// Leállítja a függőben lévő győzelem validációt.
    /// </summary>
    private void StopPendingVictoryValidation()
    {
        if (pendingVictoryValidationCoroutine != null)
        {
            StopCoroutine(pendingVictoryValidationCoroutine);
            pendingVictoryValidationCoroutine = null;
        }
    }

    /// <summary>
    /// Megjelöli a játékost befejezettként.
    /// </summary>
    private void MarkPlayerAsFinished(int playerId)
    {
        if (playerId == Player1Id)
            hasPlayer1Finished = true;

        if (playerId == Player2Id)
            hasPlayer2Finished = true;
    }

    /// <summary>
    /// Ellenőrzi, hogy mindkét játékos befejezte-e a pályát.
    /// </summary>
    private bool HaveBothPlayersFinished()
    {
        return hasPlayer1Finished && hasPlayer2Finished;
    }

    /// <summary>
    /// Kezeli a pálya teljesítését (mentés, UI, ellenségek).
    /// </summary>
    private void HandleLevelCompletion()
    {
        PlayWinSFX();
        SaveLevelProgress();
        StopAllEnemies();

        if (ShouldContinueToNextLevel())
        {
            LoadNextChallengeLevel();
            return;
        }

        ShowVictoryScreen();
        isLevelComplete = true;
    }

    #endregion
    #region Effekt kezelés
    ///<summary>
    /// Elindítja a győzelem effektet
    /// </summary>
    private void PlayWinSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictorySFX();
        }
    }
    #endregion
    #region Mentés Kezelés

    /// <summary>
    /// Menti a pálya előrehaladását a játékmód alapján.
    /// </summary>
    private void SaveLevelProgress()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("Nincs GameDataManager, nem tudok menteni!");
            return;
        }

        string gamemode = LevelContext.Gamemode;

        if (gamemode == "normal")
        {
            SaveNormalModeProgress();
        }
        else if (gamemode == "dailychallenge")
        {
            SaveDailyChallengeProgress();
        }
    }

    /// <summary>
    /// Normal játékmód előrehaladásának mentése.
    /// </summary>
    private void SaveNormalModeProgress()
    {
        int currentLevel = LevelContext.CurrentLevelIndex;
        GameDataManager.Instance.LevelCompleted(currentLevel);
        Debug.Log($"Mentés indítása a(z) {currentLevel}. szintre");
    }

    /// <summary>
    /// Daily Challenge teljesítésének mentése idővel együtt.
    /// </summary>
    private void SaveDailyChallengeProgress()
    {
        float completionTime = CalculateCompletionTime();
        GameDataManager.Instance.RecordChallengeWin(false, completionTime);
        Debug.Log($"⏱️ Daily Challenge teljesítve {completionTime:F2} másodperc alatt!");
    }

    /// <summary>
    /// Kiszámítja a pálya teljesítési idejét.
    /// </summary>
    private float CalculateCompletionTime()
    {
        return Time.time - levelStartTime;
    }

    #endregion

    #region Ellenség Kezelés

    /// <summary>
    /// Leállítja az összes ellenséget a pályán.
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

    #endregion

    #region 1 Perces Kihívás Kezelés

    /// <summary>
    /// Ellenőrzi, hogy folytatni kell-e a következő pályával (1minchallenge).
    /// </summary>
    private bool ShouldContinueToNextLevel()
    {
        return LevelContext.Gamemode == "1minchallenge";
    }

    /// <summary>
    /// Betölti a következő pályát az 1 perces kihívásban.
    /// </summary>
    private void LoadNextChallengeLevel()
    {
        ResetFinishState();

        LevelGenerator levelGen = FindFirstObjectByType<LevelGenerator>();
        if (levelGen != null)
        {
            levelGen.OnChallengeComplete();
        }
    }

    /// <summary>
    /// Megjeleníti az 1 perces kihívás eredményeit.
    /// </summary>
    public void Show1MinChallengeResults(int levelsCompleted)
    {
        isLevelComplete = true;

        UpdateUIControllerWithResults(levelsCompleted);
        ShowVictoryScreen();

        Debug.Log($"🏆 1 perces kihívás vége! Teljesített pályák: {levelsCompleted}");
    }

    /// <summary>
    /// Frissíti a UI controllert az eredményekkel.
    /// </summary>
    private void UpdateUIControllerWithResults(int levelsCompleted)
    {
        if (GameSceneUIController.Instance != null)
        {
            GameSceneUIController.Instance.Show1MinChallengeResults(levelsCompleted);
        }
    }

    #endregion

    #region Állapot Kezelés

    /// <summary>
    /// Visszaállítja a finish állapotot a következő pályához.
    /// </summary>
    public void ResetFinishState()
    {
        StopPendingVictoryValidation();
        ResetPlayerFinishFlags();
        ResetLevelCompleteFlag();
        ResetPanels();
        ResetTimerIfNeeded();
    }

    /// <summary>
    /// Visszaállítja a játékosok finish flag-jeit.
    /// </summary>
    private void ResetPlayerFinishFlags()
    {
        hasPlayer1Finished = false;
        hasPlayer2Finished = false;
    }

    /// <summary>
    /// Visszaállítja a pálya teljesítés flag-et.
    /// </summary>
    private void ResetLevelCompleteFlag()
    {
        isLevelComplete = false;
    }

    /// <summary>
    /// Visszaállítja a UI paneleket.
    /// </summary>
    private void ResetPanels()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        if (HUDPanel != null)
        {
            HUDPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Visszaállítja az időmérőt 1 perces kihívás esetén.
    /// </summary>
    private void ResetTimerIfNeeded()
    {
        if (LevelContext.Gamemode == "1minchallenge")
        {
            levelStartTime = Time.time;
        }
    }

    #endregion

    #region UI Kezelés

    /// <summary>
    /// Megjeleníti a győzelmi képernyőt.
    /// </summary>
    private void ShowVictoryScreen()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        if (HUDPanel != null)
        {
            HUDPanel.SetActive(false);
        }
    }

    #endregion
}