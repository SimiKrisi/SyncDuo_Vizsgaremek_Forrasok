using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.Networking;


public class LoginWithGoogle : MonoBehaviour
{
    public string GoogleAPI = "74617534370-jknfagv4fvojqfc0l245d9nhivm284pp.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    public TextMeshProUGUI Username, UserEmail;
    public Image UserProfilePic;
    public Button LoginButton; 
    public Sprite defaultSprite;
    public GameObject LoginPanel;
    public GameObject LogoutPanel;

    private bool isSigningIn = false;
    private string imageUrl;
    private bool isGoogleSignInInitialized = false;
    private void Start()
    {
        InitFirebase();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase sikeresen inicializálva és készen áll!");
                CheckIfUserIsLoggedIn();
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Nem sikerült betölteni a Firebase függőségeket: {0}", dependencyStatus));
            }
        });
    }
    private void CheckIfUserIsLoggedIn()
    {
        if (auth.CurrentUser != null)
        {
            // A felhasználó már be van jelentkezve!
            user = auth.CurrentUser;
            Debug.LogFormat("Visszatérő felhasználó azonosítva: {0} ({1})", user.DisplayName, user.UserId);

            // Beállítjuk a UI-t bejelentkezett állapotra
            LoginPanel.SetActive(false);
            LogoutPanel.SetActive(true);
            Username.text = user.DisplayName;
            UserEmail.text = user.Email;

            string photoUrl = user.PhotoUrl != null ? user.PhotoUrl.ToString() : null;
            StartCoroutine(LoadImage(CheckImageUrl(photoUrl)));
        }
        else
        {
            // Nincs bejelentkezve, alapállapot
            Debug.Log("Nincs bejelentkezett felhasználó.");
            LoginPanel.SetActive(true);
            LogoutPanel.SetActive(false);
        }
    }
    public void Login()
    {
        if (isSigningIn) return;

        isSigningIn = true; 
        if (LoginButton != null) LoginButton.interactable = false;
        Debug.Log("--- GOOGLE LOGIN FOLYAMAT ELINDULT ---");
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = GoogleAPI,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = false
            };
            isGoogleSignInInitialized = true;
        }

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();


        signIn.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                Debug.Log("Google Sign-In canceled.");
                isSigningIn = false; 
                if (LoginButton != null) LoginButton.interactable = true;
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
                Debug.LogError("Google Sign-In encountered an error: " + task.Exception);
                isSigningIn = false; 
                if (LoginButton != null) LoginButton.interactable = true;
            }
            else
            {
                GoogleSignInUser googleUser = task.Result;
                string idToken = googleUser.IdToken;
                imageUrl = googleUser.ImageUrl?.ToString(); // Biztonságos null ellenőrzés
                Debug.Log("Google token sikeresen lekérve. Kapcsolódás a Firebase-hez...");
                Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

               
                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        Debug.Log("Firebase Sign-In canceled.");
                        signInCompleted.SetCanceled();
                        isSigningIn = false; 
                        if (LoginButton != null) LoginButton.interactable = true;
                    }
                    else if (authTask.IsFaulted)
                    {
                        Debug.LogError("Firebase Sign-In encountered an error: " + authTask.Exception);
                        signInCompleted.SetException(authTask.Exception);
                        isSigningIn = false; 
                        if (LoginButton != null) LoginButton.interactable = true;
                    }
                    else
                    {
                        user = auth.CurrentUser;
                        Debug.LogFormat("Firebase user signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                        LoginPanel.SetActive(false);
                        LogoutPanel.SetActive(true);
                        // Mivel a Main Thread-en vagyunk, a UI frissítése és a Coroutine most már működni fog!
                        Username.text = user.DisplayName;
                        UserEmail.text = user.Email;

                        string photoUrl = user.PhotoUrl != null ? user.PhotoUrl.ToString() : null;
                        StartCoroutine(LoadImage(CheckImageUrl(photoUrl)));

                        signInCompleted.SetResult(user);
                        isSigningIn = false;
                        if (LoginButton != null) LoginButton.interactable = true;
                    }
                });
            }
        });
    }
    public void SignOut()
    {
        if (GoogleSignIn.DefaultInstance != null)
        {
            GoogleSignIn.DefaultInstance.SignOut();
            // GoogleSignIn.DefaultInstance.Disconnect(); // Ezt is használhatod, ez teljesen visszavonja az app jogosultságait a Google fióktól
        }

        
        if (auth != null)
        {
            auth.SignOut();
        }

        Debug.Log("Sikeresen kijelentkezve!");


        Username.text = "Nincs bejelentkezve";
        UserEmail.text = "";
        UserProfilePic.sprite = defaultSprite;
        LoginPanel.SetActive(true);
        LogoutPanel.SetActive(false);

        isGoogleSignInInitialized = false;
    }
    private string CheckImageUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }
        return imageUrl;
    }
    IEnumerator LoadImage(string imageUri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUri);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Debug.Log("Image loaded successfully");
            UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("Failed to load image: " + www.error);
        }
    }
}
