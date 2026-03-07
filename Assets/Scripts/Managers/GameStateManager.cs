using UnityEngine;
using System;

/// <summary>
/// Single source of truth for game state. Prevents conflicting states
/// (e.g., pausing during extraction, shooting while dead).
///
/// Check state:   if (GameStateManager.Instance.CurrentState == GameState.Playing) { ... }
/// Subscribe:     GameStateManager.Instance.OnStateChanged += HandleStateChange;
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    /// <summary>Fires when state changes. Parameters: (previousState, newState)</summary>
    public event Action<GameState, GameState> OnStateChanged;

    public GameState CurrentState { get; private set; } = GameState.PreGame;
    public GameState PreviousState { get; private set; } = GameState.PreGame;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        SetState(GameState.Playing);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void SetState(GameState newState)
    {
        if (newState == CurrentState) return;

        PreviousState = CurrentState;
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Playing:
            case GameState.Extracting:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
            case GameState.Dead:
            case GameState.Victory:
                Time.timeScale = 0f;
                break;
        }

        OnStateChanged?.Invoke(PreviousState, CurrentState);

        #if UNITY_EDITOR
        Debug.Log($"[GameState] {PreviousState} → {CurrentState}");
        #endif
    }

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused => CurrentState == GameState.Paused;
    public bool IsGameOver => CurrentState == GameState.Dead || CurrentState == GameState.Victory;

    public void TogglePause()
    {
        if (CurrentState == GameState.Paused)
        {
            SetState(PreviousState);
        }
        else if (CurrentState == GameState.Playing || CurrentState == GameState.Extracting)
        {
            SetState(GameState.Paused);
        }
    }
}

public enum GameState
{
    PreGame,
    Playing,
    Paused,
    Extracting,
    Dead,
    Victory
}
