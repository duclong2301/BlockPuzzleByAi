using UnityEngine;

/// <summary>
/// Defines a single polyomino piece shape.
/// Cells are Vector2Int offsets from the piece's origin (0,0) = bottom-left of bounding box.
/// Create instances via: Assets/Data/Pieces/ — one asset per shape.
/// </summary>
[CreateAssetMenu(menuName = "BlockPuzzle/Pieces/PieceShape", fileName = "PieceShapeSO")]
public class PieceShapeSO : ScriptableObject
{
    [Tooltip("Display name for this piece shape (e.g. 'LShape_TR').")]
    [SerializeField] private string _pieceName;

    [Tooltip("Cell offsets from origin (0,0). Bottom-left of bounding box is (0,0).")]
    [SerializeField] private Vector2Int[] _cells;

    [Tooltip("Color used for this piece's sprite renderer.")]
    [SerializeField] private Color _color = Color.white;

    public string       PieceName => _pieceName;
    public Vector2Int[] Cells     => _cells;
    public Color        Color     => _color;
}
