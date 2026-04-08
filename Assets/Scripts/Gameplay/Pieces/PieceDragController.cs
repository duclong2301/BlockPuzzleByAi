using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the active drag gesture: moves the dragged PieceView to follow the cursor,
/// updates the board's ghost preview, and commits or cancels placement on mouse release.
/// Coordinates between PieceView, BoardController, and PieceTrayController.
/// </summary>
public class PieceDragController : MonoBehaviour
{
    [SerializeField] private BoardController    _boardController;
    [SerializeField] private PieceTrayController _trayController;
    [SerializeField] private GameConfigSO       _config;

    private PieceView _draggedPiece;
    private bool      _isDragging;

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Called by PieceView.OnDragStarted to hand control to this controller.</summary>
    public void BeginDrag(PieceView piece)
    {
        _draggedPiece = piece;
        _isDragging   = true;

        piece.SetSortingOrder(2); // render above grid
        piece.transform.localScale = Vector3.one * _config.CellSize;
    }

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private void Update()
    {
        if (!_isDragging) return;

        Vector3    mouseWorld = GetMouseWorldPos();
        Vector2Int cursorGrid = _boardController.WorldToGridPos(mouseWorld);
        Vector2Int origin     = _draggedPiece.Data.GetOriginFromCursorGridPos(cursorGrid);

        // Move visual to follow cursor (piece center at cursor, z above grid)
        _draggedPiece.transform.position = new Vector3(mouseWorld.x, mouseWorld.y, -0.1f);

        // Show preview on grid
        bool valid = _boardController.CanPlace(_draggedPiece.Data, origin);
        _boardController.ShowPreview(_draggedPiece.Data, origin, valid);

        // Check release — prioritise Touchscreen first; Mouse.current can be a
        // virtual mouse on mobile and its button-released event is unreliable.
        bool released = (Touchscreen.current != null &&
                         Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended)
                     || (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame);
        if (released)
            EndDrag(cursorGrid);
    }

    // ── Internal helpers ─────────────────────────────────────────────────────

    private void EndDrag(Vector2Int cursorGrid)
    {
        _isDragging = false;
        _boardController.ClearPreview();

        Vector2Int origin  = _draggedPiece.Data.GetOriginFromCursorGridPos(cursorGrid);
        bool       placed  = _boardController.TryPlacePiece(_draggedPiece.Data, origin);

        if (placed)
        {
            _trayController.ConsumePiece(_draggedPiece);
        }
        else
        {
            _draggedPiece.ReturnToTray();
        }

        _draggedPiece = null;
    }

    /// <summary>
    /// Converts screen input position to world position for an orthographic camera.
    /// Touchscreen is checked first and remains active during TouchPhase.Ended so
    /// that EndDrag still receives the correct final finger position on release.
    /// Falling back to Mouse.current on mobile would read the virtual mouse which
    /// has a stale or zero position the moment the finger lifts.
    /// </summary>
    private static Vector3 GetMouseWorldPos()
    {
        Vector2 screenPos;
        if (Touchscreen.current != null)
        {
            var phase = Touchscreen.current.primaryTouch.phase.ReadValue();
            // Read touch pos for any meaningful phase — including Ended — so the
            // last finger position is used when the drag is committed.
            if (phase != UnityEngine.InputSystem.TouchPhase.None &&
                phase != UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
                Vector3 tp = new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z);
                return Camera.main.ScreenToWorldPoint(tp);
            }
        }
        if (Mouse.current != null)
        {
            screenPos = Mouse.current.position.ReadValue();
            Vector3 mp = new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z);
            return Camera.main.ScreenToWorldPoint(mp);
        }
        return Vector3.zero;
    }
}
