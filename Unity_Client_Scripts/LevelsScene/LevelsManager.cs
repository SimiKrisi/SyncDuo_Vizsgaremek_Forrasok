using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A szint kiválasztási képernyõ kezelését végzõ osztály.
/// Betölti a szinteket JSON-bõl, generálja a gombokat és kezeli a szint kiválasztást.
/// </summary>
public class LevelsManager : MonoBehaviour
{
    #region Konstansok

    private const string DefaultJsonFileName = "dailychallenges";
    private const string GameSceneName = "GameScene";
    private const string UIElementNameLockIcon = "LockIcon";
    private const int LevelsAheadToShow = 15;
    private const int LevelNumberOffset = 1;

    #endregion

    #region Unity Inspector Mezõk

    [Header("Konfiguráció")]
    public string jsonFileName = DefaultJsonFileName;

    [Header("UI Elemek")]
    public GameObject levelButtonPrefab;
    public Transform buttonContainer;

    #endregion

    #region Privát Mezõk

    private List<LevelRawData> loadedLevels;

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Inicializálja a szint kiválasztási képernyõt.
    /// </summary>
    void Start()
    {
        InitializeLevelSelection();
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Inicializálja a szint kiválasztási rendszert.
    /// </summary>
    private void InitializeLevelSelection()
    {
        LoadLevelsFromJson();
        GenerateLevelButtons();
    }

    #endregion

    #region JSON Betöltés

    /// <summary>
    /// Betölti a szinteket a JSON fájlból.
    /// </summary>
    private void LoadLevelsFromJson()
    {
        TextAsset jsonFile = LoadJsonFile();

        if (jsonFile == null)
        {
            HandleJsonLoadError();
            return;
        }

        ParseJsonFile(jsonFile);
    }

    /// <summary>
    /// Betölti a JSON fájlt a Resources mappából.
    /// </summary>
    private TextAsset LoadJsonFile()
    {
        return Resources.Load<TextAsset>(jsonFileName);
    }

    /// <summary>
    /// Kezeli a JSON betöltési hibát.
    /// </summary>
    private void HandleJsonLoadError()
    {
        LogJsonLoadError();
        InitializeEmptyLevelList();
    }

    /// <summary>
    /// Logolja a JSON betöltési hibát.
    /// </summary>
    private void LogJsonLoadError()
    {
        Debug.LogError($"CRITICAL: Nem található a '{jsonFileName}.json' a Resources mappában!");
    }

    /// <summary>
    /// Inicializál egy üres szint listát.
    /// </summary>
    private void InitializeEmptyLevelList()
    {
        loadedLevels = new List<LevelRawData>();
    }

    /// <summary>
    /// Feldolgozza a JSON fájlt.
    /// </summary>
    private void ParseJsonFile(TextAsset jsonFile)
    {
        LevelListWrapper wrapper = DeserializeJson(jsonFile.text);

        if (IsJsonValid(wrapper))
        {
            AssignLoadedLevels(wrapper);
            LogLoadSuccess();
        }
        else
        {
            HandleJsonParseError();
        }
    }

    /// <summary>
    /// Deserializálja a JSON szöveget.
    /// </summary>
    private LevelListWrapper DeserializeJson(string json)
    {
        return JsonUtility.FromJson<LevelListWrapper>(json);
    }

    /// <summary>
    /// Ellenõrzi, hogy a JSON érvényes-e.
    /// </summary>
    private bool IsJsonValid(LevelListWrapper wrapper)
    {
        return wrapper != null && wrapper.boards != null;
    }

    /// <summary>
    /// Hozzárendeli a betöltött szinteket.
    /// </summary>
    private void AssignLoadedLevels(LevelListWrapper wrapper)
    {
        loadedLevels = wrapper.boards;
    }

    /// <summary>
    /// Logolja a sikeres betöltést.
    /// </summary>
    private void LogLoadSuccess()
    {
        Debug.Log($"Sikeresen betöltve {loadedLevels.Count} szint a JSON-bõl.");
    }

    /// <summary>
    /// Kezeli a JSON parse hibát.
    /// </summary>
    private void HandleJsonParseError()
    {
        LogJsonParseError();
        InitializeEmptyLevelList();
    }

    /// <summary>
    /// Logolja a JSON parse hibát.
    /// </summary>
    private void LogJsonParseError()
    {
        Debug.LogError("Hiba a JSON parszolása közben (üres vagy hibás formátum).");
    }

    #endregion

    #region Gomb Generálás

    /// <summary>
    /// Generálja a szint kiválasztó gombokat.
    /// </summary>
    private void GenerateLevelButtons()
    {
        if (!HasLoadedLevels())
            return;

        int maxLevelReached = GetMaxLevelReached();
        int levelsToShow = CalculateLevelsToShow(maxLevelReached);

        ClearExistingButtons();
        CreateLevelButtons(levelsToShow, maxLevelReached);
    }

    /// <summary>
    /// Ellenõrzi, hogy vannak-e betöltött szintek.
    /// </summary>
    private bool HasLoadedLevels()
    {
        return loadedLevels != null && loadedLevels.Count > 0;
    }

    /// <summary>
    /// Visszaadja a maximálisan elért szintet.
    /// </summary>
    private int GetMaxLevelReached()
    {
        if (IsGameDataValid())
        {
            return GameDataManager.Instance.currentProfile.maxLevelReached;
        }
        return 0;
    }

    /// <summary>
    /// Ellenõrzi, hogy a GameDataManager érvényes-e.
    /// </summary>
    private bool IsGameDataValid()
    {
        return GameDataManager.Instance != null &&
               GameDataManager.Instance.currentProfile != null;
    }

    /// <summary>
    /// Kiszámolja a megjelenítendõ szintek számát.
    /// </summary>
    private int CalculateLevelsToShow(int maxLevelReached)
    {
        return Mathf.Min(loadedLevels.Count, maxLevelReached + LevelsAheadToShow);
    }

    /// <summary>
    /// Törli a meglévõ gombokat.
    /// </summary>
    private void ClearExistingButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Létrehozza a szint gombokat.
    /// </summary>
    private void CreateLevelButtons(int levelsToShow, int maxLevelReached)
    {
        for (int i = 0; i < levelsToShow; i++)
        {
            CreateSingleLevelButton(i, maxLevelReached);
        }
    }

    /// <summary>
    /// Létrehoz egy szint gombot.
    /// </summary>
    private void CreateSingleLevelButton(int levelIndex, int maxLevelReached)
    {
        GameObject buttonObject = InstantiateLevelButton();
        LevelButtonComponents components = GetButtonComponents(buttonObject);
        bool isUnlocked = IsLevelUnlocked(levelIndex, maxLevelReached);

        ConfigureLevelButton(components, levelIndex, isUnlocked);
    }

    /// <summary>
    /// Példányosítja a szint gombot.
    /// </summary>
    private GameObject InstantiateLevelButton()
    {
        return Instantiate(levelButtonPrefab, buttonContainer);
    }

    /// <summary>
    /// Visszaadja a gomb komponenseit.
    /// </summary>
    private LevelButtonComponents GetButtonComponents(GameObject buttonObject)
    {
        LevelButtonComponents components = new LevelButtonComponents();
        components.button = buttonObject.GetComponent<Button>();
        components.text = buttonObject.GetComponentInChildren<TMP_Text>();
        components.lockIcon = buttonObject.transform.Find(UIElementNameLockIcon);
        return components;
    }

    /// <summary>
    /// Ellenõrzi, hogy a szint feloldott-e.
    /// </summary>
    private bool IsLevelUnlocked(int levelIndex, int maxLevelReached)
    {
        return levelIndex <= maxLevelReached;
    }

    /// <summary>
    /// Konfigurálja a szint gombot.
    /// </summary>
    private void ConfigureLevelButton(LevelButtonComponents components, int levelIndex, bool isUnlocked)
    {
        if (isUnlocked)
        {
            ConfigureUnlockedButton(components, levelIndex);
        }
        else
        {
            ConfigureLockedButton(components);
        }
    }

    /// <summary>
    /// Konfigurálja a feloldott gombot.
    /// </summary>
    private void ConfigureUnlockedButton(LevelButtonComponents components, int levelIndex)
    {
        SetButtonInteractable(components.button, true);
        SetButtonText(components.text, GetLevelDisplayNumber(levelIndex));
        SetLockIconVisibility(components.lockIcon, false);
        AddButtonClickListener(components.button, levelIndex);
    }

    /// <summary>
    /// Konfigurálja a zárolt gombot.
    /// </summary>
    private void ConfigureLockedButton(LevelButtonComponents components)
    {
        SetButtonInteractable(components.button, false);
        SetButtonText(components.text, "");
        SetLockIconVisibility(components.lockIcon, true);
    }

    /// <summary>
    /// Beállítja a gomb interaktív állapotát.
    /// </summary>
    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    /// <summary>
    /// Beállítja a gomb szövegét.
    /// </summary>
    private void SetButtonText(TMP_Text text, string content)
    {
        if (text != null)
        {
            text.text = content;
        }
    }

    /// <summary>
    /// Visszaadja a szint megjelenítési számát.
    /// </summary>
    private string GetLevelDisplayNumber(int levelIndex)
    {
        return (levelIndex + LevelNumberOffset).ToString();
    }

    /// <summary>
    /// Beállítja a lakat ikon láthatóságát.
    /// </summary>
    private void SetLockIconVisibility(Transform lockIcon, bool visible)
    {
        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// Hozzáadja a gomb klikk eseménykezelõt.
    /// </summary>
    private void AddButtonClickListener(Button button, int levelIndex)
    {
        if (button != null)
        {
            button.onClick.AddListener(() => SelectLevel(levelIndex));
        }
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

    #region Szint Kiválasztás

    /// <summary>
    /// Kezeli a szint kiválasztását.
    /// </summary>
    public void SelectLevel(int levelIndex)
    {
        if (IsLevelIndexValid(levelIndex))
        {
            PlayButtonSFX();
            LoadSelectedLevel(levelIndex);
        }
        else
        {
            LogInvalidLevelIndex(levelIndex);
        }
    }

    /// <summary>
    /// Ellenõrzi, hogy a szint index érvényes-e.
    /// </summary>
    private bool IsLevelIndexValid(int levelIndex)
    {
        return levelIndex >= 0 && levelIndex < loadedLevels.Count;
    }

    /// <summary>
    /// Betölti a kiválasztott szintet.
    /// </summary>
    private void LoadSelectedLevel(int levelIndex)
    {
        SaveSelectedLevelIndex(levelIndex);
        LoadGameScene();
    }

    /// <summary>
    /// Menti a kiválasztott szint indexét.
    /// </summary>
    private void SaveSelectedLevelIndex(int levelIndex)
    {
        LevelContext.CurrentLevelIndex = levelIndex;
    }

    /// <summary>
    /// Betölti a játék scene-t.
    /// </summary>
    private void LoadGameScene()
    {
        SceneManager.LoadScene(GameSceneName);
    }

    /// <summary>
    /// Logolja az érvénytelen szint indexet.
    /// </summary>
    private void LogInvalidLevelIndex(int levelIndex)
    {
        Debug.LogError($"Hiba: {levelIndex}. szint index nem létezik.");
    }

    #endregion

    #region Belsõ Struktúrák

    /// <summary>
    /// Szint gomb komponensek tárolására szolgáló struktúra.
    /// </summary>
    private struct LevelButtonComponents
    {
        public Button button;
        public TMP_Text text;
        public Transform lockIcon;
    }

    #endregion
}