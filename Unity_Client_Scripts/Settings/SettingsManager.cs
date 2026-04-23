using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Globalization;
using System.Text;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elemek")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_InputField usernameInput;
    public TMP_Dropdown languageDropdown;
    [Header("Web Linkek")]
    public string creditsUrl = "https://www.google.com";

    private bool isUpdatingUsername;

    
    void Start()
    {
        if (AudioManager.Instance != null)
        {
            musicSlider.value = AudioManager.Instance.musicSource.volume;
            sfxSlider.value = AudioManager.Instance.sfxSource.volume;
        }
        if (GameDataManager.Instance != null && GameDataManager.Instance.currentProfile != null)
        {   
            usernameInput.text = SanitizeUsername(GameDataManager.Instance.currentProfile.userName);
        }
        if (LocalizationManager.Instance != null)
        {
            string currentLang = LocalizationManager.Instance.CurrentLanguage;
            if (currentLang == "de") languageDropdown.value = 1; // 1 = Német
            else languageDropdown.value = 0; // 0 = Angol
        }
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        usernameInput.onValidateInput += ValidateUsernameInput;
        usernameInput.onValueChanged.AddListener(OnUsernameChanged);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        GameDataManager.Instance.OnProfileRestored += UpdateNameText;
    }
    private void OnDestroy()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnProfileRestored -= UpdateNameText;

        }
    }
    private void UpdateNameText()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.currentProfile != null)
        {
            usernameInput.text = GameDataManager.Instance.currentProfile.userName;
        }
    }
    public void OnLanguageChanged(int index)
    {
        string selectedCode = "en";

        if (index == 0) selectedCode = "en";
        else if (index == 1) selectedCode = "de";

        Debug.Log("Nyelv váltása erre: " + selectedCode);

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage(selectedCode);
        }
    }
    public void OnMusicChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value, true);
        }
    }
    public void OnSFXChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value, true);
        }
    }
    /// <summary>
    /// Reset Progress gomb kezelése - coroutine indítással.
    /// </summary>
    public void OnResetProgressPressed()
    {
        StartCoroutine(ResetProgressCoroutine());
    }

    /// <summary>
    /// Progress reset folyamat várakozással a fájl mentés után.
    /// </summary>
    private System.Collections.IEnumerator ResetProgressCoroutine()
    {
        Debug.LogWarning("⚠️ Játékos adatok törlése folyamatban...");

        DeleteSaveFiles();
        ClearPlayerPreferences();
        ResetGameProfile();

        // Várunk 0.5 másodpercet, hogy a fájl írás biztosan befejeződjön
        yield return new WaitForSeconds(0.5f);

        RestartApplication();
    }

    /// <summary>
    /// Törli az összes mentési fájlt.
    /// </summary>
    private void DeleteSaveFiles()
    {
        string[] filesToDelete = { 
            "playerdata.json", 
            "achievements_save.json", 
            "shopitems_save.json" 
        };

        foreach (string fileName in filesToDelete)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"🗑️ Törölve: {fileName}");
            }
        }
    }

    /// <summary>
    /// Törli a PlayerPrefs beállításokat.
    /// </summary>
    private void ClearPlayerPreferences()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Azonnal mentjük a törlést
        Debug.Log("🗑️ PlayerPrefs (Beállítások) törölve.");
    }

    /// <summary>
    /// Reseteli a játék profilt random névvel és alapértékekkel.
    /// </summary>
    private void ResetGameProfile()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ResetProgressWithRandomProfile();
        }
    }

    /// <summary>
    /// Újraindítja az alkalmazást a SplashScene betöltésével.
    /// </summary>
    private void RestartApplication()
    {
        SceneManager.LoadScene("SplashScene");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnLoginButtonPressed()
    {
        Debug.Log("Login Button Pressed");
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
    public void OnUsernameChanged(string newName)
    {
        if (isUpdatingUsername)
        {
            return;
        }

        string sanitized = SanitizeUsername(newName);
        if (sanitized != newName)
        {
            isUpdatingUsername = true;
            usernameInput.SetTextWithoutNotify(sanitized);
            isUpdatingUsername = false;
            newName = sanitized;
        }

        // 1. Frissítjük a GameDataManager-ben a nevet
        if (GameDataManager.Instance != null && GameDataManager.Instance.currentProfile != null)
        {
            GameDataManager.Instance.currentProfile.userName = newName;

            // 2. Azonnal elmentjük a fájlba (playerdata.json)
            GameDataManager.Instance.SaveGame();
        }

        // (Opcionális) Ha van AuthManager, ott is frissíthetjük a kijelzést, ha szükséges
        // if (AuthManager.Instance != null) { ... }
    }
    public void OnCreditsButtonPressed()
    {
        Debug.Log("Credits Button Pressed");
        Application.OpenURL(creditsUrl);
    }

    private char ValidateUsernameInput(string text, int charIndex, char addedChar)
    {
        string normalized = addedChar.ToString().Normalize(NormalizationForm.FormD);
        StringBuilder builder = new StringBuilder(normalized.Length);

        foreach (char c in normalized)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark && category != UnicodeCategory.SpacingCombiningMark && category != UnicodeCategory.EnclosingMark)
            {
                builder.Append(c);
            }
        }

        if (builder.Length == 0)
        {
            return '\0';
        }

        return builder[0];
    }

    private string SanitizeUsername(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        string normalized = value.Normalize(NormalizationForm.FormD);
        StringBuilder builder = new StringBuilder(normalized.Length);

        foreach (char c in normalized)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark && category != UnicodeCategory.SpacingCombiningMark && category != UnicodeCategory.EnclosingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

}
