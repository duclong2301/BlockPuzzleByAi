using UnityEngine;

/// <summary>
/// Immutable runtime data for a single piece.
/// Created from PieceShapeSO via PieceData.FromSO().
/// Implements IPlaceable.
/// </summary>
public class PieceData : IPlaceable
{
    public Vector2Int[] Cells { get; }
    public Color        Color { get; }
    public string       Name  { get; }

    public PieceData(Vector2Int[] cells, Color color, string name)
    {
        Cells = cells;
        Color = color;
        Name  = name;
    }

    public static PieceData FromSO(PieceShapeSO so) =>
        new PieceData(so.Cells, so.Color, so.PieceName);

    // ── Spatial utilities ────────────────────────────────────────────────────

    /// <summary>
    /// The center of this piece's bounding box in cell-local coordinates.
    /// Used to center the visual when dragging.
    /// </summary>
    public Vector2 BoundsCenter
    {
        get
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            foreach (var c in Cells)
            {
                if (c.x < minX) minX = c.x;
                if (c.x > maxX) maxX = c.x;
                if (c.y < minY) minY = c.y;
                if (c.y > maxY) maxY = c.y;
            }
            return new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
        }
    }

    /// <summary>
    /// Given the grid cell under the cursor, returns the piece origin so that
    /// the piece's bounding box center aligns with the cursor cell.
    /// </summary>
    public Vector2Int GetOriginFromCursorGridPos(Vector2Int cursorGridPos)
    {
        var center = BoundsCenter;
        return new Vector2Int(
            cursorGridPos.x - Mathf.RoundToInt(center.x),
            cursorGridPos.y - Mathf.RoundToInt(center.y));
    }
}
