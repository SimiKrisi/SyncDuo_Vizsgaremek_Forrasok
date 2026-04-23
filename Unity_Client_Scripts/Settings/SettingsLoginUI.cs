using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// A Beállítások képernyő (Settings Scene) UI elemeit vezérlő szkript.
/// Kizárólag a gombok kattintását és a szövegek frissítését végzi, 
/// a nehéz munkát az AuthManager intézi a háttérben.
/// </summary>
public class SettingsLoginUI : MonoBehaviour
{
    [Header("UI Elemek")]
    public GameObject LoginPanel;   // A panel, ami a "Sign In" gombot tartalmazza
    public GameObject LogoutPanel;  // A panel, ami a Profilt és a "Sign Out" gombot tartalmazza
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI UserEmailText;
    public Image UserProfilePic;

    [Header("Beállítások")]
    public Button ActionButton;     // A nagy gombod (Sign In / Sign Out)
    public Sprite defaultSprite;    // Az alapértelmezett kék fej profilkép

    private void Start()
    {
        // 1. Amikor a beállítások betölt, megkérdezzük a Managert, hogy JELENLEG mi a helyzet
        bool isCurrentlyLoggedIn = AuthManager.Instance.IsLoggedIn;
        UpdateUI(isCurrentlyLoggedIn);

        // 2. Feliratkozunk, hogy ha KÉSŐBB a játékos be- vagy kijelentkezik, a UI automatikusan frissüljön
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnAuthStateChanged += UpdateUI;
        }
    }

    private void OnDestroy()
    {
        // Takarítás a scene bezárásakor: leiratkozunk az eseményről, hogy ne legyen memóriaszivárgás
        if (AuthManager.Instance != null)
        {
            AuthManager.Instance.OnAuthStateChanged -= UpdateUI;
        }
    }

    /// <summary>
    /// Frissíti a képernyőt a bejelentkezési állapot alapján.
    /// Ezt a függvényt az AuthManager eseménye is meghívja!
    /// </summary>
    private void UpdateUI(bool isLoggedIn)
    {
        // Kapcsolgatjuk a paneleket
        LoginPanel.SetActive(!isLoggedIn);
        LogoutPanel.SetActive(isLoggedIn);

        if (isLoggedIn && AuthManager.Instance.CurrentUser != null)
        {
            // Ha be van jelentkezve, kiírjuk a Google adatokat
            UsernameText.text = AuthManager.Instance.CurrentUser.DisplayName;
            UserEmailText.text = AuthManager.Instance.CurrentUser.Email;

            // Kép letöltés indítása (csak ha van URL)
            string photoUrl = AuthManager.Instance.CurrentUser.PhotoUrl?.ToString();
            if (!string.IsNullOrEmpty(photoUrl))
            {
                StartCoroutine(LoadProfileImage(photoUrl));
            }
        }
        else
        {
            // Ha vendég (Guest) módban van
            UsernameText.text = AuthManager.Instance.CurrentUserName; // Ez kiírja, hogy pl. "Guest_1234"
            UserEmailText.text = "Offline mód";
            UserProfilePic.sprite = defaultSprite;
        }

        // Biztosítjuk, hogy a gomb újra kattintható legyen
        ActionButton.interactable = true;
    }

    /// <summary>
    /// Ezt a függvényt húzd rá az Inspectorban a LOGIN gombod "On Click()" eseményére!
    /// </summary>
    public void OnLoginButtonClicked()
    {
        ActionButton.interactable = false; // Gomb lezárása a spam ellen!
        UsernameText.text = "Bejelentkezés...";

        // Szólunk a Managernek a SplashScene-ben, hogy intézze a Google piszkos munkáját
        AuthManager.Instance.LoginWithGoogle((isSuccess, errorMessage) =>
        {
            // Ez a kódblokk akkor fut le, amikor a Manager végzett (akár sikerrel, akár hibával)
            if (!isSuccess)
            {
                Debug.LogWarning($"Bejelentkezés sikertelen: {errorMessage}");
                ActionButton.interactable = true; // Hiba esetén oldjuk fel a gombot
                UpdateUI(false); // Visszaállítjuk a UI-t alapállapotba
            }
            // Siker esetén a Manager úgyis elsüti az 'OnAuthStateChanged' eseményt, ami le fogja futtatni az UpdateUI(true)-t!
        });
    }

    /// <summary>
    /// Ezt a függvényt húzd rá az Inspectorban a LOGOUT (Kijelentkezés) gombod "On Click()" eseményére!
    /// </summary>
    public void OnSignOutButtonClicked()
    {
        AuthManager.Instance.SignOut();
        // A Manager elsüti az eseményt, a UI pedig automatikusan visszaáll Guest módra.
    }

    /// <summary>
    /// Letölti a profilképet a Google-től.
    /// </summary>
    private IEnumerator LoadProfileImage(string imageUri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError($"Nem sikerült betölteni a profilképet: {www.error}");
        }
    }
}