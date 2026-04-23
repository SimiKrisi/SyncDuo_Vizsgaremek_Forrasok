using UnityEngine;
using System.Collections.Generic;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    // Esemény, amire a szövegek feliratkozhatnak (ha váltasz, azonnal frissülnek)
    public Action OnLanguageChanged;

    // A jelenlegi nyelv kódja
    public string CurrentLanguage { get; private set; } = "en";

    // --- A SZÓTÁR (Kulcs -> {Angol, Német}) ---
    // A kulcs (pl. "btn_login") alapján kapjuk vissza a fordításokat
    private Dictionary<string, Dictionary<string, string>> translations;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);

        InitializeTranslations();
    }

    // Update is called once per frame
    public void Localise()
    {
        // Betöltjük a mentett nyelvet, ha van
        if (GameDataManager.Instance != null && GameDataManager.Instance.currentProfile != null)
        {
            SetLanguage(GameDataManager.Instance.currentProfile.language);
        }
        else
        {
            SetLanguage("en");
        }
    }
    public void SetLanguage(string langCode)
    {
        CurrentLanguage = langCode;

        // Elmentjük a GameDataManager-be is
        if (GameDataManager.Instance != null && GameDataManager.Instance.currentProfile != null)
        {
            GameDataManager.Instance.currentProfile.language = langCode;
            GameDataManager.Instance.SaveGame(); // Ez menti lokálisan és (ha van net) felhõbe is!
        }

        // Szólunk mindenkinek, hogy frissítsék a szöveget
        OnLanguageChanged?.Invoke();
    }
    public string GetTranslation(string key)
    {
        if (translations.ContainsKey(key))
        {
            if (translations[key].ContainsKey(CurrentLanguage))
            {
                return translations[key][CurrentLanguage];
            }
        }
        return key; // Ha nincs fordítás, visszaadjuk a kulcsot (hogy lásd a hibát)
    }
    private void InitializeTranslations()
    {
        translations = new Dictionary<string, Dictionary<string, string>>();

        // Segédfüggvény a szótár feltöltéséhez
        void Add(string key, string en, string de)
        {
            translations.Add(key, new Dictionary<string, string>
            {
                { "en", en },
                { "de", de }
            });
        }

        // === FORDÍTÁSOK ===
        // MAIN MENU
        Add("btn_play", "PLAY", "SPIELEN");
        Add("btn_1min", "1 min", "1 min");
        Add("btn_daily", "Daily", "Täglich");
        Add("btn_leaderboard", "Leaderboard", "Bestenliste");
        Add("btn_achievements", "Achievements", "Erfolge");
        Add("btn_shop", "Shop", "Geschäft");
        Add("btn_www", "WWW", "WWW");
        Add("btn_rateUs", "Rate", "Bewerten");
        Add("btn_donate", "$", "$");

        //SPLASH
        Add("txt_loading", "Loading...", "Laden...");

        // BEÁLLÍTÁSOK
        Add("lbl_music", "Music volume", "Musiklautstärke");
        Add("lbl_sfx", "Effects volume", "Effektlautstärke");
        Add("lbl_nickname", "Nickname:", "Spitzname:");
        Add("lbl_language", "Language:", "Sprache:");
        Add("btn_login", "Sign In with Google", "Mit Google anmelden");
        Add("btn_logout", "Sign Out", "Abmelden");
        Add("btn_credits", "Credits", "Mitwirkende");

       
    }
}
