using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates and manages the 10×10 cell visual grid.
/// Owns cell occupancy colors (visual layer).
/// Provides world↔grid coordinate conversion used by BoardController and PieceDragController.
/// </summary>
public class GridView : MonoBehaviour
{
    [SerializeField] private GameConfigSO _config;
    [SerializeField] private Color _emptyCellColor = new Color(0.13f, 0.13f, 0.18f, 1f);

    private CellView[,] _cells;
    private Color[,]    _cellColors;    // tracks piece color per occupied cell
    private Vector3     _gridOrigin;    // world position of cell (0,0)'s center

    private readonly List<Vector2Int> _previewCells = new List<Vector2Int>();

    public float CellSize => _config.CellSize;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        BuildGrid();
    }

    private void BuildGrid()
    {
        int   w  = _config.GridWidth;
        int   h  = _config.GridHeight;
        float cs = _config.CellSize;

        // Center the entire grid at the GameObject's position
        _gridOrigin = transform.position + new Vector3(
            -(w * cs) / 2f + cs / 2f,
            -(h * cs) / 2f + cs / 2f,
            0f);

        _cells      = new CellView[w, h];
        _cellColors = new Color[w, h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var go = new GameObject($"Cell_{x}_{y}");
                go.transform.SetParent(transform);
                go.transform.position = GridToWorld(new Vector2Int(x, y));

                var cell = go.AddComponent<CellView>();
                cell.Initialize(cs, _emptyCellColor);

                // Collider for physics raycasts (not used for drag but good for future)
                var col  = go.AddComponent<BoxCollider2D>();
                col.size = Vector2.one * cs;

                _cells[x, y] = cell;
            }
        }
    }

    // ── Coordinate conversion ────────────────────────────────────────────────

    /// <summary>Returns the world-space center of the given grid cell.</summary>
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return _gridOrigin + new Vector3(gridPos.x * CellSize, gridPos.y * CellSize, 0f);
    }

    /// <summary>Returns the nearest grid cell for a world-space position.</summary>
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        var local = worldPos - _gridOrigin;
        return new Vector2Int(
            Mathf.RoundToInt(local.x / CellSize),
            Mathf.RoundToInt(local.y / CellSize));
    }

    public bool IsValidGridPos(Vector2Int pos) =>
        pos.x >= 0 && pos.x < _config.GridWidth &&
        pos.y >= 0 && pos.y < _config.GridHeight;

    // ── Cell state updates ───────────────────────────────────────────────────

    public void SetCellOccupied(int x, int y, Color color)
    {
        _cellColors[x, y] = color;
        _cells[x, y].SetOccupied(color);
    }

    public void ClearCell(int x, int y)
    {
        _cellColors[x, y] = default;
        _cells[x, y].Clear();
    }

    public void ClearRow(int row)
    {
        for (int x = 0; x < _config.GridWidth; x++)
            ClearCell(x, row);
    }

    public void ClearColumn(int col)
    {
        for (int y = 0; y < _config.GridHeight; y++)
            ClearCell(col, y);
    }

    // ── Drag preview ─────────────────────────────────────────────────────────

    /// <summary>
    /// Highlights cells where the dragged piece would land.
    /// Color is green (valid) or red (invalid).
    /// Pass GridModel so cells can be properly restored when preview is cleared.
    /// </summary>
    public void ShowPreview(Vector2Int[] offsets, Vector2Int origin, bool valid, GridModel model)
    {
        ClearPreview(model);

        foreach (var offset in offsets)
        {
            int x = origin.x + offset.x;
            int y = origin.y + offset.y;
            if (!IsValidGridPos(new Vector2Int(x, y))) continue;

            _cells[x, y].SetPreview(valid);
            _previewCells.Add(new Vector2Int(x, y));
        }
    }

    /// <summary>
    /// Restores all cells that were showing a preview back to their actual state.
    /// </summary>
    public void ClearPreview(GridModel model)
    {
        foreach (var pos in _previewCells)
        {
            if (model.IsOccupied(pos.x, pos.y))
                _cells[pos.x, pos.y].SetOccupied(_cellColors[pos.x, pos.y]);
            else
                _cells[pos.x, pos.y].Clear();
        }
        _previewCells.Clear();
    }

    /// <summary>Resets all cells to the empty visual state. Called at the start of a new game.</summary>
    public void ClearAll()
    {
        int w = _config.GridWidth;
        int h = _config.GridHeight;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                _cells[x, y].Clear();
                _cellColors[x, y] = default;
            }
        _previewCells.Clear();
    }
}
