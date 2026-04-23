using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ApiForGame;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

/// <summary>
/// Ranglista kezelését végző osztály.
/// Három kategóriát támogat: 1 Minute Challenge, Daily Challenge, Rank List.
/// </summary>
public class LeaderBoardManager : MonoBehaviour
{
    #region Konstansok

    private const string UIElementRank = "RankText";
    private const string UIElementPlayerName = "PlayerNameText";
    private const string UIElementScore = "ScoreText";
    private const string UIElementDate = "DateText"; // Ha létezik, kikapcsoljuk
    private const float TabActiveYPosition = -80f;
    private const float TabInactiveYPosition = 20f;
    private const float TabAnimationSpeed = 10f;
    private const string LoginRequiredMessage = "Kérlek jelentkezz be.";
    private const string EmptyCategoryMessage = "Még nincs adat ebben a kategóriában.";
    private const string ApiTemporarilyUnavailableMessage = "A ranglista szolgáltatás átmenetileg nem elérhető.";
    private const int ApiRetryDelaySeconds = 120;

    #endregion

    #region Enums

    /// <summary>
    /// Ranglista kategóriák.
    /// </summary>
    public enum LeaderboardCategory
    {
        OneMinuteChallenge = 0,
        DailyChallenge = 1,
        RankList = 2
    }

    #endregion

    #region Unity Inspector Mezők

    [Header("UI Konténerek")]
    public RectTransform contentContainer;
    public ScrollRect scrollRect;

    [Header("Prefabok")]
    public GameObject rowPrefab;
    public GameObject headerPrefab1min;
    public GameObject headerPrefabDaily;
    public GameObject headerPrefabRank;
    public GameObject messagePrefab;

    [Header("Tab Gombok")]
    public List<RectTransform> categoryButtons;

    [Header("Tab Animáció")]
    public float activeYPosition = TabActiveYPosition;
    public float inactiveYPosition = TabInactiveYPosition;
    public float animationSpeed = TabAnimationSpeed;

    #endregion

    #region Privát Mezők

    private readonly GameApi api = new GameApi();
    private LeaderboardCategory currentCategory = LeaderboardCategory.OneMinuteChallenge;
    private readonly List<LeaderboardEntry> currentLeaderboardData = new List<LeaderboardEntry>();
    private bool isLoading;
    private string emptyMessage = EmptyCategoryMessage;
    private DateTime nextApiAttemptUtc = DateTime.MinValue;

    #endregion

    #region Beágyazott JSON Osztályok

    /// <summary>
    /// Ranglista bejegyzés célzott adatstruktúra.
    /// </summary>
    public class LeaderboardEntry
    {
        public int rank;
        public string playerName;
        public string scoreDisplay;

        public LeaderboardEntry(int rank, string name, string display)
        {
            this.rank = rank;
            this.playerName = name;
            this.scoreDisplay = display;
        }
    }

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Inicializálja a ranglistát.
    /// </summary>
    async void Start()
    {
        await InitializeLeaderboardAsync();
    }

