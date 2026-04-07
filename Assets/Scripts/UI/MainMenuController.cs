using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Main Menu screen.
/// Displays the all-time best score and provides a Start button.
/// Active when GameState == MainMenu.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private ScoreSystem      _scoreSystem;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _bestScoreLabel;

    [Header("Buttons")]
    [SerializeField] private Button _startButton;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _startButton.onClick.AddListener(OnStartClicked);
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
        RefreshBestScore();
        UpdateVisibility(_gameStateManager.CurrentState);
    }

    // ── Event handlers ───────────────────────────────────────────────────────

    private void HandleStateChanged(GameState previous, GameState next)
    {
        UpdateVisibility(next);
        if (next == GameState.MainMenu)
            RefreshBestScore();
    }

    private void OnStartClicked()
    {
        _scoreSystem?.ResetScore();
        _gameStateManager.StartGame();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void RefreshBestScore()
    {
        _bestScoreLabel.text = $"BEST: {SaveSystem.LoadBestScore():N0}";
    }

    private void UpdateVisibility(GameState state)
    {
        bool show = state == GameState.MainMenu;
        _canvasGroup.alpha          = show ? 1f : 0f;
        _canvasGroup.interactable   = show;
        _canvasGroup.blocksRaycasts = show;
    }
}
