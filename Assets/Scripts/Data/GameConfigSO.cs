using UnityEngine;

/// <summary>
/// Core gameplay configuration. All grid and tray layout values live here.
/// Create instance via: Assets/Data/Config/GameConfigSO.asset
/// </summary>
[CreateAssetMenu(menuName = "BlockPuzzle/Config/GameConfig", fileName = "GameConfigSO")]
public class GameConfigSO : ScriptableObject
{
    [Header("Grid")]
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 10;
    [SerializeField] private float _cellSize = 1f;

    [Header("Piece Tray")]
    [SerializeField] private int _pieceTraySize = 3;
    [SerializeField] private float _trayPieceScale = 0.7f;
    [SerializeField] private float _trayYOffset = 2.5f;

    public int GridWidth  => _gridWidth;
    public int GridHeight => _gridHeight;
    public float CellSize => _cellSize;

    public int PieceTraySize    => _pieceTraySize;
    public float TrayPieceScale => _trayPieceScale;
    public float TrayYOffset    => _trayYOffset;
}
