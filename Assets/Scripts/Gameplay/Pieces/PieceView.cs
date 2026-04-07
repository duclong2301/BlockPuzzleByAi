using System;
using UnityEngine;

/// <summary>
/// Visual representation of a single piece in the tray or during dragging.
/// Builds its child-cell GameObjects procedurally from PieceData.
/// Uses OnMouseDown to start a drag via the OnDragStarted event.
/// Requires BoxCollider2D (added in Initialize).
/// </summary>
public class PieceView : MonoBehaviour
{
    /// <summary>Fired when the player presses the mouse button on this piece.</summary>
    public event Action<PieceView> OnDragStarted;

    private PieceData _data;
    private Vector3   _trayPosition;
    private float     _trayScale;
    private bool      _isDraggable = true;

    public PieceData Data => _data;

    // ── Setup ────────────────────────────────────────────────────────────────

    /// <summary>
    /// (Re-)initialises the piece with new data and rebuilds the visual.
    /// cellSize should be the full grid cell size — scale down via SetTrayScale().
    /// </summary>
    public void Initialize(PieceData data, float cellSize)
    {
        _data = data;
        BuildVisual(cellSize);
    }

    private void BuildVisual(float cellSize)
    {
        // Destroy existing child cells
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        var center = _data.BoundsCenter;

        foreach (var cell in _data.Cells)
        {
            var go = new GameObject($"Cell_{cell.x}_{cell.y}");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(
                (cell.x - center.x) * cellSize,
                (cell.y - center.y) * cellSize,
                0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = PrimitiveSprite.Square;
            sr.color        = _data.Color;
            sr.sortingOrder = 1;

            go.transform.localScale = Vector3.one * (cellSize - 0.06f);
        }

        // Collider on root sized to piece bounding box for click detection
        UpdateCollider(cellSize);
    }

    private void UpdateCollider(float cellSize)
    {
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var c in _data.Cells)
        {
            if (c.x < minX) minX = c.x;
            if (c.x > maxX) maxX = c.x;
            if (c.y < minY) minY = c.y;
            if (c.y > maxY) maxY = c.y;
        }

        var col = gameObject.GetComponent<BoxCollider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
        col.size   = new Vector2((maxX - minX + 1) * cellSize, (maxY - minY + 1) * cellSize);
        col.offset = Vector2.zero;
    }

    // ── Position / scale ─────────────────────────────────────────────────────

    public void SetTrayPosition(Vector3 worldPos, float scale)
    {
        _trayPosition    = worldPos;
        _trayScale       = scale;
        transform.position    = worldPos;
        transform.localScale  = Vector3.one * scale;
    }

    public void ReturnToTray()
    {
        transform.position   = _trayPosition;
        transform.localScale = Vector3.one * _trayScale;
        SetSortingOrder(1);
    }

    public void SetSortingOrder(int order)
    {
        foreach (Transform child in transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = order;
        }
    }

    // ── Drag state ───────────────────────────────────────────────────────────

    public void SetDraggable(bool draggable) => _isDraggable = draggable;

    private void OnMouseDown()
    {
        if (_isDraggable)
            OnDragStarted?.Invoke(this);
    }
}