    /// <summary>
    /// Frissíti az aktív tab vizualizációját.
    /// </summary>
    void Update()
    {
        UpdateActiveTabHighlight();
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Inicializálja a ranglista összes elemét.
    /// </summary>
    private async Task InitializeLeaderboardAsync()
    {
        await LoadCategoryDataAsync(currentCategory);
        GenerateLeaderboardList();
        UpdateTabVisuals();
    }

    #endregion

    #region Kategória Váltás

    /// <summary>
    /// Vált a megadott kategóriára (tab gombokból hívva).
    /// </summary>
    public void SwitchToCategory(int categoryIndex)
    {
        LeaderboardCategory newCategory = (LeaderboardCategory)categoryIndex;

        if (newCategory != currentCategory && !isLoading)
        {
            _ = ChangeCategoryAsync(newCategory);
        }
    }

    /// <summary>
    /// Megváltoztatja az aktív kategóriát.
    /// </summary>
    private async Task ChangeCategoryAsync(LeaderboardCategory newCategory)
    {
        Debug.Log($"[LeaderBoard] Kategória váltás: {currentCategory} -> {newCategory}");

        currentCategory = newCategory;
        await LoadCategoryDataAsync(currentCategory);
        RegenerateList();
        UpdateTabVisuals();
    }

    /// <summary>
    /// Újragenerálja a ranglistát.
    /// </summary>
    private void RegenerateList()
    {
        ClearList();
        GenerateLeaderboardList();
    }

    #endregion

    #region Adat Betöltés

    /// <summary>
    /// Betölti a megadott kategória adatait API-ból.
    /// </summary>
    private async Task LoadCategoryDataAsync(LeaderboardCategory category)
    {
        Debug.Log($"[LeaderBoard] Adatok betöltése: {category}");

        isLoading = true;
        currentLeaderboardData.Clear();
        emptyMessage = EmptyCategoryMessage;

        if (!IsUserLoggedIn())
        {
            emptyMessage = LoginRequiredMessage;
            isLoading = false;
            return;
        }

        if (DateTime.UtcNow < nextApiAttemptUtc)
        {
            emptyMessage = ApiTemporarilyUnavailableMessage;
            isLoading = false;
            return;
        }

        try
        {
            switch (category)
            {
                case LeaderboardCategory.OneMinuteChallenge:
                    await LoadOneMinuteChallengeDataFromApiAsync();
                    break;
                case LeaderboardCategory.DailyChallenge:
                    await LoadDailyChallengeDataFromApiAsync();
                    break;
                case LeaderboardCategory.RankList:
                    await LoadRankListDataFromApiAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            nextApiAttemptUtc = DateTime.UtcNow.AddSeconds(ApiRetryDelaySeconds);
            Debug.LogError($"[LeaderBoard] Hiba az adatok betöltésekor: {ex.Message}");
            emptyMessage = ApiTemporarilyUnavailableMessage;
        }
        finally
        {
            isLoading = false;
        }

        Debug.Log($"[LeaderBoard] {currentLeaderboardData.Count} bejegyzés betöltve.");
    }

    private bool IsUserLoggedIn()
    {
        return AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn;
    }

    private bool HasRows(string response)
    {
        return !string.IsNullOrEmpty(response) &&
               (response.Contains("\"data\":[{") || response.Contains("\"result\":[{"));
    }

    private IEnumerable<JToken> GetRows(string response)
    {
        JObject root = JObject.Parse(response);
        return root["data"] ?? root["result"];
    }

    /// <summary>
    /// Betölti a 1 Minute Challenge ranglista adatokat API-ból (legtöbb pálya).
    /// </summary>
    private async Task LoadOneMinuteChallengeDataFromApiAsync()
    {
        string response = await api.SelectAllSpeedrunRecords();
        if (string.IsNullOrEmpty(response))
        {
            throw new Exception("SelectAllSpeedrunRecords nem elérhető.");
        }

        if (!HasRows(response))
        {
            return;
        }

        List<(string UserId, int Score)> rawEntries = new List<(string, int)>();
        foreach (var row in GetRows(response))
        {
            string uId = row?["user_id"]?.ToString();
            int score = (int)(row?["speedrun_amount"] ?? 0);

          
            if (!string.IsNullOrEmpty(uId) && score > 0)
            {
                rawEntries.Add((uId, score));
            }
        }

        var topRecords = rawEntries
            .GroupBy(x => x.UserId)
            .Select(g => new { UserId = g.Key, BestScore = g.Max(x => x.Score) })
            .OrderByDescending(x => x.BestScore)
            .Take(35)
            .ToList();

        var tasks = topRecords.Select(async (record, index) =>
        {
            string displayName = await FetchDisplayNameAsync(record.UserId);
            return new LeaderboardEntry(
                index + 1,
                displayName,
                FormatLevelScore(record.BestScore)
            );
        }).ToList();
        var resolvedEntries = await Task.WhenAll(tasks);
        foreach (var entry in resolvedEntries.OrderBy(x => x.rank))
        {
            currentLeaderboardData.Add(entry);
        }
    }

    /// <summary>
    /// Betölti a Daily Challenge adatokat API-ból a bejelentkezett felhasználóhoz.
    /// </summary>
    private async Task LoadDailyChallengeDataFromApiAsync()
    {
        string response = await api.SelectAllDailyCRecords();
        if (string.IsNullOrEmpty(response))
        {
            throw new Exception("SelectAllDailyCRecords nem elérhető.");
        }
        
        if (!HasRows(response))
        {
            return;
        }

        List<(string UserId, float Time)> rawEntries = new List<(string, float)>();

        foreach (var row in GetRows(response))
        {
            string uId = row?["user_id"]?.ToString();
            float time = (float)(row?["dailyc_time"] ?? 0f);

            
            if (!string.IsNullOrEmpty(uId) && time > 0)
            {
                rawEntries.Add((uId, time));
            }
        }

        
        var topRecords = rawEntries
            .GroupBy(x => x.UserId)
            .Select(g => new { UserId = g.Key, BestScore = g.Min(x => x.Time) })
            .OrderBy(x => x.BestScore)
            .Take(35)
            .ToList();

        
        var tasks = topRecords.Select(async (record, index) =>
        {
            string displayName = await FetchDisplayNameAsync(record.UserId);
            return new LeaderboardEntry(
                index + 1,
                displayName,
                FormatTimeScore(record.BestScore)
            );
        }).ToList();

       
        var resolvedEntries = await Task.WhenAll(tasks);

        
        foreach (var entry in resolvedEntries.OrderBy(x => x.rank))
        {
            currentLeaderboardData.Add(entry);
        }
    }

    /// <summary>
    /// RankList esetén is API adatokból épít (SelectAllSpeedrunRecords fallback).
    /// </summary>
    private async Task LoadRankListDataFromApiAsync()
    {
        string response = await api.SelectAllUserStats();
        if (string.IsNullOrEmpty(response))
        {
            throw new Exception("SelectAllUserStatsRecords nem elérhető.");
        }

        if (!HasRows(response))
        {
            return;
        }

        List<(string UserId, int Level)> rawEntries = new List<(string, int)>();

        foreach (var row in GetRows(response))
        {
            string uId = row?["user_id"]?.ToString();
            int level = (int)(row?["levels_completed"] ?? 0f);


            if (!string.IsNullOrEmpty(uId) && level > 0)
            {
                rawEntries.Add((uId, level));
            }
        }


        var topRecords = rawEntries
            .GroupBy(x => x.UserId)
            .Select(g => new { UserId = g.Key, BestScore = g.Max(x => x.Level) })
            .OrderByDescending(x => x.BestScore)
            .Take(35)
            .ToList();

        
        var tasks = topRecords.Select(async (record, index) =>
        {
            string displayName = await FetchDisplayNameAsync(record.UserId);
            return new LeaderboardEntry(
                index + 1,
                displayName,
                FormatLevelScore(record.BestScore)
            );
        }).ToList();


        var resolvedEntries = await Task.WhenAll(tasks);


        foreach (var entry in resolvedEntries.OrderBy(x => x.rank))
        {
            currentLeaderboardData.Add(entry);
        }
    }
    /// <summary>
    /// Lekérdezi a felhasználó megjelenítendő nevét a users táblából a user_id alapján.
    /// </summary>
    private async Task<string> FetchDisplayNameAsync(string userId)
    {
        try
        {
            string res = await api.SelectUser(userId, new { columns = "display_name" });
            if (HasRows(res))
            {
                JObject root = JObject.Parse(res);
                JToken data = root["data"]?[0] ?? root["result"]?[0];
                string name = data?["display_name"]?.ToString();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
            }
        }
        catch (Exception)
        {
            // Csendes hibakezelés, hogy egy hibás név miatt ne omoljon össze a lista
        }

        // Ha a saját magunk ID-ját látjuk, de valamiért nem jött le a név a szerverről
        if (GameDataManager.Instance != null && GameDataManager.Instance.currentProfile.userId == userId)
        {
            return GameDataManager.Instance.currentProfile.userName;
        }

        return "Unknown";
    }
    

    /// <summary>
    /// Formázza az időt megjelenítéshez.
    /// </summary>
    private string FormatTimeScore(float time)
    {
        return $"{time:F2}s";
    }

    /// <summary>
    /// Formázza a pályaszámot megjelenítéshez.
    /// </summary>
    private string FormatLevelScore(int levels)
    {
        return $"{levels}";
    }

    #endregion

    #region Lista Generálás

    /// <summary>
    /// Generálja a ranglista UI listát.
    /// </summary>
    private void GenerateLeaderboardList()
    {
        Debug.Log($"[LeaderBoard] Lista generálás kezdődik... ({currentLeaderboardData.Count} elem)");

        CreateCategoryHeader();

        if (currentLeaderboardData.Count == 0)
        {
            CreateEmptyMessage();
        }
        else
        {
            CreateLeaderboardRows();
        }
    }

    /// <summary>
    /// Törli a meglévő lista tartalmát.
    /// </summary>
    private void ClearList()
    {
        Debug.Log("[LeaderBoard] Lista törlése...");

        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Létrehozza a kategória header-t.
    /// </summary>
    private void CreateCategoryHeader()
    {
        GameObject prefabToUse = GetHeaderPrefabForCategory(currentCategory);

        if (prefabToUse != null)
        {
            Instantiate(prefabToUse, contentContainer);
        }
    }

    /// <summary>
    /// Visszaadja a megfelelő header prefabot a kategória alapján.
    /// </summary>
    private GameObject GetHeaderPrefabForCategory(LeaderboardCategory category)
    {
        switch (category)
        {
            case LeaderboardCategory.OneMinuteChallenge:
                return headerPrefab1min;
            case LeaderboardCategory.DailyChallenge:
                return headerPrefabDaily;
            case LeaderboardCategory.RankList:
                return headerPrefabRank;
            default:
                return headerPrefabRank;
        }
    }

    /// <summary>
    /// Létrehoz egy üres üzenetet ha nincs adat.
    /// </summary>
    private void CreateEmptyMessage()
    {
        GameObject emptyRow = Instantiate(messagePrefab, contentContainer);
        TMP_Text messageText = emptyRow.GetComponentInChildren<TMP_Text>();

        if (messageText != null)
        {
            messageText.text = emptyMessage;
        }
    }

    /// <summary>
    /// Létrehozza a ranglista sorait.
    /// </summary>
    private void CreateLeaderboardRows()
    {
        foreach (var entry in currentLeaderboardData)
        {
            CreateLeaderboardRow(entry);
        }
    }

    /// <summary>
    /// Létrehoz egy ranglista sort.
    /// </summary>
    private void CreateLeaderboardRow(LeaderboardEntry entry)
    {
        GameObject row = InstantiateRow();
        SetupRowData(row, entry);
    }

    /// <summary>
    /// Példányosítja a sor prefab-ot.
    /// </summary>
    private GameObject InstantiateRow()
    {
        return Instantiate(rowPrefab, contentContainer);
    }

    /// <summary>
    /// Beállítja a sor adatait.
    /// </summary>
    private void SetupRowData(GameObject row, LeaderboardEntry entry)
    {
        bool rankSet = SetRowRank(row, entry.rank);
        bool nameSet = SetRowPlayerName(row, entry.playerName);
        bool scoreSet = SetRowScore(row, entry.scoreDisplay);

        if (!rankSet || !nameSet || !scoreSet)
        {
            ApplyFallbackRowBinding(row, entry);
        }

        DisableRowDate(row);
    }

    private void ApplyFallbackRowBinding(GameObject row, LeaderboardEntry entry)
    {
        TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>(true);

        if (texts.Length > 0)
        {
            texts[0].text = $"#{entry.rank}";
        }

        if (texts.Length > 1)
        {
            texts[1].text = entry.scoreDisplay;
        }

        if (texts.Length > 2)
        {
            texts[2].text = entry.playerName;
        }
    }

    /// <summary>
    /// Beállítja a helyezés szövegét.
    /// </summary>
    private bool SetRowRank(GameObject row, int rank)
    {
        Transform rankObj = row.transform.Find(UIElementRank);
        if (rankObj != null)
        {
            TMP_Text rankText = rankObj.GetComponent<TMP_Text>();
            if (rankText != null)
            {
                rankText.text = $"#{rank}";
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Beállítja a játékos nevét.
    /// </summary>
    private bool SetRowPlayerName(GameObject row, string playerName)
    {
        Transform nameObj = row.transform.Find(UIElementPlayerName);
        if (nameObj != null)
        {
            TMP_Text nameText = nameObj.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = playerName;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Beállítja az eredmény szövegét (idő/pálya).
    /// </summary>
    private bool SetRowScore(GameObject row, string scoreDisplay)
    {
        Transform scoreObj = row.transform.Find(UIElementScore);
        if (scoreObj != null)
        {
            TMP_Text scoreText = scoreObj.GetComponent<TMP_Text>();
            if (scoreText != null)
            {
                scoreText.text = scoreDisplay;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Eltünteti a dátumot, ha még a prefab-ban benne van.
    /// </summary>
    private void DisableRowDate(GameObject row)
    {
        Transform dateObj = row.transform.Find(UIElementDate);
        if (dateObj != null)
        {
            dateObj.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Tab Vizualizáció

    /// <summary>
    /// Frissíti az aktív tab kijelzését.
    /// </summary>
    private void UpdateActiveTabHighlight()
    {
        if (categoryButtons == null || categoryButtons.Count == 0)
            return;

        UpdateTabVisuals();
    }

    /// <summary>
    /// Frissíti a tab gombok vizuális megjelenését.
    /// </summary>
    private void UpdateTabVisuals()
    {
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            if (categoryButtons[i] != null)
            {
                UpdateSingleTab(categoryButtons[i], i);
            }
        }
    }

    /// <summary>
    /// Frissíti egy tab vizuális megjelenését.
    /// </summary>
    private void UpdateSingleTab(RectTransform tabRect, int tabIndex)
    {
        bool isActive = tabIndex == (int)currentCategory;
        float targetY = isActive ? activeYPosition : inactiveYPosition;
        AnimateTabToPosition(tabRect, targetY);
    }

    /// <summary>
    /// Animálja a tab-ot a cél pozícióhoz.
    /// </summary>
    private void AnimateTabToPosition(RectTransform tabRect, float targetY)
    {
        Vector2 currentPos = tabRect.anchoredPosition;
        float newY = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * animationSpeed);
        tabRect.anchoredPosition = new Vector2(currentPos.x, newY);
    }

    #endregion
}
