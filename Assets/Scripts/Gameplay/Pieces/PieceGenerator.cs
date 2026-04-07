using UnityEngine;

/// <summary>
/// Draws random pieces from PieceCatalogSO.
/// Pure logic class — no MonoBehaviour. Created by PieceTrayController.
/// </summary>
public class PieceGenerator
{
    private readonly PieceCatalogSO _catalog;

    public PieceGenerator(PieceCatalogSO catalog)
    {
        _catalog = catalog;
    }

    /// <summary>Returns one randomly selected PieceData from the catalog.</summary>
    public PieceData GenerateRandom()
    {
        int index = Random.Range(0, _catalog.Count);
        return PieceData.FromSO(_catalog.GetPiece(index));
    }

    /// <summary>Returns an array of `count` independently random pieces.</summary>
    public PieceData[] GenerateTray(int count)
    {
        var tray = new PieceData[count];
        for (int i = 0; i < count; i++)
            tray[i] = GenerateRandom();
        return tray;
    }
}
