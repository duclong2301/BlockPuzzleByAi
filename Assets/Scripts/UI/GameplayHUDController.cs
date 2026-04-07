using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays live score, best score, and a pause button during gameplay.
/// Binds exclusively to events — never polls state each frame.
/// Active when GameState == Playing.
/// </summary>
public class GameplayHUDController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private ScoreSystem      _scoreSystem;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private TextMeshProUGUI _bestScoreLabel;

    [Header("Buttons")]
    [SerializeField] private Button _pauseButton;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _pauseButton.onClick.AddListener(OnPauseClicked);
    }

    private void OnEnable()
    {
        _gameStateManager.OnStateChanged += HandleStateChanged;
        _scoreSystem.OnScoreChanged      += HandleScoreChanged;
        _scoreSystem.OnBestScoreChanged  += HandleBestScoreChanged;
    }

    private void OnDisable()
    {
        _gameStateManager.OnStateChanged -= HandleStateChanged;
        _scoreSystem.OnScoreChanged      -= HandleScoreChanged;
        _scoreSystem.OnBestScoreChanged  -= HandleBestScoreChanged;
    }

    private void Start()
    {
        RefreshLabels();
        UpdateVisibility(_gameStateManager.CurrentState);
    }

    // ── Event handlers ───────────────────────────────────────────────────────

    private void HandleStateChanged(GameState previous, GameState next)
    {
        UpdateVisibility(next);
        if (next == GameState.Playing)
            RefreshLabels();
    }

    private void HandleScoreChanged(int newScore)
    {
        _scoreLabel.text = $"SCORE\n{newScore:N0}";
    }

    private void HandleBestScoreChanged(int newBest)
    {
        _bestScoreLabel.text = $"BEST\n{newBest:N0}";
    }

    private void OnPauseClicked()
    {
        _gameStateManager.PauseGame();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void RefreshLabels()
    {
        _scoreLabel.text     = $"SCORE\n{_scoreSystem.CurrentScore:N0}";
        _bestScoreLabel.text = $"BEST\n{_scoreSystem.BestScore:N0}";
    }

    private void UpdateVisibility(GameState state)
    {
        bool show = state == GameState.Playing;
        _canvasGroup.alpha          = show ? 1f : 0f;
        _canvasGroup.interactable   = show;
        _canvasGroup.blocksRaycasts = show;
    }
}
