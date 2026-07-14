using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Centralized currency system. All currency changes go through here.
///
/// Usage:
///   CurrencyManager.Instance.AddCurrency(100, "kill_basic");
///   CurrencyManager.Instance.SpendCurrency(950, "mystery_box");
///   int current = CurrencyManager.Instance.CurrentCurrency;
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    /// <summary>Fires when currency changes. Parameters: (oldValue, newValue)</summary>
    public event Action<int, int> OnCurrencyChanged;

    /// <summary>Fires when currency is earned. Parameters: (amount, reason)</summary>
    public event Action<int, string> OnCurrencyEarned;

    /// <summary>Fires when currency is spent. Parameters: (amount, reason)</summary>
    public event Action<int, string> OnCurrencySpent;

    private int currentCurrency = 0;
    private int sessionEarnings = 0;
    private int sessionSpending = 0;
    private List<CurrencyTransaction> transactionLog = new List<CurrencyTransaction>();

    public int CurrentCurrency => currentCurrency;
    public int SessionEarnings => sessionEarnings;
    public int SessionSpending => sessionSpending;
    public int SessionNet => sessionEarnings - sessionSpending;

    public const int CURRENCY_PER_HIT = 10;
    public const int CURRENCY_PER_KILL = 100;
    public const int CURRENCY_PER_MELEE_KILL = 115;
    public const int CURRENCY_PER_WAVE_BONUS = 50;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewSession()
    {
        sessionEarnings = 0;
        sessionSpending = 0;
        transactionLog.Clear();
    }

    /// <summary>
    /// Add currency. All earning goes through here.
    /// </summary>
    public void AddCurrency(int amount, string reason)
    {
        if (amount <= 0) return;

        int oldValue = currentCurrency;
        currentCurrency += amount;
        sessionEarnings += amount;

        LogTransaction(amount, reason);

        OnCurrencyEarned?.Invoke(amount, reason);
        OnCurrencyChanged?.Invoke(oldValue, currentCurrency);

        #if UNITY_EDITOR
        Debug.Log($"[Currency] +{amount} ({reason}) | Total: {currentCurrency}");
        #endif
    }

    /// <summary>
    /// Spend currency. Returns true if successful, false if not enough.
    /// </summary>
    public bool SpendCurrency(int amount, string reason)
    {
        if (amount <= 0) return false;
        if (currentCurrency < amount) return false;

        int oldValue = currentCurrency;
        currentCurrency -= amount;
        sessionSpending += amount;

        LogTransaction(-amount, reason);

        OnCurrencySpent?.Invoke(amount, reason);
        OnCurrencyChanged?.Invoke(oldValue, currentCurrency);

        #if UNITY_EDITOR
        Debug.Log($"[Currency] -{amount} ({reason}) | Total: {currentCurrency}");
        #endif

        return true;
    }

    public bool CanAfford(int amount)
    {
        return currentCurrency >= amount;
    }

    public void SaveCurrency()
    {
        PlayerPrefs.SetInt("player_currency", currentCurrency);
        PlayerPrefs.Save();

        #if UNITY_EDITOR
        Debug.Log($"[Currency] Saved: {currentCurrency}");
        #endif
    }

    public void LoadCurrency()
    {
        currentCurrency = PlayerPrefs.GetInt("player_currency", 0);
        OnCurrencyChanged?.Invoke(0, currentCurrency);

        #if UNITY_EDITOR
        Debug.Log($"[Currency] Loaded: {currentCurrency}");
        #endif
    }

    public void OnExtractionSuccess()
    {
        SaveCurrency();
    }

    /// <summary>
    /// Call when player dies. Loses a percentage of session earnings.
    /// </summary>
    public void OnPlayerDeath(float lossPercent = 0.5f)
    {
        int loss = Mathf.RoundToInt(sessionEarnings * lossPercent);
        currentCurrency -= loss;
        if (currentCurrency < 0) currentCurrency = 0;

        LogTransaction(-loss, "death_penalty");
        SaveCurrency();

        #if UNITY_EDITOR
        Debug.Log($"[Currency] Death penalty: -{loss} | Remaining: {currentCurrency}");
        #endif
    }

    private void LogTransaction(int amount, string reason)
    {
        transactionLog.Add(new CurrencyTransaction
        {
            amount = amount,
            reason = reason,
            timestamp = Time.time
        });
    }
}

[System.Serializable]
public class CurrencyTransaction
{
    public int amount;
    public string reason;
    public float timestamp;
}
