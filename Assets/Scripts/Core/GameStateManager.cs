using System;
using UnityEngine;

/// <summary>
/// All possible game states.
/// </summary>
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

/// <summary>
/// Finite state machine for the top-level game flow.
/// Other systems listen to OnStateChanged and react accordingly.
///
/// Valid transitions:
///   MainMenu  -> Playing   (StartGame)
///   Playing   -> Paused    (PauseGame)
///   Paused    -> Playing   (ResumeGame)
///   Playing   -> GameOver  (TriggerGameOver)
///   GameOver  -> Playing   (StartGame / restart)
///   GameOver  -> MainMenu  (GoToMainMenu)
///   Playing   -> MainMenu  (GoToMainMenu)
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private GameState _currentState;

    public GameState CurrentState => _currentState;

    /// <summary>Args: (previousState, newState)</summary>
    public event Action<GameState, GameState> OnStateChanged;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private void Start()
    {
        TransitionTo(GameState.MainMenu);
    }

    // ── Transitions ──────────────────────────────────────────────────────────

    /// <summary>Start a new game from MainMenu or restart from GameOver.</summary>
    public void StartGame()
    {
        if (_currentState == GameState.MainMenu || _currentState == GameState.GameOver)
            TransitionTo(GameState.Playing);
    }

    public void PauseGame()
    {
        if (_currentState == GameState.Playing)
            TransitionTo(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (_currentState == GameState.Paused)
            TransitionTo(GameState.Playing);
    }

    public void TriggerGameOver()
    {
        if (_currentState == GameState.Playing)
            TransitionTo(GameState.GameOver);
    }

    public void GoToMainMenu()
    {
        if (_currentState == GameState.GameOver || _currentState == GameState.Playing)
            TransitionTo(GameState.MainMenu);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private void TransitionTo(GameState newState)
    {
        var previous = _currentState;
        _currentState = newState;
        OnStateChanged?.Invoke(previous, newState);
    }
}
