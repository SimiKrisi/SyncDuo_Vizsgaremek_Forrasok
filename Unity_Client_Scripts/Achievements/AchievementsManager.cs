using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Az achievement (teljesítmény) rendszer kezelését végző osztály.
/// Kezeli az achievementek betöltését, feltételek ellenőrzését, jutalmak kiosztását és UI generálását.
/// </summary>
public class AchievementsManager : MonoBehaviour
{
    #region Konstansok

    private const string AchievementsSaveFileName = "achievements_save.json";
    private const string AchievementsResourcePath = "achievements";
    private const string CoinDisplayFormat = "{0} $";
    private const string DefaultCoinDisplay = "0";
    private const string ColorTagFormat = "<color={0}>{1}</color>";
    private const string ColorGreen = "green";
    private const string ColorYellow = "yellow";
    private const string UIElementNameStatusIcon = "StatusIcon";
    private const string UIElementNameDescription = "Description";
    private const string UIElementNameAmount = "Amount";
    private const float CollectedItemColorR = 0.8f;
    private const float CollectedItemColorG = 1f;
    private const float CollectedItemColorB = 0.8f;
    private const float UnlockedItemColorR = 1f;
    private const float UnlockedItemColorG = 1f;
    private const float UnlockedItemColorB = 0.8f;

    #endregion

    #region Unity Inspector Mezők

