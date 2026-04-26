using ApiForGame;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Játékos adatok kezelését végző központi osztály.
/// Kezeli a mentést/betöltést (helyi és felhő), progressziót, pénzt és statisztikákat.
/// </summary>
public class GameDataManager : MonoBehaviour
{
    #region Konstansok

    private const string SaveFileName = "playerdata.json";
    private const string ShopItemsSaveFileName = "shopitems_save.json";
    private const string AchievementsSaveFileName = "achievements_save.json";
    private const string ShopItemsResourcePath = "shopitems";
    private const string AchievementsResourcePath = "achievements";
    private const string DateFormat = "yyyy-MM-dd";
    private const string GuestUserName = "Guest";
    private const string TempUserId = "";
    private const int MinimumCoins = 0;
    private const int CloudSyncRetryDelaySeconds = 120;
    private readonly GameApi api = new GameApi();
    private bool isCloudSyncInProgress;
    private DateTime nextCloudSyncAttemptUtc = DateTime.MinValue;
    #endregion

    #region Singleton

    public static GameDataManager Instance;

    #endregion

    #region Publikus Mezők

    public UserProfileData currentProfile;

    #endregion

    #region Events

    public Action OnCoinBalanceChanged;
    public Action OnStatsUpdated;
    public Action OnProfileRestored;
    public Action<List<int>> OnShopItemsRestored;
    public Action<Dictionary<int, int>> OnAchievementsRestored;

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Singleton inicializálása és DontDestroyOnLoad beállítása.
    /// </summary>
    void Awake()
    {
        InitializeSingleton();
    }

    /// <summary>
    /// Képernyő orientáció beállítása.
    /// </summary>
    void Start()
    {
        SetScreenOrientation();
    }

    #endregion

    #region Singleton Kezelés

