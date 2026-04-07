using UnityEngine;

/// <summary>
/// Scoring formula configuration. All score values live here — never hardcoded.
/// Create instance via: Assets/Data/Config/ScoringConfigSO.asset
/// </summary>
[CreateAssetMenu(menuName = "BlockPuzzle/Config/ScoringConfig", fileName = "ScoringConfigSO")]
public class ScoringConfigSO : ScriptableObject
{
    [Tooltip("Points awarded per cell occupied when a piece is placed.")]
    [SerializeField] private int _pointsPerCell = 1;

    [Tooltip("Additional points per line (row or column) cleared in a single placement.")]
    [SerializeField] private int _pointsPerLineClear = 10;

    [Tooltip("Base bonus multiplied by (comboCount - 1) for consecutive line clears.")]
    [SerializeField] private int _comboBaseBonus = 20;

    public int PointsPerCell      => _pointsPerCell;
    public int PointsPerLineClear => _pointsPerLineClear;
    public int ComboBaseBonus     => _comboBaseBonus;
}