    [Header("UI Elemek")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI coinText;
    public Transform contentContainer;
    public GameObject itemPrefab;

    [Header("Adatok")]
    public List<AchievementData> achievements = new List<AchievementData>();

    [Header("Grafika")]
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public Sprite collectedSprite;

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Inicializálja az achievement rendszert.
    /// </summary>
    void Start()
    {
        InitializeAchievements();
    }

    /// <summary>
    /// Leiratkozik az eseményekről a megsemmisítéskor.
    /// </summary>
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Inicializálja az összes achievement komponenst.
    /// </summary>
    private void InitializeAchievements()
    {
        InitializeUsername();
        SubscribeToEvents();
        LoadAchievements();
        CheckAllAchievements();
        UpdateCoinUI();
        GenerateList();
    }

    /// <summary>
    /// Inicializálja a felhasználónév megjelenítését.
    /// </summary>
    private void InitializeUsername()
    {
        ClearUsernameText();

        if (IsGameDataValid() && HasValidUsername())
        {
            DisplayUsername();
        }
    }

    /// <summary>
    /// Törli a felhasználónév szövegét.
    /// </summary>
    private void ClearUsernameText()
    {
        usernameText.text = "";
    }

    /// <summary>
    /// Ellenőrzi, hogy a GameDataManager érvényes-e.
    /// </summary>
    private bool IsGameDataValid()
    {
        return GameDataManager.Instance != null &&
               GameDataManager.Instance.currentProfile != null;
    }

    /// <summary>
    /// Ellenőrzi, hogy van-e érvényes felhasználónév.
    /// </summary>
    private bool HasValidUsername()
    {
        return !string.IsNullOrEmpty(GameDataManager.Instance.currentProfile.userName);
    }

    /// <summary>
    /// Megjeleníti a felhasználónevet.
    /// </summary>
    private void DisplayUsername()
    {
        string username = GetCurrentUsername();
        SetUsernameText(username);
        LogUsernameDisplay(username);
    }

    /// <summary>
    /// Visszaadja az aktuális felhasználónevet.
    /// </summary>
    private string GetCurrentUsername()
    {
        return GameDataManager.Instance.currentProfile.userName;
    }

    /// <summary>
    /// Beállítja a felhasználónév szövegét.
    /// </summary>
    private void SetUsernameText(string username)
    {
        usernameText.text = username;
    }

    /// <summary>
    /// Logolja a felhasználónév megjelenítését.
    /// </summary>
    private void LogUsernameDisplay(string username)
    {
        Debug.Log($"Felhasználónév automatikusan kitöltve: {username}");
    }

    /// <summary>
    /// Feliratkozik a GameDataManager eseményeire.
    /// </summary>
    private void SubscribeToEvents()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnStatsUpdated += CheckAllAchievements;
        }
    }

    /// <summary>
    /// Leiratkozik a GameDataManager eseményeiről.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnStatsUpdated -= CheckAllAchievements;
        }
    }

    #endregion

    #region Achievement Ellenőrzés

    /// <summary>
    /// Ellenőrzi az összes achievement feltételét.
    /// </summary>
    public void CheckAllAchievements()
    {
        if (!IsGameDataValid())
            return;

        UserProfileData profile = GetCurrentProfile();
        bool hasChanges = CheckLockedAchievements(profile);

        if (hasChanges)
        {
            HandleAchievementChanges();
        }
    }

    /// <summary>
    /// Visszaadja az aktuális profilt.
    /// </summary>
    private UserProfileData GetCurrentProfile()
    {
        return GameDataManager.Instance.currentProfile;
    }

    /// <summary>
    /// Ellenőrzi a zárolt achievementeket.
    /// </summary>
    private bool CheckLockedAchievements(UserProfileData profile)
    {
        bool changed = false;

        foreach (AchievementData ach in achievements)
        {
            if (IsAchievementLocked(ach))
            {
                if (CheckAchievementCondition(ach, profile))
                {
                    UnlockAchievement(ach);
                    changed = true;
                }
            }
        }

        return changed;
    }

    /// <summary>
    /// Ellenőrzi, hogy az achievement zárolva van-e.
    /// </summary>
    private bool IsAchievementLocked(AchievementData ach)
    {
        return ach.state == AchievementState.Locked;
    }

    /// <summary>
    /// Ellenőrzi az achievement feltételét.
    /// </summary>
    private bool CheckAchievementCondition(AchievementData ach, UserProfileData profile)
    {
        return AchievementRules.IsConditionMet(ach.id, profile);
    }

    /// <summary>
    /// Feloldja az achievementet.
    /// </summary>
    private void UnlockAchievement(AchievementData ach)
    {
        ach.state = AchievementState.Unlocked;
        LogAchievementUnlocked(ach.description);
    }

    /// <summary>
    /// Logolja az achievement feloldását.
    /// </summary>
    private void LogAchievementUnlocked(string description)
    {
        Debug.Log($"🔓 ACHIEVEMENT FELOLDVA: {description}");
    }

    /// <summary>
    /// Kezeli az achievement változásokat.
    /// </summary>
    private void HandleAchievementChanges()
    {
        SaveAchievementsLocal();
        TriggerCloudSync();
        GenerateList();
    }

    #endregion

    #region Achievement Betöltés és Mentés

    /// <summary>
    /// Betölti az achievementeket helyi mentésből vagy Resources-ból.
    /// </summary>
    void LoadAchievements()
    {
        string localPath = GetAchievementsSavePath();

        if (HasLocalAchievementSave(localPath))
        {
            LoadAchievementsFromLocalSave(localPath);
        }
        else
        {
            LoadAchievementsFromResources();
        }
    }

    /// <summary>
    /// Visszaadja az achievementek mentési útvonalát.
    /// </summary>
    private string GetAchievementsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, AchievementsSaveFileName);
    }

    /// <summary>
    /// Ellenőrzi, hogy van-e helyi achievement mentés.
    /// </summary>
    private bool HasLocalAchievementSave(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// Betölti az achievementeket a helyi mentésből.
    /// </summary>
    private void LoadAchievementsFromLocalSave(string path)
    {
        string json = File.ReadAllText(path);
        DeserializeAchievements(json);
        LogLocalLoadSuccess(path);
    }

    /// <summary>
    /// Deserializálja az achievementeket JSON-ből.
    /// </summary>
    private void DeserializeAchievements(string json)
    {
        AchievementListWrapper wrapper = JsonUtility.FromJson<AchievementListWrapper>(json);
        achievements = wrapper.list;
    }

    /// <summary>
    /// Logolja a helyi betöltés sikerességét.
    /// </summary>
    private void LogLocalLoadSuccess(string path)
    {
        Debug.Log($"📂 Achievementek betöltve a helyi mentésből: {path}");
    }

    /// <summary>
    /// Betölti az achievementeket a Resources mappából.
    /// </summary>
    private void LoadAchievementsFromResources()
    {
        TextAsset jsonFile = LoadAchievementsResource();

        if (jsonFile != null)
        {
            ProcessResourceFile(jsonFile);
        }
        else
        {
            LogResourceLoadError();
        }
    }

    /// <summary>
    /// Betölti az achievements resource fájlt.
    /// </summary>
    private TextAsset LoadAchievementsResource()
    {
        return Resources.Load<TextAsset>(AchievementsResourcePath);
    }

    /// <summary>
    /// Feldolgozza a resource fájlt.
    /// </summary>
    private void ProcessResourceFile(TextAsset jsonFile)
    {
        DeserializeAchievements(jsonFile.text);
        SaveAchievementsLocal();
        LogResourceLoadSuccess();
    }

    /// <summary>
    /// Logolja a resource betöltés sikerességét.
    /// </summary>
    private void LogResourceLoadSuccess()
    {
        Debug.Log("🆕 Alap achievementek betöltve és mentve.");
    }

    /// <summary>
    /// Logolja a resource betöltési hibát.
    /// </summary>
    private void LogResourceLoadError()
    {
        Debug.LogError("❌ Nem található az achievements.json a Resources mappában!");
    }

    /// <summary>
    /// Menti az achievementek állapotát helyi fájlba.
    /// </summary>
    public void SaveAchievementsLocal()
    {
        string json = SerializeAchievements();
        string path = GetAchievementsSavePath();
        WriteAchievementsToFile(path, json);
    }

    /// <summary>
    /// Szerializálja az achievementeket JSON formátumba.
    /// </summary>
    private string SerializeAchievements()
    {
        AchievementListWrapper wrapper = CreateAchievementsWrapper();
        return JsonUtility.ToJson(wrapper, true);
    }

    /// <summary>
    /// Létrehoz egy wrapper objektumot az achievementekhez.
    /// </summary>
    private AchievementListWrapper CreateAchievementsWrapper()
    {
        AchievementListWrapper wrapper = new AchievementListWrapper();
        wrapper.list = achievements;
        return wrapper;
    }

    /// <summary>
    /// Kiírja az achievements JSON-t fájlba.
    /// </summary>
    private void WriteAchievementsToFile(string path, string json)
    {
        File.WriteAllText(path, json);
    }

    #endregion

    #region Coin UI Kezelés

    /// <summary>
    /// Frissíti a coin UI megjelenítését.
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
    /// Megjeleníti az aktuális coin mennyiséget.
    /// </summary>
    private void DisplayCurrentCoins()
    {
        int currentCoins = GetCurrentCoins();
        SetCoinText(string.Format(CoinDisplayFormat, currentCoins));
    }

    /// <summary>
    /// Visszaadja az aktuális coin mennyiséget.
    /// </summary>
    private int GetCurrentCoins()
    {
        return GameDataManager.Instance.currentProfile.coins;
    }

    /// <summary>
    /// Beállítja a coin szöveg értékét.
    /// </summary>
    private void SetCoinText(string text)
    {
        if (coinText != null)
        {
            coinText.text = text;
        }
    }

    /// <summary>
    /// Megjeleníti az alapértelmezett coin értéket.
    /// </summary>
    private void DisplayDefaultCoins()
    {
        SetCoinText(DefaultCoinDisplay);
    }

    #endregion

    #region Lista Generálás

    /// <summary>
    /// Generálja a teljes achievement UI listát.
    /// </summary>
    void GenerateList()
    {
        ClearExistingList();
        List<AchievementData> sortedAchievements = GetSortedAchievements();
        CreateAchievementItems(sortedAchievements);
    }

    /// <summary>
    /// Törli a meglévő lista tartalmát.
    /// </summary>
    private void ClearExistingList()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Visszaadja a rendezett achievement listát.
    /// </summary>
    private List<AchievementData> GetSortedAchievements()
    {
        return achievements
            .OrderByDescending(x => x.state)
            .ThenBy(x => x.id)
            .ToList();
    }

    /// <summary>
    /// Létrehozza az achievement item UI elemeket.
    /// </summary>
    private void CreateAchievementItems(List<AchievementData> sortedAchievements)
    {
        foreach (AchievementData ach in sortedAchievements)
        {
            CreateAchievementItem(ach);
        }
    }

    /// <summary>
    /// Létrehoz egy achievement item UI elemet.
    /// </summary>
    private void CreateAchievementItem(AchievementData ach)
    {
        GameObject newItem = InstantiateAchievementPrefab();
        SetupAchievementIcon(newItem, ach);
        SetupAchievementDescription(newItem, ach);
        SetupAchievementReward(newItem, ach);
        SetupAchievementButton(newItem, ach);
    }

    /// <summary>
    /// Példányosítja az achievement prefab-ot.
    /// </summary>
    private GameObject InstantiateAchievementPrefab()
    {
        return Instantiate(itemPrefab, contentContainer);
    }

    /// <summary>
    /// Beállítja az achievement ikon képét és színét.
    /// </summary>
    private void SetupAchievementIcon(GameObject itemObject, AchievementData ach)
    {
        Transform iconObj = itemObject.transform.Find(UIElementNameStatusIcon);
        if (iconObj != null)
        {
            Image iconImg = iconObj.GetComponent<Image>();
            if (iconImg != null)
            {
                SetIconProperties(iconImg, ach.state);
            }
        }
    }

    /// <summary>
    /// Beállítja az ikon tulajdonságait állapot szerint.
    /// </summary>
    private void SetIconProperties(Image iconImg, AchievementState state)
    {
        iconImg.preserveAspect = true;
        iconImg.sprite = GetIconSpriteByState(state);
        iconImg.color = GetIconColorByState(state);
    }

    /// <summary>
    /// Visszaadja az ikon sprite-ját az állapot szerint.
    /// </summary>
    private Sprite GetIconSpriteByState(AchievementState state)
    {
        switch (state)
        {
            case AchievementState.Locked:
                return lockedSprite;
            case AchievementState.Unlocked:
                return unlockedSprite;
            case AchievementState.Collected:
                return collectedSprite;
            default:
                return lockedSprite;
        }
    }

    /// <summary>
    /// Visszaadja az ikon színét az állapot szerint.
    /// </summary>
    private Color GetIconColorByState(AchievementState state)
    {
        if (state == AchievementState.Collected)
        {
            return new Color(CollectedItemColorR, CollectedItemColorG, CollectedItemColorB);
        }
        return Color.white;
    }

    /// <summary>
    /// Beállítja az achievement leírás szövegét.
    /// </summary>
    private void SetupAchievementDescription(GameObject itemObject, AchievementData ach)
    {
        Transform descObj = itemObject.transform.Find(UIElementNameDescription);
        if (descObj != null)
        {
            descObj.GetComponent<TMP_Text>().text = ach.description;
        }
    }

    /// <summary>
    /// Beállítja az achievement jutalom szövegét.
    /// </summary>
    private void SetupAchievementReward(GameObject itemObject, AchievementData ach)
    {
        Transform amountObj = itemObject.transform.Find(UIElementNameAmount);
        if (amountObj != null)
        {
            string displayText = GetRewardDisplayText(ach);
            amountObj.GetComponent<TMP_Text>().text = displayText;
        }
    }

    /// <summary>
    /// Visszaadja a jutalom megjelenítési szövegét.
    /// </summary>
    private string GetRewardDisplayText(AchievementData ach)
    {
        string color = GetRewardColor(ach.state);
        string rewardText = $"{ach.reward} $";
        return string.Format(ColorTagFormat, color, rewardText);
    }

    /// <summary>
    /// Visszaadja a jutalom színét az állapot szerint.
    /// </summary>
    private string GetRewardColor(AchievementState state)
    {
        return (state == AchievementState.Collected) ? ColorGreen : ColorYellow;
    }

    /// <summary>
    /// Beállítja az achievement gomb funkcionalitását és megjelenését.
    /// </summary>
    private void SetupAchievementButton(GameObject itemObject, AchievementData ach)
    {
        Button btn = itemObject.GetComponent<Button>();
        Image bg = itemObject.GetComponent<Image>();

        if (btn != null)
        {
            ConfigureButtonByState(btn, bg, ach);
        }
    }

    /// <summary>
    /// Konfigurálja a gombot az állapot szerint.
    /// </summary>
    private void ConfigureButtonByState(Button btn, Image bg, AchievementData ach)
    {
        switch (ach.state)
        {
            case AchievementState.Locked:
                ConfigureLockedButton(btn, bg);
                break;
            case AchievementState.Unlocked:
                ConfigureUnlockedButton(btn, bg, ach);
                break;
            case AchievementState.Collected:
                ConfigureCollectedButton(btn, bg);
                break;
        }
    }

    /// <summary>
    /// Konfigurálja a zárolt gombot.
    /// </summary>
    private void ConfigureLockedButton(Button btn, Image bg)
    {
        btn.interactable = false;
        if (bg != null)
        {
            bg.color = Color.white;
        }
    }

    /// <summary>
    /// Konfigurálja a feloldott gombot.
    /// </summary>
    private void ConfigureUnlockedButton(Button btn, Image bg, AchievementData ach)
    {
        btn.interactable = true;
        if (bg != null)
        {
            bg.color = new Color(UnlockedItemColorR, UnlockedItemColorG, UnlockedItemColorB);
        }
        btn.onClick.AddListener(() => OnClaimClicked(ach));
    }

    /// <summary>
    /// Konfigurálja a begyűjtött gombot.
    /// </summary>
    private void ConfigureCollectedButton(Button btn, Image bg)
    {
        btn.interactable = false;
        if (bg != null)
        {
            bg.color = new Color(CollectedItemColorR, CollectedItemColorG, CollectedItemColorB);
        }
    }

    #endregion

    #region Jutalom Átvétel

    /// <summary>
    /// Kezeli a jutalom átvételi kattintást.
    /// </summary>
    void OnClaimClicked(AchievementData ach)
    {
        if (IsAchievementUnlocked(ach))
        {
            ProcessRewardClaim(ach);
        }
    }

    /// <summary>
    /// Ellenőrzi, hogy az achievement feloldott-e.
    /// </summary>
    private bool IsAchievementUnlocked(AchievementData ach)
    {
        return ach.state == AchievementState.Unlocked;
    }

    /// <summary>
    /// Feldolgozza a jutalom átvételét.
    /// </summary>
    private void ProcessRewardClaim(AchievementData ach)
    {
        PlayClaimSFX();
        MarkAchievementAsCollected(ach);
        GrantRewardCoins(ach.reward);
        SaveAndRefreshUI();
        TriggerCloudSync();
        LogRewardClaimed(ach.reward);
    }

    ///<summary>
    /// Lejátszik egy jutalom átvételi hanghatást.
    /// </summary>
    private void PlayClaimSFX()
    { 
    AudioManager.Instance?.sfxSource.PlayOneShot(AudioManager.Instance.achievementClaimSFX);
    }

    /// <summary>
    /// Megjelöli az achievementet begyűjtöttként.
    /// </summary>
    private void MarkAchievementAsCollected(AchievementData ach)
    {
        ach.state = AchievementState.Collected;
    }

    /// <summary>
    /// Jóváírja a jutalom pénzösszeget.
    /// </summary>
    private void GrantRewardCoins(int reward)
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.AddCoins(reward);
        }
    }

    /// <summary>
    /// Menti az állapotot és frissíti a UI-t.
    /// </summary>
    private void SaveAndRefreshUI()
    {
        SaveAchievementsLocal();
        GenerateList();
    }

    private void TriggerCloudSync()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SyncDataToCloud();
        }
    }

    /// <summary>
    /// Logolja a jutalom átvételét.
    /// </summary>
    private void LogRewardClaimed(int reward)
    {
        Debug.Log($"💰 Jutalom átvéve: {reward}");
    }

    #endregion
}