    /// <summary>
    /// Inicializálja a singleton példányt és beállítja a perzisztenciát.
    /// </summary>
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameDataManager] Singleton létrehozva és DontDestroyOnLoad beállítva.");
        }
        else
        {
            Debug.LogWarning("[GameDataManager] Duplikált példány észlelve. Törlés...");
            Destroy(gameObject);
        }
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Beállítja a képernyő orientációját portrait módra.
    /// </summary>
    private void SetScreenOrientation()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    #endregion

    #region Login Nyilvántartás

    /// <summary>
    /// Rögzíti a mai bejelentkezést ha még nem történt meg.
    /// </summary>
    public void RecordLogin()
    {
        if (!IsProfileValid())
            return;

        string today = GetTodayDate();

        if (IsNewLoginDay(today))
        {
            AddLoginDate(today);
            SaveAndNotifyStatsUpdate();
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy ma még nem történt-e bejelentkezés.
    /// </summary>
    private bool IsNewLoginDay(string date)
    {
        return !currentProfile.loginDates.Contains(date);
    }

    /// <summary>
    /// Hozzáadja a bejelentkezési dátumot a listához.
    /// </summary>
    private void AddLoginDate(string date)
    {
        currentProfile.loginDates.Add(date);
        int totalDays = currentProfile.loginDates.Count;
        Debug.Log($"[GameDataManager] Új nap rögzítve: {date}. Összesen: {totalDays} nap.");
    }

    #endregion

    #region Pálya Történet Nyilvántartás

    /// <summary>
    /// Rögzít egy pálya kísérletet a történetbe.
    /// </summary>
    public void RecordLevelAttempt(int levelIndex, float duration, LevelResult result)
    {
        if (!IsProfileValid())
            return;

        LevelAttempt attempt = CreateLevelAttempt(levelIndex, duration, result);
        AddAttemptToHistory(attempt);
        SaveAndNotifyStatsUpdate();
    }

    /// <summary>
    /// Létrehoz egy új pálya kísérlet objektumot.
    /// </summary>
    private LevelAttempt CreateLevelAttempt(int levelIndex, float duration, LevelResult result)
    {
        return new LevelAttempt(levelIndex, duration, result);
    }

    /// <summary>
    /// Hozzáad egy kísérletet a történethez.
    /// </summary>
    private void AddAttemptToHistory(LevelAttempt attempt)
    {
        currentProfile.levelHistory.Add(attempt);
        Debug.Log($"[GameDataManager] Pálya naplózva: Szint {attempt.levelIndex}, Idő: {attempt.duration}s, Eredmény: {attempt.result}");
    }

    #endregion

    #region Pénz Kezelés

    /// <summary>
    /// Hozzáad vagy levon pénzt a játékos egyenlegéből.
    /// </summary>
    public void AddCoins(int amount)
    {
        if (!IsProfileValid())
            return;

        UpdateCoinBalance(amount);
        ClampCoinsToMinimum();
        LogCoinChange();
        SaveAndNotifyCoinChange();
    }

    /// <summary>
    /// Frissíti a pénz egyenleget.
    /// </summary>
    private void UpdateCoinBalance(int amount)
    {
        currentProfile.coins += amount;
    }

    /// <summary>
    /// Biztosítja, hogy a pénz ne legyen negatív.
    /// </summary>
    private void ClampCoinsToMinimum()
    {
        if (currentProfile.coins < MinimumCoins)
        {
            currentProfile.coins = MinimumCoins;
        }
    }

    /// <summary>
    /// Logolja a pénz változást.
    /// </summary>
    private void LogCoinChange()
    {
        Debug.Log($"[GameDataManager] Pénz változott. Új egyenleg: {currentProfile.coins}");
    }

    /// <summary>
    /// Visszaadja a játékos jelenlegi pénzegyenlegét.
    /// </summary>
    public int GetCoins()
    {
        if (!IsProfileValid())
            return MinimumCoins;

        return currentProfile.coins;
    }

    #endregion

    #region Progresszió Kezelés

    /// <summary>
    /// Rögzíti egy pálya teljesítését és frissíti a progressziót.
    /// </summary>
    public void LevelCompleted(int finishedLevelIndex)
    {
        if (!IsProfileValid())
            return;

        int nextLevelIndex = CalculateNextLevelIndex(finishedLevelIndex);

        if (IsNewProgressLevel(nextLevelIndex))
        {
            UpdateMaxLevelReached(nextLevelIndex);
            SaveGame();
        }
    }

    /// <summary>
    /// Kiszámítja a következő pálya indexét.
    /// </summary>
    private int CalculateNextLevelIndex(int currentLevelIndex)
    {
        return currentLevelIndex + 1;
    }

    /// <summary>
    /// Ellenőrzi, hogy új progressziós szintet ért-e el.
    /// </summary>
    private bool IsNewProgressLevel(int levelIndex)
    {
        return levelIndex > currentProfile.maxLevelReached;
    }

    /// <summary>
    /// Frissíti a maximálisan elért szintet.
    /// </summary>
    private void UpdateMaxLevelReached(int levelIndex)
    {
        currentProfile.maxLevelReached = levelIndex;
        Debug.Log($"[GameDataManager] Helyi progressz frissítve: {levelIndex}");
    }

    #endregion

    #region Challenge Nyilvántartás

    /// <summary>
    /// Rögzíti egy challenge teljesítését (daily vagy 1 perces).
    /// </summary>
    public void RecordChallengeWin(bool isOneMinuteChallenge, float completionTime = 0f, int levelsCompleted = 0)
    {
        if (!IsProfileValid())
            return;

        string today = GetTodayDate();

        if (isOneMinuteChallenge)
        {
            RecordOneMinuteChallenge(today, levelsCompleted);
        }
        else
        {
            RecordDailyChallenge(today, completionTime);
        }

        SaveAndNotifyStatsUpdate();
    }

    /// <summary>
    /// Rögzíti a daily challenge teljesítését.
    /// </summary>
    private void RecordDailyChallenge(string date, float completionTime)
    {
        if (IsDailyChallengeCompletedToday(date))
        {
            LogDailyChallengeAlreadyCompleted(date);
            return;
        }

        AddDailyChallengeRecord(date, completionTime);
    }

    /// <summary>
    /// Ellenőrzi, hogy ma már teljesítve van-e a daily challenge.
    /// </summary>
    private bool IsDailyChallengeCompletedToday(string date)
    {
        foreach (var record in currentProfile.dailyChallengesCompleted)
        {
            if (record.date == date)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Logolja, hogy a daily challenge már teljesítve van ma.
    /// </summary>
    private void LogDailyChallengeAlreadyCompleted(string date)
    {
        Debug.LogWarning($"[GameDataManager] Daily Challenge már teljesítve ma: {date}");
    }

    /// <summary>
    /// Hozzáad egy új daily challenge rekordot.
    /// </summary>
    private void AddDailyChallengeRecord(string date, float completionTime)
    {
        DailyChallengeRecord newRecord = new DailyChallengeRecord(date, completionTime);
        currentProfile.dailyChallengesCompleted.Add(newRecord);
        Debug.Log($"[GameDataManager] Daily Challenge teljesítve. Dátum: {date}, Idő: {completionTime:F2}s");
    }

    /// <summary>
    /// Rögzíti az 1 perces challenge teljesítését.
    /// </summary>
    private void RecordOneMinuteChallenge(string date, int levelsCompleted)
    {
        OneMinChallengeRecord newRecord = new OneMinChallengeRecord(levelsCompleted);
        currentProfile.oneMinChallengesCompleted.Add(newRecord);
        Debug.Log($"[GameDataManager] 1 Minute Challenge teljesítve. Dátum: {date}, Pályák: {levelsCompleted}");
    }

    #endregion

    #region Mentés/Betöltés - Helyi

    /// <summary>
    /// Menti a játékállást (helyi és felhő).
    /// </summary>
    public void SaveGame()
    {
        SaveLocalData();

        if (IsLoggedIn())
        {
            SyncDataToCloud();
        }
    }
   

    /// <summary>
    /// Menti az adatokat helyi fájlba.
    /// </summary>
    private void SaveLocalData()
    {
        Debug.Log("[GameDataManager] SaveLocalData() hívva.");
        Debug.Log($"[GameDataManager] Mentendő adatok - userName: {currentProfile?.userName}, coins: {currentProfile?.coins}, maxLevel: {currentProfile?.maxLevelReached}");

        string json = SerializeProfile();
        Debug.Log($"[GameDataManager] Szerializált JSON: {json}");

        string path = GetSaveFilePath();
        Debug.Log($"[GameDataManager] Mentés helye: {path}");

        WriteJsonToFile(path, json);
        LogSaveSuccess(path);
    }

    /// <summary>
    /// Serializálja a profilt JSON formátumra.
    /// </summary>
    private string SerializeProfile()
    {
        return JsonUtility.ToJson(currentProfile);
    }

    /// <summary>
    /// Visszaadja a mentési fájl teljes elérési útját.
    /// </summary>
    private string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    /// <summary>
    /// Kiírja a JSON tartalmat fájlba.
    /// </summary>
    private void WriteJsonToFile(string path, string json)
    {
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Logolja a sikeres mentést.
    /// </summary>
    private void LogSaveSuccess(string path)
    {
        Debug.Log("[GameDataManager] Helyi mentés sikeres: " + path);
    }

    /// <summary>
    /// Betölti a helyi mentett adatokat.
    /// </summary>
    public void LoadLocalData()
    {
        Debug.Log("[GameDataManager] LoadLocalData() hívva.");

        string path = GetSaveFilePath();
        Debug.Log($"[GameDataManager] Mentési fájl elérési útja: {path}");
        Debug.Log("[GameDataManager] Fájl betöltése megkezdődik.");

        LoadProfileFromFile(path);
        ValidateLoadedProfile();
    }

    /// <summary>
    /// Betölti a profilt a fájlból.
    /// </summary>
    private void LoadProfileFromFile(string path)
    {
        try
        {
            Debug.Log($"[GameDataManager] Fájl beolvasása: {path}");
            string json = File.ReadAllText(path);
            Debug.Log($"[GameDataManager] JSON tartalom: {json}");

            currentProfile = JsonUtility.FromJson<UserProfileData>(json);

            Debug.Log("[GameDataManager] JSON deserializáció sikeres.");
            Debug.Log($"[GameDataManager] Betöltött adatok - UserName: {currentProfile?.userName}, Coins: {currentProfile?.coins}, MaxLevel: {currentProfile?.maxLevelReached}");

            LogProfileLoaded();
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning($"[GameDataManager] Fájl nem található: {path}");
            Debug.LogWarning("[GameDataManager] Guest profil létrehozása.");
            CreateGuestProfile();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GameDataManager] Hiba a profil betöltésekor: {ex.Message}");
            Debug.LogError($"[GameDataManager] Exception típus: {ex.GetType().Name}");
            Debug.LogError($"[GameDataManager] StackTrace: {ex.StackTrace}");
            Debug.LogWarning("[GameDataManager] Guest profil létrehozása hiba miatt.");
            CreateGuestProfile();
        }
    }

    /// <summary>
    /// Validálja a betöltött profilt és javítja a null értékeket.
    /// </summary>
    private void ValidateLoadedProfile()
    {
        if (currentProfile == null)
        {
            CreateGuestProfile();
            return;
        }

        // Null-check és javítás a listákra
        if (currentProfile.loginDates == null)
            currentProfile.loginDates = new System.Collections.Generic.List<string>();

        if (currentProfile.levelHistory == null)
            currentProfile.levelHistory = new System.Collections.Generic.List<LevelAttempt>();

        if (currentProfile.dailyChallengesCompleted == null)
            currentProfile.dailyChallengesCompleted = new System.Collections.Generic.List<DailyChallengeRecord>();

        if (currentProfile.oneMinChallengesCompleted == null)
            currentProfile.oneMinChallengesCompleted = new System.Collections.Generic.List<OneMinChallengeRecord>();

        // String értékek javítása
        if (string.IsNullOrEmpty(currentProfile.language))
            currentProfile.language = "en";

        if (string.IsNullOrEmpty(currentProfile.userName))
            currentProfile.userName = GuestUserName;

        if (string.IsNullOrEmpty(currentProfile.userId))
            currentProfile.userId = TempUserId;

        Debug.Log($"[GameDataManager] Profil validálva: {currentProfile.userName}, Coins: {currentProfile.coins}");
    }

    /// <summary>
    /// Logolja a profil betöltését.
    /// </summary>
    private void LogProfileLoaded()
    {
        Debug.Log($"[GameDataManager] Helyi profil betöltve: {currentProfile.userName} (Szint: {currentProfile.maxLevelReached}, Coins: {currentProfile.coins})");
    }

    /// <summary>
    /// Létrehoz egy üres guest profilt.
    /// </summary>
    private void CreateGuestProfile()
    {
        Debug.Log("[GameDataManager] CreateGuestProfile() hívva.");
        Debug.Log($"[GameDataManager] Új profil létrehozása: userId='{TempUserId}', userName='{GuestUserName}'");

        currentProfile = new UserProfileData(TempUserId, GuestUserName);

        Debug.Log("[GameDataManager] Guest profil létrehozva.");
        Debug.Log($"[GameDataManager] Guest profil adatok - userName: {currentProfile.userName}, coins: {currentProfile.coins}, maxLevel: {currentProfile.maxLevelReached}");
    }

    #endregion

    #region Mentés/Betöltés - Felhő

    /// <summary>
    /// Szinkronizálja az adatokat a felhővel ("Local-First" logikával).
    /// </summary>
    public async void SyncDataToCloud()
    {
        if (!IsLoggedIn()) return;

        if (isCloudSyncInProgress)
        {
            Debug.LogWarning("[GameDataManager][Cloud] Szinkron már folyamatban van, új kérés kihagyva.");
            return;
        }

        if (DateTime.UtcNow < nextCloudSyncAttemptUtc)
        {
            Debug.LogWarning("[GameDataManager][Cloud] Szinkron ideiglenesen tiltva (retry cooldown).");
            return;
        }

        isCloudSyncInProgress = true;

        Debug.Log("[GameDataManager][Cloud] Szinkronizáció indítása.");
        
        string localUserId = currentProfile.userId;
        string userName = currentProfile.userName;
        string googleId = "";

#if UNITY_EDITOR
        googleId = "Mock_Google_ID_12345";
#else
        if (AuthManager.Instance.CurrentUser != null)
        {
            googleId = AuthManager.Instance.CurrentUser.UserId;
        }
#endif

        if (string.IsNullOrEmpty(googleId))
        {
            Debug.LogWarning("[GameDataManager][Cloud] Google ID üres. API szinkron kihagyva.");
            return;
        }

        int localCoins = currentProfile.coins;
        int localLevel = currentProfile.maxLevelReached;

        bool HasRows(string response)
        {
            return !string.IsNullOrEmpty(response) &&
                   (response.Contains("\"data\":[{") || response.Contains("\"result\":[{"));
        }

        string EnsureApiResponse(string response, string operation)
        {
            if (string.IsNullOrEmpty(response))
            {
                throw new Exception($"API nem elérhető: {operation}");
            }

            return response;
        }

        async System.Threading.Tasks.Task<bool> UserExistsAsync(string userId)
        {
            string userRes = await api.SelectUser(userId, new { columns = "user_id" });
            EnsureApiResponse(userRes, "SelectUser");
            return HasRows(userRes);
        }

        async System.Threading.Tasks.Task<string> GenerateUniqueUserIdAsync(string preferredId)
        {
            if (!string.IsNullOrEmpty(preferredId) && !await UserExistsAsync(preferredId))
            {
                return preferredId;
            }

            string candidate;
            do
            {
                candidate = Guid.NewGuid().ToString();
            }
            while (await UserExistsAsync(candidate));

            return candidate;
        }

        async System.Threading.Tasks.Task<List<int>> GetCloudShopItemIdsAsync(string userId)
        {
            List<int> cloudItemIds = new List<int>();
            string shopRes = await api.SelectShopRecord(userId, new { columns = "item_number" });
            EnsureApiResponse(shopRes, "SelectShopRecord");

            if (!HasRows(shopRes))
            {
                return cloudItemIds;
            }

            JObject shopRoot = JObject.Parse(shopRes);
            JToken shopDataArray = shopRoot["data"] ?? shopRoot["result"];
            if (shopDataArray == null)
            {
                return cloudItemIds;
            }

            foreach (var itemToken in shopDataArray)
            {
                int itemNum = (int)(itemToken["item_number"] ?? -1);
                if (itemNum != -1)
                {
                    cloudItemIds.Add(itemNum);
                }
            }

            return cloudItemIds;
        }

        async System.Threading.Tasks.Task<Dictionary<int, int>> GetCloudAchievementStatesAsync(string userId)
        {
            Dictionary<int, int> cloudAchievements = new Dictionary<int, int>();
            string achRes = await api.SelectAchievementRecord(userId, new { columns = "achievement_id, status" });
            EnsureApiResponse(achRes, "SelectAchievementRecord");

            if (!HasRows(achRes))
            {
                return cloudAchievements;
            }

            JObject achRoot = JObject.Parse(achRes);
            JToken achDataArray = achRoot["data"] ?? achRoot["result"];
            if (achDataArray == null)
            {
                return cloudAchievements;
            }

            foreach (var achToken in achDataArray)
            {
                int achId = (int)(achToken["achievement_id"] ?? -1);
                int status = (int)(achToken["status"] ?? 0);

                if (achId < 0)
                {
                    continue;
                }

                if (!cloudAchievements.ContainsKey(achId) || cloudAchievements[achId] < status)
                {
                    cloudAchievements[achId] = status;
                }
            }

            return cloudAchievements;
        }

        async System.Threading.Tasks.Task UploadLocalAchievementsAsync(string userIdToSave)
        {
            Dictionary<int, int> cloudAchievements = await GetCloudAchievementStatesAsync(userIdToSave);
            List<AchievementData> localAchievements = GetLocalAchievements();

            if (localAchievements == null)
            {
                return;
            }

            foreach (var achievement in localAchievements)
            {
                int localStatus;
                if (achievement.state == AchievementState.Unlocked)
                {
                    localStatus = 1;
                }
                else if (achievement.state == AchievementState.Collected)
                {
                    localStatus = 2;
                }
                else
                {
                    continue;
                }

                if (!cloudAchievements.TryGetValue(achievement.id, out int cloudStatus))
                {
                    await api.InsertAchievementRecord(userIdToSave, new
                    {
                        achievement_id = achievement.id,
                        status = localStatus
                    });
                }
                else if (cloudStatus < localStatus)
                {
                    await api.UpdateAchievementRecord(userIdToSave, new
                    {
                        achievement_id = achievement.id,
                        status = localStatus
                    });
                }
            }
        }

        async System.Threading.Tasks.Task UploadLocalStateAsync(string userIdToSave)
        {
            await api.UpdateUser(userIdToSave, new { google_id = googleId, display_name = userName });
            await api.UpdateUserStat(userIdToSave, new
            {
                coins = currentProfile.coins,
                levels_completed = currentProfile.maxLevelReached,
                best_speedrun_amount = GetBestSpeedrunAmount(),
                best_dailyc_time = GetBestDailyTime()
            });
            ////
            await api.UpdateDailyCRecord(localUserId, new
            {
                dailyc_time = GetBestDailyTime(),
                date = DateTime.Now.ToString("yyyy-MM-dd")
            });
            await api.UpdateSpeedrunRecord(localUserId, new
            {
                speedrun_amount = GetBestSpeedrunAmount(),
                date = DateTime.Now.ToString("yyyy-MM-dd")
            });
            ////
            List<int> cloudItemIds = await GetCloudShopItemIdsAsync(userIdToSave);
            List<ShopItemData> localShopItems = GetLocalShopItems();

            if (localShopItems != null)
            {
                foreach (var item in localShopItems)
                {
                    if ((int)item.state > 0 && !cloudItemIds.Contains(item.id))
                    {
                        Debug.Log($"[GameDataManager][Cloud] Hiányzó megvásárolt tárgy feltöltése (ID: {item.id}).");
                        await api.InsertShopRecord(userIdToSave, item.id);
                    }
                }
            }

            await UploadLocalAchievementsAsync(userIdToSave);
        }

        async System.Threading.Tasks.Task RestoreLocalFromCloudAsync(string cloudUserId, string cloudDisplayName)
        {
            currentProfile.userId = cloudUserId;

            if (!string.IsNullOrEmpty(cloudDisplayName))
            {
                currentProfile.userName = cloudDisplayName;
                userName = cloudDisplayName;
            }

            string statRes = await api.SelectUserStat(cloudUserId, new { columns = "coins, levels_completed, best_speedrun_amount, best_dailyc_time" });
            EnsureApiResponse(statRes, "SelectUserStat");
            if (HasRows(statRes))
            {
                JObject statRoot = JObject.Parse(statRes);
                JToken statData = statRoot["data"]?[0] ?? statRoot["result"]?[0];
                if (statData != null)
                {
                    currentProfile.coins = (int)(statData["coins"] ?? currentProfile.coins);
                    currentProfile.maxLevelReached = (int)(statData["levels_completed"] ?? currentProfile.maxLevelReached);
                    string yesterdayDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

                    if (statData["best_speedrun_amount"] != null)
                    {
                        int bestSpeedrun = (int)(statData["best_speedrun_amount"] ?? 0f);
                        currentProfile.oneMinChallengesCompleted.Add(new OneMinChallengeRecord(bestSpeedrun));
                    }

                    if (statData["best_dailyc_time"] != null)
                    {
                        float bestDailyTime = (float)(statData["best_dailyc_time"] ?? 0f);
                        currentProfile.dailyChallengesCompleted.Add(new DailyChallengeRecord(yesterdayDate, bestDailyTime));
                    }
                }
            }

            List<int> cloudItemIds = await GetCloudShopItemIdsAsync(cloudUserId);
            Dictionary<int, int> cloudAchievementStates = await GetCloudAchievementStatesAsync(cloudUserId);

            try
            {
                string shopSavePath = Path.Combine(Application.persistentDataPath, ShopItemsSaveFileName);
                ShopItemListWrapper localWrapper = null;

                if (File.Exists(shopSavePath))
                {
                    string savedJson = File.ReadAllText(shopSavePath);
                    localWrapper = JsonUtility.FromJson<ShopItemListWrapper>(savedJson);
                }
                else
                {
                    TextAsset defaultShopJson = Resources.Load<TextAsset>(ShopItemsResourcePath);
                    if (defaultShopJson != null)
                    {
                        localWrapper = JsonUtility.FromJson<ShopItemListWrapper>(defaultShopJson.text);
                    }
                }

                if (localWrapper != null && localWrapper.list != null)
                {
                    HashSet<int> cloudItemIdSet = new HashSet<int>(cloudItemIds);
                    bool changed = false;

                    foreach (var item in localWrapper.list)
                    {
                        if (cloudItemIdSet.Contains(item.id) && (int)item.state != 1 && item.state !=(ShopItemState)2)
                        {
                            item.state = (ShopItemState)1;
                            changed = true;
                        }
                    }

                    if (changed || !File.Exists(shopSavePath))
                    {
                        File.WriteAllText(shopSavePath, JsonUtility.ToJson(localWrapper, true));
                    }

                    ShopManager shopManager = FindObjectOfType<ShopManager>();
                    if (shopManager != null && shopManager.ShopItems != null)
                    {
                        foreach (var item in shopManager.ShopItems)
                        {
                            if (cloudItemIdSet.Contains(item.id))
                            {
                                item.state = (ShopItemState)1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[GameDataManager][Cloud] Shop state restore sikertelen: {ex.Message}");
            }

            try
            {
                string achievementsSavePath = Path.Combine(Application.persistentDataPath, AchievementsSaveFileName);
                AchievementListWrapper localAchievementsWrapper = null;

                if (File.Exists(achievementsSavePath))
                {
                    string savedAchievementsJson = File.ReadAllText(achievementsSavePath);
                    localAchievementsWrapper = JsonUtility.FromJson<AchievementListWrapper>(savedAchievementsJson);
                }
                else
                {
                    TextAsset defaultAchievementsJson = Resources.Load<TextAsset>(AchievementsResourcePath);
                    if (defaultAchievementsJson != null)
                    {
                        localAchievementsWrapper = JsonUtility.FromJson<AchievementListWrapper>(defaultAchievementsJson.text);
                    }
                }

                if (localAchievementsWrapper != null && localAchievementsWrapper.list != null)
                {
                    bool changed = false;
                    foreach (var achievement in localAchievementsWrapper.list)
                    {
                        if (cloudAchievementStates.TryGetValue(achievement.id, out int cloudStatus))
                        {
                            AchievementState targetState = AchievementState.Locked;
                            if (cloudStatus == 1)
                            {
                                targetState = AchievementState.Unlocked;
                            }
                            else if (cloudStatus == 2)
                            {
                                targetState = AchievementState.Collected;
                            }

                            if (achievement.state != targetState)
                            {
                                achievement.state = targetState;
                                changed = true;
                            }
                        }
                    }

                    if (changed || !File.Exists(achievementsSavePath))
                    {
                        File.WriteAllText(achievementsSavePath, JsonUtility.ToJson(localAchievementsWrapper, true));
                    }

                    AchievementsManager achievementsManager = FindObjectOfType<AchievementsManager>();
                    if (achievementsManager != null && achievementsManager.achievements != null)
                    {
                        Dictionary<int, AchievementData> localById = new Dictionary<int, AchievementData>();
                        foreach (var localAchievement in localAchievementsWrapper.list)
                        {
                            localById[localAchievement.id] = localAchievement;
                        }

                        foreach (var achievement in achievementsManager.achievements)
                        {
                            if (localById.TryGetValue(achievement.id, out AchievementData localAchievement))
                            {
                                achievement.state = localAchievement.state;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[GameDataManager][Cloud] Achievement state restore sikertelen: {ex.Message}");
            }

            SaveLocalData();

            OnProfileRestored?.Invoke();
            OnStatsUpdated?.Invoke();
            OnCoinBalanceChanged?.Invoke();

            if (cloudItemIds.Count > 0)
            {
                Debug.Log($"[GameDataManager][Cloud] {cloudItemIds.Count} darab tárgy letöltve a bolthoz.");
                OnShopItemsRestored?.Invoke(cloudItemIds);
            }

            if (cloudAchievementStates.Count > 0)
            {
                OnAchievementsRestored?.Invoke(cloudAchievementStates);
            }
        }

        try
        {
            if (string.IsNullOrEmpty(localUserId))
            {
                localUserId = Guid.NewGuid().ToString();
                currentProfile.userId = localUserId;
                SaveLocalData();
            }

            bool isGuest = string.IsNullOrEmpty(googleId);

            if (isGuest)
            {
                Debug.Log("[GameDataManager][Cloud] Üres Google ID - vendég mód.");

                if (!await UserExistsAsync(localUserId))
                {
                    await api.InsertUser(new { user_id = localUserId, google_id = "", display_name = userName, status = 1 });
                    await api.InsertUserStat(localUserId, new
                    {
                        coins = currentProfile.coins,
                        levels_completed = currentProfile.maxLevelReached,
                        best_speedrun_amount = GetBestSpeedrunAmount(),
                        best_dailyc_time = GetBestDailyTime()
                    });
                    ////
                    await api.InsertDailyCRecord(localUserId, new
                    {
                        dailyc_time = GetBestDailyTime(),
                        date = DateTime.Now.ToString("yyyy-MM-dd")
                    });
                    await api.InsertSpeedrunRecord(localUserId, new
                    {
                        speedrun_amount = GetBestSpeedrunAmount(),
                        date = DateTime.Now.ToString("yyyy-MM-dd")
                    });
                    ////
                    Debug.Log("[GameDataManager][Cloud] Új vendég profil létrehozva a felhőben.");
                }

                await UploadLocalStateAsync(localUserId);
                return;
            }

            string googleRes = await api.SelectUserByGoogleId(googleId);
            EnsureApiResponse(googleRes, "SelectUserByGoogleId");

            if (!HasRows(googleRes))
            {
                string uniqueUserId = await GenerateUniqueUserIdAsync(localUserId);
                currentProfile.userId = uniqueUserId;
                localUserId = uniqueUserId;

                await api.InsertUser(new { user_id = uniqueUserId, google_id = googleId, display_name = userName, status = 1 });
                await api.InsertUserStat(uniqueUserId, new
                {
                    coins = currentProfile.coins,
                    levels_completed = currentProfile.maxLevelReached,
                    best_speedrun_amount = GetBestSpeedrunAmount(),
                    best_dailyc_time = GetBestDailyTime()
                });
                ////
                await api.InsertDailyCRecord(localUserId, new
                {
                    dailyc_time = GetBestDailyTime(),
                    date = DateTime.Now.ToString("yyyy-MM-dd")
                });
                await api.InsertSpeedrunRecord(localUserId, new
                {
                    speedrun_amount = GetBestSpeedrunAmount(),
                    date = DateTime.Now.ToString("yyyy-MM-dd")
                });
                ////
                SaveLocalData();
                await UploadLocalStateAsync(uniqueUserId);
                Debug.Log("[GameDataManager][Cloud] Új Google-felhasználó létrehozva és mentve.");
                return;
            }

            JObject gRoot = JObject.Parse(googleRes);
            JToken gData = gRoot["data"]?[0] ?? gRoot["result"]?[0];
            string cloudUserId = gData?["user_id"]?.ToString();
            string cloudDisplayName = gData?["display_name"]?.ToString();

            if (!string.IsNullOrEmpty(cloudUserId) && cloudUserId != localUserId)
            {
                Debug.Log($"[GameDataManager][Cloud] Google ID másik userhez tartozik. Felhő profil visszaállítása... (Cloud ID: {cloudUserId})");
                await RestoreLocalFromCloudAsync(cloudUserId, cloudDisplayName);
                return;
            }

            Debug.Log("[GameDataManager][Cloud] Google ID és user ID egyezik. Helyi adatok feltöltése.");
            await UploadLocalStateAsync(localUserId);
        }
        catch (Exception ex)
        {
            nextCloudSyncAttemptUtc = DateTime.UtcNow.AddSeconds(CloudSyncRetryDelaySeconds);
            Debug.LogError($"[GameDataManager][Cloud] Hiba a szinkronizáció során: {ex.Message}");
            Debug.LogWarning($"[GameDataManager][Cloud] Következő próbálkozás: {nextCloudSyncAttemptUtc:O}");
        }
        finally
        {
            isCloudSyncInProgress = false;
        }
    }
    /// <summary>
    /// Letölti az adatokat a felhőből (placeholder).
    /// </summary>
    public void FetchDataFromCloud(Action onComplete)
    {
        SyncDataToCloud(); // Egyelőre a Sync megcsinálja a letöltést is!
        onComplete?.Invoke();
    }
    private List<ShopItemData> GetLocalShopItems()
    {
        try
        {
            ShopManager shopManager = FindObjectOfType<ShopManager>();
            if (shopManager != null && shopManager.ShopItems != null)
            {
                return new List<ShopItemData>(shopManager.ShopItems);
            }

            string shopSavePath = Path.Combine(Application.persistentDataPath, ShopItemsSaveFileName);
            if (File.Exists(shopSavePath))
            {
                string savedJson = File.ReadAllText(shopSavePath);
                ShopItemListWrapper savedWrapper = JsonUtility.FromJson<ShopItemListWrapper>(savedJson);
                if (savedWrapper != null && savedWrapper.list != null)
                {
                    return savedWrapper.list;
                }
            }

            TextAsset defaultShopJson = Resources.Load<TextAsset>(ShopItemsResourcePath);
            if (defaultShopJson != null)
            {
                ShopItemListWrapper defaultWrapper = JsonUtility.FromJson<ShopItemListWrapper>(defaultShopJson.text);
                if (defaultWrapper != null && defaultWrapper.list != null)
                {
                    return defaultWrapper.list;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[GameDataManager][Cloud] Helyi shop itemek beolvasása sikertelen: {ex.Message}");
        }

        return new List<ShopItemData>();
    }
    private List<AchievementData> GetLocalAchievements()
    {
        try
        {
            AchievementsManager achievementsManager = FindObjectOfType<AchievementsManager>();
            if (achievementsManager != null && achievementsManager.achievements != null)
            {
                return new List<AchievementData>(achievementsManager.achievements);
            }

            string achievementsSavePath = Path.Combine(Application.persistentDataPath, AchievementsSaveFileName);
            if (File.Exists(achievementsSavePath))
            {
                string savedJson = File.ReadAllText(achievementsSavePath);
                AchievementListWrapper savedWrapper = JsonUtility.FromJson<AchievementListWrapper>(savedJson);
                if (savedWrapper != null && savedWrapper.list != null)
                {
                    return savedWrapper.list;
                }
            }

            TextAsset defaultAchievementsJson = Resources.Load<TextAsset>(AchievementsResourcePath);
            if (defaultAchievementsJson != null)
            {
                AchievementListWrapper defaultWrapper = JsonUtility.FromJson<AchievementListWrapper>(defaultAchievementsJson.text);
                if (defaultWrapper != null && defaultWrapper.list != null)
                {
                    return defaultWrapper.list;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[GameDataManager][Cloud] Helyi achievementek beolvasása sikertelen: {ex.Message}");
        }

        return new List<AchievementData>();
    }
    #endregion

    #region Helper Metódusok

    /// <summary>
    /// Ellenőrzi, hogy a profil érvényes-e.
    /// </summary>
    private bool IsProfileValid()
    {
        return currentProfile != null;
    }
    /// <summary>
    /// Visszaadja a valaha elért legtöbb teljesített pályát az 1 perces kihívásban.
    /// </summary>
    public int GetBestSpeedrunAmount()
    {
        int best = 0;
        if (currentProfile != null && currentProfile.oneMinChallengesCompleted != null)
        {
            foreach (var record in currentProfile.oneMinChallengesCompleted)
            {
                if (record.levelsCompleted > best)
                {
                    best = record.levelsCompleted;
                }
            }
        }
        return best;
    }

    /// <summary>
    /// Visszaadja a leggyorsabb (legkisebb) napi kihívás időt másodpercben.
    /// </summary>
    public float GetBestDailyTime()
    {
        float best = float.MaxValue; // Induljunk egy nagyon nagy számból
        bool hasValidRecord = false;

        if (currentProfile != null && currentProfile.dailyChallengesCompleted != null)
        {
            foreach (var record in currentProfile.dailyChallengesCompleted)
            {
                // Csak a 0-nál nagyobb (érvényes) időket nézzük, és keressük a legkisebbet
                if (record.completionTime > 0 && record.completionTime < best)
                {
                    best = record.completionTime;
                    hasValidRecord = true;
                }
            }
        }

        // Ha nincs még rekordja, adjunk vissza 0-t. Egyébként kerekítsük 2 tizedesjegyre.
        return hasValidRecord ? (float)Math.Round(best, 2) : 0f;
    }
    /// <summary>
    /// Ellenőrzi, hogy be van-e jelentkezve a felhasználó.
    /// </summary>
    private bool IsLoggedIn()
    {
        return AuthManager.Instance != null && AuthManager.Instance.IsLoggedIn;
    }

    /// <summary>
    /// Visszaadja a mai dátumot sztring formátumban.
    /// </summary>
    private string GetTodayDate()
    {
        return DateTime.Now.ToString(DateFormat);
    }

    /// <summary>
    /// Menti az adatokat és értesíti a coin változásról.
    /// </summary>
    private void SaveAndNotifyCoinChange()
    {
        SaveGame();
        OnCoinBalanceChanged?.Invoke();
    }

    /// <summary>
    /// Menti az adatokat és értesíti a statisztika változásról.
    /// </summary>
    private void SaveAndNotifyStatsUpdate()
    {
        SaveGame();
        OnStatsUpdated?.Invoke();
    }

    #endregion

    #region Progress Reset

    /// <summary>
    /// Reseteli a progresszt és létrehoz egy új profilt random guest névvel.
    /// </summary>
    public void ResetProgressWithRandomProfile()
    {
        string randomGuestName = GenerateRandomGuestName();
        CreateFreshProfile(randomGuestName);
        SaveGame();

        Debug.Log($"[GameDataManager] Progress resetelve. Új profil: {randomGuestName}");
    }

    /// <summary>
    /// Generál egy random guest nevet.
    /// </summary>
    private string GenerateRandomGuestName()
    {
        const string guestPrefix = "Guest_";
        const int minRange = 1000;
        const int maxRange = 9999;

        int randomNumber = UnityEngine.Random.Range(minRange, maxRange);
        return guestPrefix + randomNumber;
    }

    /// <summary>
    /// Létrehoz egy új, tiszta profilt.
    /// </summary>
    private void CreateFreshProfile(string username)
    {
        string tempUserId = Guid.NewGuid().ToString();
        currentProfile = new UserProfileData(tempUserId, username);
    }

    #endregion
}