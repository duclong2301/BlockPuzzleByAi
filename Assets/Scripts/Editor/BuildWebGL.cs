using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildWebGL
{
    private const string OutputPath = "Builds/WebGL";

    [MenuItem("BlockPuzzle/Build/WebGL")]
    public static void Build()
    {
        // Gather all enabled scenes from Build Settings
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            Debug.LogError("[BuildWebGL] No scenes in Build Settings. Add a scene first.");
            return;
        }

        // Switch to WebGL first so Unity resolves platform-specific assemblies correctly
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            Debug.Log("[BuildWebGL] Switching active build target to WebGL...");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            // SwitchActiveBuildTarget triggers a domain reload — build will need to be re-run after
            Debug.Log("[BuildWebGL] Platform switched. Please run Build/WebGL again.");
            return;
        }

        var options = new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = OutputPath,
            target           = BuildTarget.WebGL,
            targetGroup      = BuildTargetGroup.WebGL,
            options          = BuildOptions.None,
        };

        // WebGL-specific settings
        PlayerSettings.WebGL.compressionFormat   = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.linkerTarget        = WebGLLinkerTarget.Wasm;
        PlayerSettings.productName               = "Block Puzzle";

        Debug.Log($"[BuildWebGL] Building {scenes.Length} scene(s) to '{OutputPath}'...");

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
            Debug.Log($"[BuildWebGL] ✅ Build succeeded! Size: {summary.totalSize / 1024 / 1024} MB → {OutputPath}");
        else
            Debug.LogError($"[BuildWebGL] ❌ Build FAILED: {summary.totalErrors} error(s)");
    }
}
