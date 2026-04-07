using UnityEngine;

/// <summary>
/// Something that has discrete cells and can be placed on the grid.
/// Implemented by PieceData to decouple piece logic from placement infrastructure.
/// </summary>
public interface IPlaceable
{
    /// <summary>Cell offsets from the placement origin (0,0).</summary>
    Vector2Int[] Cells { get; }

    /// <summary>Display name of this piece.</summary>
    string Name { get; }
}
