using UnityEngine;

/// <summary>
/// Visual representation of one grid cell.
/// Implements IClearable — call Clear() to restore empty state.
/// Sprite is generated procedurally via PrimitiveSprite.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class CellView : MonoBehaviour, IClearable
{
    private static readonly Color GhostValid   = new Color(0.3f, 1f,   0.3f, 0.7f);
    private static readonly Color GhostInvalid = new Color(1f,   0.3f, 0.3f, 0.5f);

    private SpriteRenderer _spriteRenderer;
    private Color          _emptyColor;

    public bool IsEmpty { get; private set; } = true;

    private void Awake()
    {
        _spriteRenderer        = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = PrimitiveSprite.Square;
        _spriteRenderer.sortingOrder = 0;
    }

    /// <summary>
    /// Called once by GridView during grid construction.
    /// </summary>
    public void Initialize(float cellSize, Color emptyColor)
    {
        _emptyColor              = emptyColor;
        transform.localScale     = Vector3.one * (cellSize - 0.06f); // small gap between cells
        _spriteRenderer.color    = emptyColor;
        IsEmpty = true;
    }

    // ── State transitions ────────────────────────────────────────────────────

    public void SetOccupied(Color pieceColor)
    {
        _spriteRenderer.color = pieceColor;
        IsEmpty = false;
    }

    public void Clear()
    {
        _spriteRenderer.color = _emptyColor;
        IsEmpty = true;
    }

    /// <summary>
    /// Show drag-preview highlight. Call CellView.Clear() or SetOccupied() to restore.
    /// </summary>
    public void SetPreview(bool valid)
    {
        _spriteRenderer.color = valid ? GhostValid : GhostInvalid;
    }
}
