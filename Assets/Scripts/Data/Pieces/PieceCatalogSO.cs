using UnityEngine;

/// <summary>
/// Catalog of all available piece shapes.
/// PieceGenerator draws randomly from this catalog.
/// Create a single instance at: Assets/Data/Pieces/PieceCatalogSO.asset
/// </summary>
[CreateAssetMenu(menuName = "BlockPuzzle/Pieces/PieceCatalog", fileName = "PieceCatalogSO")]
public class PieceCatalogSO : ScriptableObject
{
    [Tooltip("All piece shapes available in the game. Each is equally likely to be selected.")]
    [SerializeField] private PieceShapeSO[] _pieces;

    public int            Count          => _pieces != null ? _pieces.Length : 0;
    public PieceShapeSO[] Pieces         => _pieces;
    public PieceShapeSO   GetPiece(int i) => _pieces[i];
}
