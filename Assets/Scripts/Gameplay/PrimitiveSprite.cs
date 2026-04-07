using UnityEngine;

/// <summary>
/// Provides a shared procedurally-generated white square sprite used by CellView and PieceView.
/// Avoids creating 100+ duplicate textures at runtime.
/// </summary>
public static class PrimitiveSprite
{
    private static Sprite _square;

    /// <summary>
    /// A 1×1 unit white square sprite (pixelsPerUnit = 4).
    /// Tint via SpriteRenderer.color. Scale transform to set world size.
    /// </summary>
    public static Sprite Square
    {
        get
        {
            if (_square != null)
                return _square;

            const int Size = 4;
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp
            };

            var pixels = new Color[Size * Size];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;

            tex.SetPixels(pixels);
            tex.Apply();

            _square = Sprite.Create(
                tex,
                new Rect(0, 0, Size, Size),
                new Vector2(0.5f, 0.5f),
                Size  // pixelsPerUnit = Size → sprite is exactly 1 world unit
            );

            return _square;
        }
    }
}
