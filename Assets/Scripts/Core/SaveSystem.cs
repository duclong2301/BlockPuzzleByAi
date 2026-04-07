using UnityEngine;

/// <summary>
/// Static utility for persisting game data.
/// Wraps PlayerPrefs — all keys are namespaced to avoid collisions.
/// </summary>
public static class SaveSystem
{
    private const string KeyBestScore = "BlockPuzzle.BestScore";

    public static void SaveBestScore(int score)
    {
        PlayerPrefs.SetInt(KeyBestScore, score);
        PlayerPrefs.Save();
    }

    public static int LoadBestScore() =>
        PlayerPrefs.GetInt(KeyBestScore, 0);

    /// <summary>Wipe all saved data. Use only in development / settings reset.</summary>
    public static void ClearAllData()
    {
        PlayerPrefs.DeleteKey(KeyBestScore);
        PlayerPrefs.Save();
    }
}
