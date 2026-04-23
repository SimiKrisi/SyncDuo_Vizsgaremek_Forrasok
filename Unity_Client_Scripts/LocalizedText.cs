using UnityEngine;
using TMPro; // Fontos a TextMeshPro-hoz!

public class LocalizedText : MonoBehaviour
{
    public string translationKey; // Ezt az Inspectorban írod be (pl. "btn_login")

    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        // Feliratkozunk a nyelvváltásra
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            UpdateText(); // Azonnal frissítjük indításkor is
        }
    }

    void OnDestroy()
    {
        // Fontos: Leiratkozunk, ha megsemmisül az objektum, hogy ne legyen hiba
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    void UpdateText()
    {
        if (textComponent != null && LocalizationManager.Instance != null)
        {
            textComponent.text = LocalizationManager.Instance.GetTranslation(translationKey);
        }
    }
}