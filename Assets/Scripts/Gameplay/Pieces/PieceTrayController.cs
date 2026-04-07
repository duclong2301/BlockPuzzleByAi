using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the 3-piece tray: generates pieces, positions PieceViews, and handles
/// game-over detection after each piece is consumed.
/// Sends OnGameOver when no remaining piece can be placed on the board.
/// </summary>
public class PieceTrayController : MonoBehaviour
{
    [SerializeField] private GameConfigSO        _config;
    [SerializeField] private PieceCatalogSO      _catalog;
    [SerializeField] private PieceDragController _dragController;
    [SerializeField] private BoardController     _boardController;
    [SerializeField] private GameStateManager    _gameStateManager;

    private PieceGenerator _generator;
    private PieceView[]    _pieceViews;
    private int            _activeCount;

    /// <summary>Fired every time a fresh tray of pieces is generated.</summary>
    public event Action OnNewTrayGenerated;

    /// <summary>Fired when no remaining tray piece can fit anywhere on the board.</summary>
    public event Action OnGameOver;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private void Awake()
    {
        _generator  = new PieceGenerator(_catalog);
        _pieceViews = new PieceView[_config.PieceTraySize];

        for (int i = 0; i < _config.PieceTraySize; i++)
        {
            var go = new GameObject($"PieceView_{i}");
            go.transform.SetParent(transform);
            _pieceViews[i] = go.AddComponent<PieceView>();
            _pieceViews[i].OnDragStarted += OnPieceDragStarted;
        }
    }

    private void OnEnable()
    {
        if (_gameStateManager != null)
            _gameStateManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (_gameStateManager != null)
            _gameStateManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState previous, GameState next)
    {
        if (next == GameState.Playing)
            GenerateNewTray();
    }

    // ── Tray management ──────────────────────────────────────────────────────

    private void GenerateNewTray()
    {
        float cs        = _config.CellSize;
        float gridW     = _config.GridWidth  * cs;
        float gridH     = _config.GridHeight * cs;
        float trayY     = -(gridH / 2f) - _config.TrayYOffset;
        float spacing   = gridW / (_config.PieceTraySize + 1);
        float startX    = -(gridW / 2f) + spacing;

        _activeCount = _config.PieceTraySize;

        for (int i = 0; i < _config.PieceTraySize; i++)
        {
            var data = _generator.GenerateRandom();
            float x  = startX + i * spacing;

            _pieceViews[i].Initialize(data, cs);
            _pieceViews[i].SetTrayPosition(
                transform.position + new Vector3(x, trayY, 0f),
                _config.TrayPieceScale);
            _pieceViews[i].gameObject.SetActive(true);
            _pieceViews[i].SetDraggable(true);
        }

        OnNewTrayGenerated?.Invoke();
        CheckGameOver();
    }

    /// <summary>
    /// Called by PieceDragController after a successful placement.
    /// Hides the view and triggers a new tray when the last piece is consumed.
    /// </summary>
    public void ConsumePiece(PieceView piece)
    {
        piece.gameObject.SetActive(false);
        _activeCount--;

        if (_activeCount <= 0)
            GenerateNewTray();
        else
            CheckGameOver();
    }

    /// <returns>All PieceData for tray slots that are still active.</returns>
    public List<PieceData> GetRemainingPieces()
    {
        var list = new List<PieceData>(_activeCount);
        foreach (var view in _pieceViews)
            if (view.gameObject.activeSelf)
                list.Add(view.Data);
        return list;
    }

    // ── Internal helpers ─────────────────────────────────────────────────────

    private void OnPieceDragStarted(PieceView piece)
    {
        _dragController.BeginDrag(piece);
    }

    private void CheckGameOver()
    {
        foreach (var view in _pieceViews)
        {
            if (!view.gameObject.activeSelf) continue;
            if (_boardController.CanPlaceAnywhere(view.Data))
                return;
        }
        OnGameOver?.Invoke();
        _gameStateManager?.TriggerGameOver();
    }
}
