using System;
using UnityEngine;

/// <summary>
/// Tracks current score and best score.
/// Combo streak increments when placements consecutively clear lines.
/// All scoring values come from ScoringConfigSO — nothing is hardcoded here.
/// </summary>
public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private ScoringConfigSO _config;

    private int _currentScore;
    private int _bestScore;
    private int _comboCount;   // number of consecutive placements that cleared ≥1 line

    public int CurrentScore => _currentScore;
    public int BestScore    => _bestScore;
    public int ComboCount   => _comboCount;

    /// <summary>Fired with the new current score value whenever it changes.</summary>
    public event Action<int> OnScoreChanged;

    /// <summary>Fired with the new best score when the current score surpasses it.</summary>
    public event Action<int> OnBestScoreChanged;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        _bestScore = SaveSystem.LoadBestScore();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveBestScore(_bestScore);
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Awards points for a single piece placement.
    /// Formula: (cells × PointsPerCell) + (lines × PointsPerLineClear) + combo bonus.
    /// </summary>
    public void AddPlacementScore(int cellsPlaced, int linesCleared)
    {
        int points = cellsPlaced * _config.PointsPerCell;

        if (linesCleared > 0)
        {
            _comboCount++;
            points += linesCleared * _config.PointsPerLineClear;
            if (_comboCount > 1)
                points += _config.ComboBaseBonus * (_comboCount - 1);
        }
        else
        {
            _comboCount = 0;
        }

        _currentScore += points;
        OnScoreChanged?.Invoke(_currentScore);

        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;
            SaveSystem.SaveBestScore(_bestScore);
            OnBestScoreChanged?.Invoke(_bestScore);
        }
    }

    /// <summary>Resets current score and combo for a new game. Best score is preserved.</summary>
    public void ResetScore()
    {
        _currentScore = 0;
        _comboCount   = 0;
        OnScoreChanged?.Invoke(0);
    }
}
