using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonManager : MonoBehaviour
{
    [Header("Name of Scene")]
    public string nameofscene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
    public void OnBackButtonClicked()
    {
        PlayButtonSFX();
        Debug.Log("Back Button Pressed");
        SceneManager.LoadScene(nameofscene);
    }
}
