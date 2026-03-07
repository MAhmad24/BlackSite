using UnityEngine;
using TMPro;

/// <summary>
/// Displays currency in the HUD. Subscribes to CurrencyManager events
/// so it updates automatically — no polling needed.
/// </summary>
public class CurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI currencyText;

    void Start()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged += UpdateDisplay;
            UpdateDisplay(0, CurrencyManager.Instance.CurrentCurrency);
        }
    }

    void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateDisplay;
        }
    }

    private void UpdateDisplay(int oldValue, int newValue)
    {
        if (currencyText != null)
        {
            currencyText.text = $"${newValue}";
        }
    }
}
