using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Result returned by GridModel.PlacePiece().
/// All fields populated only when Success = true.
/// </summary>
public struct PlacementResult
{
    public bool      Success;
    public int       CellsPlaced;
    public int       LinesCleared;
    public List<int> ClearedRows;
    public List<int> ClearedCols;
}

/// <summary>
/// Pure C# model of the 10×10 board state.
/// No Unity dependencies except Vector2Int.
/// All visual updates are handled by GridView — this class owns only logical state.
/// </summary>
public class GridModel
{
    private readonly bool[,] _cells;

    public int Width  { get; }
    public int Height { get; }

    /// <summary>Fired after lines are cleared. Args: cleared row indices, cleared column indices.</summary>
    public event Action<List<int>, List<int>> OnLinesCleared;

    public GridModel(int width, int height)
    {
        Width  = width;
        Height = height;
        _cells = new bool[width, height];
    }

    public bool IsOccupied(int x, int y) => _cells[x, y];

    public bool IsInBounds(int x, int y) =>
        x >= 0 && x < Width && y >= 0 && y < Height;

    /// <summary>
    /// Returns true if every cell in (offsets + origin) is in bounds and currently empty.
    /// </summary>
    public bool CanPlace(Vector2Int[] offsets, Vector2Int origin)
    {
        foreach (var offset in offsets)
        {
            int x = origin.x + offset.x;
            int y = origin.y + offset.y;
            if (!IsInBounds(x, y) || _cells[x, y])
                return false;
        }
        return true;
    }

    /// <summary>
    /// Places the piece, then checks and clears full rows/columns.
    /// Returns a PlacementResult with details; Result.Success = false if placement is invalid.
    /// </summary>
    public PlacementResult PlacePiece(Vector2Int[] offsets, Vector2Int origin)
    {
        if (!CanPlace(offsets, origin))
            return new PlacementResult { Success = false };

        foreach (var offset in offsets)
            _cells[origin.x + offset.x, origin.y + offset.y] = true;

        var clearedRows = new List<int>();
        var clearedCols = new List<int>();
        CollectFullLines(clearedRows, clearedCols);
        ClearLines(clearedRows, clearedCols);

        if (clearedRows.Count > 0 || clearedCols.Count > 0)
            OnLinesCleared?.Invoke(clearedRows, clearedCols);

        return new PlacementResult
        {
            Success      = true,
            CellsPlaced  = offsets.Length,
            LinesCleared = clearedRows.Count + clearedCols.Count,
            ClearedRows  = clearedRows,
            ClearedCols  = clearedCols
        };
    }

    /// <summary>
    /// Returns true if at least one grid position exists where the given offsets fit.
    /// Used for game-over detection.
    /// </summary>
    public bool HasAnyValidPlacement(Vector2Int[] offsets)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                if (CanPlace(offsets, new Vector2Int(x, y)))
                    return true;
        return false;
    }

    /// <summary>Resets all cells to empty. Called at the start of a new game.</summary>
    public void Reset()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _cells[x, y] = false;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void CollectFullLines(List<int> rows, List<int> cols)
    {
        for (int y = 0; y < Height; y++)
        {
            bool full = true;
            for (int x = 0; x < Width; x++)
                if (!_cells[x, y]) { full = false; break; }
            if (full) rows.Add(y);
        }

        for (int x = 0; x < Width; x++)
        {
            bool full = true;
            for (int y = 0; y < Height; y++)
                if (!_cells[x, y]) { full = false; break; }
            if (full) cols.Add(x);
        }
    }

    private void ClearLines(List<int> rows, List<int> cols)
    {
        foreach (int row in rows)
            for (int x = 0; x < Width; x++)
                _cells[x, row] = false;

        foreach (int col in cols)
            for (int y = 0; y < Height; y++)
                _cells[col, y] = false;
    }
}
