using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Game Over screen.
/// Shows final score and best score; provides Restart and Main Menu buttons.
/// Active when GameState == GameOver.
/// </summary>
public class GameOverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private ScoreSystem      _scoreSystem;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _finalScoreLabel;
    [SerializeField] private TextMeshProUGUI _bestScoreLabel;

    [Header("Buttons")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _restartButton.onClick.AddListener(OnRestartClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnEnable()
    {
        _gameStateManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        _gameStateManager.OnStateChanged -= HandleStateChanged;
    }

    private void Start()
    {
        UpdateVisibility(_gameStateManager.CurrentState);
    }

    // ── Event handlers ───────────────────────────────────────────────────────

    private void HandleStateChanged(GameState previous, GameState next)
    {
        UpdateVisibility(next);
        if (next == GameState.GameOver)
            RefreshLabels();
    }

    private void OnRestartClicked()
    {
        _scoreSystem.ResetScore();
        _gameStateManager.StartGame();
    }

    private void OnMainMenuClicked()
    {
        _gameStateManager.GoToMainMenu();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void RefreshLabels()
    {
        _finalScoreLabel.text = $"SCORE\n{_scoreSystem.CurrentScore:N0}";
        _bestScoreLabel.text  = $"BEST\n{_scoreSystem.BestScore:N0}";
    }

    private void UpdateVisibility(GameState state)
    {
        bool show = state == GameState.GameOver;
        _canvasGroup.alpha          = show ? 1f : 0f;
        _canvasGroup.interactable   = show;
        _canvasGroup.blocksRaycasts = show;
    }
}
