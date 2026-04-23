using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// A fõmenü kezelését végzõ osztály.
/// Kezeli a gombok eseményeit, UI frissítését és scene váltásokat.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    #region Konstansok

    private const string DateFormat = "yyyy-MM-dd";
    private const string CoinDisplayFormat = "{0} $";
    private const string DefaultCoinText = "0";
    private const string NormalGamemode = "normal";
    private const string DailyChallengeGamemode = "dailychallenge";
    private const string OneMinuteChallengeGamemode = "1minchallenge";
    private const string DailyChallengesFileName = "dailychallenges";
    private const string LevelSelectSceneName = "LevelSelectScene";
    private const string GameSceneName = "GameScene";
    private const string SettingsSceneName = "SettingsScene";
    private const string AchievementsSceneName = "AchievementsScene";
    private const string LeaderboardSceneName = "LeaderboardScene";
    private const string ShopSceneName = "ShopScene";
    private const int InvalidLevelIndex = -1;

    #endregion

    #region Inspector Mezõk

    [Header("UI Elemek")]
    public TextMeshProUGUI coinText;
    public Button DailyButton;

    [Header("Web Linkek")]
    public string websiteUrl = "https://www.google.com";
    public string rateusUrl = "https://www.google.com";
    public string donateUrl = "https://www.google.com";

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Inicializálja a fõmenüt (gombok, UI frissítés).
    /// </summary>
    void Start()
    {
        InitializeMainMenu();
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Inicializálja a fõmenü elemeit.
    /// </summary>
    private void InitializeMainMenu()
    {
        SetDailyButtonInteraction();
        UpdateCoinUI();
    }

    #endregion

    #region Daily Challenge Ellenõrzés

    /// <summary>
    /// Beállítja a daily button interaktivitását a teljesítés alapján.
    /// </summary>
    public void SetDailyButtonInteraction()
    {
        if (HasPlayerCompletedDailyToday())
        {
            DisableDailyButton();
        }
        else
        {
            EnableDailyButton();
        }
    }

    /// <summary>
    /// Ellenõrzi, hogy a játékos teljesítette-e ma a daily challenge-et.
    /// </summary>
    private bool HasPlayerCompletedDailyToday()
    {
        if (!IsDailyChallengeDataValid())
        {
            return false;
        }

        string today = GetTodayDate();
        DailyChallengeRecord lastRecord = GetLastDailyChallengeRecord();

        return IsCompletedToday(lastRecord, today);
    }

    /// <summary>
    /// Ellenõrzi, hogy a daily challenge adatok érvényesek-e.
    /// </summary>
    private bool IsDailyChallengeDataValid()
    {
        return GameDataManager.Instance != null &&
               GameDataManager.Instance.currentProfile != null &&
               GameDataManager.Instance.currentProfile.dailyChallengesCompleted != null &&
               GameDataManager.Instance.currentProfile.dailyChallengesCompleted.Count > 0;
    }

    /// <summary>
    /// Visszaadja az utolsó daily challenge rekordot.
    /// </summary>
    private DailyChallengeRecord GetLastDailyChallengeRecord()
    {
        List<DailyChallengeRecord> records = GameDataManager.Instance.currentProfile.dailyChallengesCompleted;
        int lastIndex = records.Count - 1;
        return records[lastIndex];
    }

    /// <summary>
    /// Ellenõrzi, hogy a rekord ma lett-e teljesítve.
    /// </summary>
    private bool IsCompletedToday(DailyChallengeRecord record, string today)
    {
        return record.date == today;
    }

    /// <summary>
    /// Letiltja a daily button-t.
    /// </summary>
    private void DisableDailyButton()
    {
        Debug.Log("Mai napi kihívás már teljesítve!");
        DailyButton.interactable = false;
    }

    /// <summary>
    /// Engedélyezi a daily button-t.
    /// </summary>
    private void EnableDailyButton()
    {
        DailyButton.interactable = true;
    }

    #endregion

    #region UI Frissítés

    /// <summary>
    /// Frissíti a coin UI elemet az aktuális egyenleggel.
    /// </summary>
    public void UpdateCoinUI()
    {
        if (IsGameDataValid())
        {
            DisplayCurrentCoins();
        }
        else
        {
            DisplayDefaultCoins();
        }
    }

    /// <summary>
    /// Ellenõrzi, hogy a GameDataManager adatai érvényesek-e.
    /// </summary>
    private bool IsGameDataValid()
    {
        return GameDataManager.Instance != null &&
               GameDataManager.Instance.currentProfile != null;
    }

    /// <summary>
    /// Megjeleníti az aktuális coin egyenleget.
    /// </summary>
    private void DisplayCurrentCoins()
    {
        int currentCoins = GetCurrentCoins();
        SetCoinText(currentCoins);
    }

    /// <summary>
    /// Visszaadja az aktuális coin egyenleget.
    /// </summary>
    private int GetCurrentCoins()
    {
        return GameDataManager.Instance.currentProfile.coins;
    }

    /// <summary>
    /// Beállítja a coin szöveget a megadott értékkel.
    /// </summary>
    private void SetCoinText(int coins)
    {
        if (coinText != null)
        {
            coinText.text = string.Format(CoinDisplayFormat, coins);
        }
    }

    /// <summary>
    /// Megjeleníti az alapértelmezett coin értéket.
    /// </summary>
    private void DisplayDefaultCoins()
    {
        if (coinText != null)
        {
            coinText.text = DefaultCoinText;
        }
    }

    #endregion

    #region Gomb Események - Játékmódok

    /// <summary>
    /// Play gomb eseménykezelõje (Normal mód).
    /// </summary>
    public void OnPlayButtonPressed()
    {
        Debug.Log("Play Button Pressed");
        SetNormalGamemode();
        LoadLevelSelectScene();
        PlayButtonSFX();
    }

    /// <summary>
    /// 1 perces kihívás gomb eseménykezelõje.
    /// </summary>
    public void On1minChButtonPressed()
    {
        Debug.Log("1 min Button Pressed");
        SetOneMinuteChallengeGamemode();
        LoadGameScene();
        PlayButtonSFX();
    }

    /// <summary>
    /// Daily Challenge gomb eseménykezelõje.
    /// </summary>
    public void OnDailyChButtonPressed()
    {
        Debug.Log("Daily Button Pressed");
        PlayButtonSFX();

        if (HasPlayerCompletedDailyToday())
        {
            return;
        }

        int levelIndex = LoadRandomDailyChallengeLevel();

        if (IsValidLevelIndex(levelIndex))
        {
            SetDailyChallengeContext(levelIndex);
            LoadGameScene();
        }
    }

    /// <summary>
    /// Beállítja a normal játékmódot.
    /// </summary>
    private void SetNormalGamemode()
    {
        LevelContext.Gamemode = NormalGamemode;
    }

    /// <summary>
    /// Beállítja az 1 perces kihívás játékmódot.
    /// </summary>
    private void SetOneMinuteChallengeGamemode()
    {
        LevelContext.Gamemode = OneMinuteChallengeGamemode;
    }

    /// <summary>
    /// Beállítja a daily challenge kontextust.
    /// </summary>
    private void SetDailyChallengeContext(int levelIndex)
    {
        LevelContext.DailyChallengeIndex = levelIndex;
        LevelContext.Gamemode = DailyChallengeGamemode;
        Debug.Log($"Környezeti változó beállítva: {LevelContext.DailyChallengeIndex}");
    }

    #endregion

    #region JSON Betöltés

    /// <summary>
    /// Betölt egy random daily challenge pályát.
    /// </summary>
    private int LoadRandomDailyChallengeLevel()
    {
        return LoadLevelsFromJson(DailyChallengesFileName);
    }

    /// <summary>
    /// Betölti a pályákat JSON-bõl és visszaad egy random indexet.
    /// </summary>
    private int LoadLevelsFromJson(string jsonFileName)
    {
        TextAsset jsonFile = LoadJsonFile(jsonFileName);

        if (jsonFile == null)
        {
            return InvalidLevelIndex;
        }

        LevelListWrapper wrapper = ParseJsonFile(jsonFile);

        if (IsValidLevelWrapper(wrapper))
        {
            return GetRandomLevelIndex(wrapper.boards);
        }

        LogParsingError();
        return InvalidLevelIndex;
    }

    /// <summary>
    /// Betölti a JSON fájlt a Resources mappából.
    /// </summary>
    private TextAsset LoadJsonFile(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile == null)
        {
            LogJsonFileNotFound(fileName);
        }

        return jsonFile;
    }

    /// <summary>
    /// Parsolja a JSON fájlt LevelListWrapper objektummá.
    /// </summary>
    private LevelListWrapper ParseJsonFile(TextAsset jsonFile)
    {
        return JsonUtility.FromJson<LevelListWrapper>(jsonFile.text);
    }

    /// <summary>
    /// Ellenõrzi, hogy a level wrapper érvényes-e.
    /// </summary>
    private bool IsValidLevelWrapper(LevelListWrapper wrapper)
    {
        return wrapper != null && wrapper.boards != null && wrapper.boards.Count > 0;
    }

    /// <summary>
    /// Visszaad egy random pálya indexet.
    /// </summary>
    private int GetRandomLevelIndex(List<LevelRawData> levels)
    {
        int randomIndex = UnityEngine.Random.Range(0, levels.Count);
        LogLevelsLoaded(levels.Count);
        return randomIndex;
    }

    /// <summary>
    /// Ellenõrzi, hogy a pálya index érvényes-e.
    /// </summary>
    private bool IsValidLevelIndex(int index)
    {
        return index != InvalidLevelIndex;
    }

    /// <summary>
    /// Logolja a JSON fájl hiányát.
    /// </summary>
    private void LogJsonFileNotFound(string fileName)
    {
        Debug.LogError($"CRITICAL: Nem található a '{fileName}.json' a Resources mappában!");
    }

    /// <summary>
    /// Logolja a parsing hibát.
    /// </summary>
    private void LogParsingError()
    {
        Debug.LogError("Hiba a JSON parszolása közben (üres vagy hibás formátum).");
    }

    /// <summary>
    /// Logolja a betöltött pályák számát.
    /// </summary>
    private void LogLevelsLoaded(int count)
    {
        Debug.Log($"Sikeresen betöltve {count} szint a JSON-bõl.");
    }

    #endregion

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

    #region Gomb Események - Navigáció

    /// <summary>
    /// Settings gomb eseménykezelõje.
    /// </summary>
    public void OnSettingsButtonPressed()
    {
        PlayButtonSFX();
        Debug.Log("Settings Button Pressed");
        LoadSettingsScene();
    }

    /// <summary>
    /// Achievements gomb eseménykezelõje.
    /// </summary>
    public void OnAchivementsButtonPressed()
    {
        PlayButtonSFX();
        Debug.Log("Achivements Button Pressed");
        LoadAchievementsScene();
    }

    /// <summary>
    /// Leaderboard gomb eseménykezelõje.
    /// </summary>
    public void OnLeaderboardButtonPressed()
    {
        PlayButtonSFX();
        Debug.Log("Leaderboard Button Pressed");
        LoadLeaderboardScene();
    }

    /// <summary>
    /// Shop gomb eseménykezelõje.
    /// </summary>
    public void OnShopButtonPressed()
    {
        PlayButtonSFX();
        Debug.Log("Shop Button Pressed");
        LoadShopScene();
    }

    #endregion

    #region Gomb Események - Külsõ Linkek

    /// <summary>
    /// Website gomb eseménykezelõje.
    /// </summary>
    public void OnWebButtonPressed()
    {
        Debug.Log("Web Button Pressed");
        OpenWebsite();
    }

    /// <summary>
    /// Rate Us gomb eseménykezelõje.
    /// </summary>
    public void OnRateUsButtonPressed()
    {
        Debug.Log("Google Play Rate Button Pressed");
        OpenRateUsPage();
    }

    /// <summary>
    /// Donate gomb eseménykezelõje.
    /// </summary>
    public void OnDonateButtonPressed()
    {
        Debug.Log("Donate Button Pressed");
        OpenDonatePage();
    }

    /// <summary>
    /// Megnyitja a weboldalt.
    /// </summary>
    private void OpenWebsite()
    {
        Application.OpenURL(websiteUrl);
    }

    /// <summary>
    /// Megnyitja a Rate Us oldalt.
    /// </summary>
    private void OpenRateUsPage()
    {
        Application.OpenURL(rateusUrl);
    }

    /// <summary>
    /// Megnyitja a Donate oldalt.
    /// </summary>
    private void OpenDonatePage()
    {
        Application.OpenURL(donateUrl);
    }

    #endregion

    #region Scene Betöltés

    /// <summary>
    /// Betölti a Level Select scene-t.
    /// </summary>
    private void LoadLevelSelectScene()
    {
        SceneManager.LoadScene(LevelSelectSceneName);
    }

    /// <summary>
    /// Betölti a Game scene-t.
    /// </summary>
    private void LoadGameScene()
    {
        SceneManager.LoadScene(GameSceneName);
    }

    /// <summary>
    /// Betölti a Settings scene-t.
    /// </summary>
    private void LoadSettingsScene()
    {
        SceneManager.LoadScene(SettingsSceneName);
    }

    /// <summary>
    /// Betölti az Achievements scene-t.
    /// </summary>
    private void LoadAchievementsScene()
    {
        SceneManager.LoadScene(AchievementsSceneName);
    }

    /// <summary>
    /// Betölti a Leaderboard scene-t.
    /// </summary>
    private void LoadLeaderboardScene()
    {
        SceneManager.LoadScene(LeaderboardSceneName);
    }

    /// <summary>
    /// Betölti a Shop scene-t.
    /// </summary>
    private void LoadShopScene()
    {
        SceneManager.LoadScene(ShopSceneName);
    }

    #endregion

    #region Helper Metódusok

    /// <summary>
    /// Visszaadja a mai dátumot sztring formátumban.
    /// </summary>
    private string GetTodayDate()
    {
        return DateTime.Now.ToString(DateFormat);
    }

    #endregion
}
