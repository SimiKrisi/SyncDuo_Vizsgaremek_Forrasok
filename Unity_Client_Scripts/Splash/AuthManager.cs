using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// Az autentikáció és felhasználói munkamenet kezelését végző osztály.
/// Kezeli a bejelentkezést, auto-login-t, guest módot és kijelentkezést.
/// </summary>
public class AuthManager : MonoBehaviour
{
    #region Konstansok

    //private const string ServerBaseUrl = "http://localhost:8000";
    private const string GuestIdKey = "Local_Guest_ID";
    private const string GuestPlayerName = "Guest_Player";
    private const string GuestNamePrefix = "Guest_";
    private const string DefaultGuestUserName = "Guest";
    private const string MainMenuSceneName = "MainMenuScene";
    private const int SplashSceneBuildIndex = 0;
    private const int GuestIdMinRange = 1000;
    private const int GuestIdMaxRange = 9999;

    private const string GoogleWebAPI = "74617534370-jknfagv4fvojqfc0l245d9nhivm284pp.apps.googleusercontent.com";

    #endregion

    #region Singleton

    public static AuthManager Instance;

    #endregion

    #region Publikus Property-k

    public bool IsLoggedIn { get; private set; } = false;
    public string CurrentUserName { get; private set; }

    public FirebaseAuth auth;
    public FirebaseUser CurrentUser;

    private bool isFirebaseInitialized = false;
    private bool isGoogleConfigured = false;

    public event Action<bool> OnAuthStateChanged;

    #endregion

    #region Unity Életciklus Metódusok

