using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    void Start()
    {
        // 1. Azonnali frissítés induláskor
        UpdateCoinUI();

        // 2. FELIRATKOZÁS: Ha a GameDataManager szól, mi frissítünk
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnCoinBalanceChanged += UpdateCoinUI;
        }
    }

    void OnDestroy()
    {
        // FONTOS: LEIRATKOZÁS (Hogy ne legyen hiba, ha bezárod az ablakot)
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.OnCoinBalanceChanged -= UpdateCoinUI;
        }
    }

    public void UpdateCoinUI()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.currentProfile != null)
        {
            coinText.text = GameDataManager.Instance.currentProfile.coins.ToString() + " $"; // Tettem mögé egy $-jelet is
        }
    }
}