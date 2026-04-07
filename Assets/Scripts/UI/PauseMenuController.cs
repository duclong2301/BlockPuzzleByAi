using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Pause overlay.
/// Provides Resume and Main Menu buttons.
/// Active when GameState == Paused.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateManager _gameStateManager;

    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _resumeButton.onClick.AddListener(OnResumeClicked);
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

        // Pause time while in the pause menu
        Time.timeScale = (next == GameState.Paused) ? 0f : 1f;
    }

    private void OnResumeClicked()
    {
        _gameStateManager.ResumeGame();
    }

    private void OnMainMenuClicked()
    {
        _gameStateManager.GoToMainMenu();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void UpdateVisibility(GameState state)
    {
        bool show = state == GameState.Paused;
        _canvasGroup.alpha          = show ? 1f : 0f;
        _canvasGroup.interactable   = show;
        _canvasGroup.blocksRaycasts = show;
    }
}