    /// <summary>
    /// Singleton inicializálása és perzisztencia beállítása.
    /// Helyi adat betöltése és auto-login ellenőrzés intro után.
    /// </summary>
    void Awake()
    {
        InitializeSingleton();
        //StartCoroutine(InitializeWithIntro());
       
    }
    void Start()
    {
        if(IsCurrentSceneSplash())
        {
            StartCoroutine(InitializeWithIntro());
        }
        
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Inicializálás

    /// <summary>
    /// Inicializálás intro hangeffekttel és várakozással.
    /// </summary>
    private IEnumerator InitializeWithIntro()
    {
        PlayIntroSound();

        yield return new WaitForSeconds(1f);

        LoadLocalGameData();
        InitializeFirebase();
        //AttemptAutoLoginOrInitializeGuest();
        //RecordLoginToday();
        //LocalizationManager.Instance.Localise();
        //ProceedToMainMenu();
    }

    /// <summary>
    /// Lejátssza az intro hangeffektet.
    /// </summary>
    private void PlayIntroSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayIntroSFX();
        }
    }

    /// <summary>
    /// Betölti a helyi játékadatokat.
    /// </summary>
    private void LoadLocalGameData()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.LoadLocalData();
        }
    }
    private void RecordLoginToday()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.RecordLogin();
        }
    }
    #endregion
    #region Rirebase és Hitelesítési állapotkezelése

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                isFirebaseInitialized = true;

                // Feliratkozunk a be/kijelentkezés eseményre
                auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null); // Kezdeti állapot ellenőrzése
            }
            else
            {
                Debug.LogError($"Nem sikerült betölteni a Firebase függőségeket: {task.Result}");
                InitializeGuestMode(); // Ha a Firebase valamiért összeomlik, kapjon a játékos Guest módot
            }

            // Miután kiderült, hogy ki a játékos (Google vagy Guest), befejezzük az indulást
            RecordLoginToday();
            if (LocalizationManager.Instance != null) LocalizationManager.Instance.Localise();
            ProceedToMainMenu();
        });
    }

    
    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth != null)
        {
            CurrentUser = auth.CurrentUser;

            if (CurrentUser != null)
            {
                // A Firebase emlékszik a játékosra (Auto-login), vagy most jelentkezett be
                CompleteSuccessfulLogin(CurrentUser.DisplayName);
                OnAuthStateChanged?.Invoke(true);
            }
            else
            {
                // Nincs bejelentkezve senki -> Vendég (Guest) módot indítunk
                InitializeGuestMode();
                OnAuthStateChanged?.Invoke(false);
            }
        }
    }
    private void CompleteSuccessfulLogin(string username)
    {
        IsLoggedIn = true;
        CurrentUserName = username;
        Debug.Log($"SIKERES GOOGLE BEJELENTKEZÉS: {CurrentUserName}");

        SyncProfileToCloud();
    }
    private async void SyncProfileToCloud()
    {
        if (!IsGameDataManagerValid()) return;
        GameDataManager.Instance.SyncDataToCloud();
    }
    #endregion
    #region Google Bejelentkezés Hívása (UI-ból)

    public void LoginWithGoogle(Action<bool, string> onCompleteResult)
    {

#if UNITY_EDITOR
        Debug.LogWarning("<b>[EDITOR MÓD]</b> A Google Sign-In szimulálása");
        IsLoggedIn = true;
        CurrentUserName = "Editor_Google_User1";
        OnAuthStateChanged?.Invoke(true);
        SyncProfileToCloud();
        onCompleteResult?.Invoke(true, "Sikeres bejelentkezés (Editor mód).");
        return;
#endif
        if (!isFirebaseInitialized)
        {
            onCompleteResult?.Invoke(false, "A szerver kapcsolat még inicializálódik.");
            return;
        }

        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true,
            RequestEmail = true,
            UseGameSignIn = false
        };
        isGoogleConfigured = true;
        

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

        signIn.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled) onCompleteResult?.Invoke(false, "Google Sign-In megszakítva.");
            else if (task.IsFaulted) onCompleteResult?.Invoke(false, $"Hiba: {task.Exception.Flatten().InnerExceptions[0].Message}");
            else
            {
                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
                {
                    if (authTask.IsCanceled) onCompleteResult?.Invoke(false, "Firebase Sign-In megszakítva.");
                    else if (authTask.IsFaulted) onCompleteResult?.Invoke(false, $"Hiba: {authTask.Exception.Flatten().InnerExceptions[0].Message}");
                    else
                    {
                        onCompleteResult?.Invoke(true, "Sikeres bejelentkezés!");
                    }
                });
            }
        });
    }

    #endregion
    #region Guest Mód (Megőrizve!)

    public void InitializeGuestMode()
    {
        IsLoggedIn = false;

        if (HasExistingProfile())
        {
            UseExistingProfile();
        }
        else
        {
            CreateNewGuestProfile();
        }

//      UpdateGameDataProfileName();
    }

    private bool HasExistingProfile()
    {
        return IsGameDataManagerValid() && !IsDefaultGuestProfile();
    }

    private bool IsDefaultGuestProfile()
    {
        return GameDataManager.Instance.currentProfile.userName == DefaultGuestUserName;
    }

    private void UseExistingProfile()
    {
        CurrentUserName = GameDataManager.Instance.currentProfile.userName;
        Debug.Log($"🟠 OFFLINE MÓD (Létező profil): {CurrentUserName}");
    }

    private void CreateNewGuestProfile()
    {
        if (HasSavedGuestId())
        {
            CurrentUserName = GuestPlayerName;
        }
        else
        {
            string newGuestId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(GuestIdKey, newGuestId);
            PlayerPrefs.Save();

            CurrentUserName = GuestNamePrefix + UnityEngine.Random.Range(GuestIdMinRange, GuestIdMaxRange);
        }

        Debug.Log($"🟠 ÚJ GUEST MÓD: {CurrentUserName}");
    }

    private bool HasSavedGuestId()
    {
        return PlayerPrefs.HasKey(GuestIdKey);
    }

    #endregion

    #region Adatfrissítés és Scene Navigáció

    private bool IsGameDataManagerValid()
    {
        return GameDataManager.Instance != null && GameDataManager.Instance.currentProfile != null;
    }

    private void UpdateGameDataProfileName()
    {
        if (IsGameDataManagerValid())
        {
            GameDataManager.Instance.currentProfile.userName = CurrentUserName;
        }
    }

    private void TriggerCloudSync()
    {
        GameDataManager.Instance.SyncDataToCloud();
    }

    private void ProceedToMainMenu()
    {
        if (IsCurrentSceneSplash())
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }
    }

    private bool IsCurrentSceneSplash()
    {
        return SceneManager.GetActiveScene().buildIndex == SplashSceneBuildIndex;
    }

    #endregion

    #region Kijelentkezés

    public void SignOut()
    {
#if UNITY_EDITOR
        Debug.LogWarning("<b>[EDITOR MÓD]</b> Kijelentkezés szimulálása...");

        // Visszaállítjuk a rendszert Vendég (Guest) módba
        InitializeGuestMode();

        // Szólunk a Settings UI-nak, hogy frissüljön (hozza vissza a Login gombot)
        OnAuthStateChanged?.Invoke(false);

        Debug.Log("Szimulált kijelentkezés sikeres. Visszatérés Vendég módba.");

        return; // ITT KILÉPÜNK! Nem futtatjuk az Androidos natív kódokat.
#endif
        if (!isGoogleConfigured)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = GoogleWebAPI,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = false
            };
            isGoogleConfigured = true;
        }
        if (GoogleSignIn.DefaultInstance != null) GoogleSignIn.DefaultInstance.SignOut();

        if (auth != null)
        {
            auth.SignOut();
            // Amint az auth.SignOut() lefut, a Firebase AuthStateChanged azonnal észreveszi,
            // és automatikusan elindítja a Vendég (Guest) módot!
        }
        isGoogleConfigured = false;
        Debug.Log("Sikeres Kijelentkezés. Visszatérés Vendég módba.");
    }

    #endregion
}