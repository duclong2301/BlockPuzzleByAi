using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orchestrates the board: owns GridModel, coordinates with GridView and ScoreSystem.
/// Exposes TryPlacePiece() for PieceDragController and preview helpers.
/// </summary>
public class BoardController : MonoBehaviour
{
    [SerializeField] private GameConfigSO     _config;
    [SerializeField] private GridView         _gridView;
    [SerializeField] private ScoreSystem      _scoreSystem;
    [SerializeField] private GameStateManager _gameStateManager;

    private GridModel _gridModel;

    /// <summary>Fired after a piece is successfully placed on the grid.</summary>
    public event Action<PieceData> OnPiecePlaced;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        _gridModel = new GridModel(_config.GridWidth, _config.GridHeight);
    }

    private void OnEnable()
    {
        if (_gameStateManager != null)
            _gameStateManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (_gameStateManager != null)
            _gameStateManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState previous, GameState next)
    {
        if (next == GameState.Playing)
            ResetBoard();
    }

    /// <summary>Clears the grid model and view to start a fresh game.</summary>
    public void ResetBoard()
    {
        _gridModel.Reset();
        _gridView.ClearAll();
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Attempts to place the piece on the grid at the computed origin.
    /// Updates GridView and ScoreSystem on success.
    /// Returns true if placement was valid and executed.
    /// </summary>
    public bool TryPlacePiece(PieceData piece, Vector2Int origin)
    {
        var result = _gridModel.PlacePiece(piece.Cells, origin);
        if (!result.Success)
            return false;

        // Update visuals: occupied cells
        foreach (var offset in piece.Cells)
            _gridView.SetCellOccupied(origin.x + offset.x, origin.y + offset.y, piece.Color);

        // Update visuals: cleared lines
        foreach (int row in result.ClearedRows) _gridView.ClearRow(row);
        foreach (int col in result.ClearedCols) _gridView.ClearColumn(col);

        _scoreSystem.AddPlacementScore(result.CellsPlaced, result.LinesCleared);
        OnPiecePlaced?.Invoke(piece);

        return true;
    }

    /// <summary>Returns true if the piece can be placed at the given origin.</summary>
    public bool CanPlace(PieceData piece, Vector2Int origin) =>
        _gridModel.CanPlace(piece.Cells, origin);

    /// <summary>Returns true if the piece can be placed anywhere on the grid.</summary>
    public bool CanPlaceAnywhere(PieceData piece) =>
        _gridModel.HasAnyValidPlacement(piece.Cells);

    /// <summary>Converts a world position to the nearest grid cell index.</summary>
    public Vector2Int WorldToGridPos(Vector3 worldPos) =>
        _gridView.WorldToGrid(worldPos);

    // ── Drag preview ─────────────────────────────────────────────────────────

    public void ShowPreview(PieceData piece, Vector2Int origin, bool valid)
    {
        _gridView.ShowPreview(piece.Cells, origin, valid, _gridModel);
    }

    public void ClearPreview()
    {
        _gridView.ClearPreview(_gridModel);
    }
}
