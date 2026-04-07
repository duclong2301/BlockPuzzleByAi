using UnityEngine;

/// <summary>
/// Sets the Main Camera's orthographic size and Y position at startup so that
/// the 10×10 board + piece tray always fit the screen regardless of aspect ratio.
/// Attach to the Main Camera; wire _config and _boardTransform in the inspector.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFitter : MonoBehaviour
{
    [SerializeField] private GameConfigSO _config;
    [SerializeField] private Transform    _boardTransform;

    [Header("Extra padding (world units)")]
    [SerializeField] private float _horizontalPad = 0.5f; // each side
    [SerializeField] private float _topPad        = 1.5f; // space for HUD
    [SerializeField] private float _trayPiecePad  = 2.0f; // approx max piece height

    private void Awake()
    {
        var cam = GetComponent<Camera>();

        float cs    = _config.CellSize;
        float gridW = _config.GridWidth  * cs;
        float gridH = _config.GridHeight * cs;
        float boardY = _boardTransform != null ? _boardTransform.position.y : 0f;

        // Vertical extent of all content
        float contentTop    = boardY + gridH / 2f + _topPad;
        float contentBottom = boardY - gridH / 2f - _config.TrayYOffset - _trayPiecePad;
        float contentCenterY = (contentTop + contentBottom) / 2f;
        float contentHalfH   = (contentTop - contentBottom) / 2f;

        // Minimum ortho size to fit board width (width-constrained in portrait)
        float halfBoardWidth      = gridW / 2f + _horizontalPad;
        float orthoForWidth = halfBoardWidth / cam.aspect; // cam.aspect = screen width/height

        // Use whichever constraint is larger
        cam.orthographicSize = Mathf.Max(orthoForWidth, contentHalfH);

        // Re-center camera vertically on all content
        var p = transform.position;
        transform.position = new Vector3(p.x, contentCenterY, p.z);
    }
}
