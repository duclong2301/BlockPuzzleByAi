using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One-time setup script that creates all ScriptableObject assets, folders,
/// and the GameScene with a complete hierarchy.
/// Run via: BlockPuzzle → Setup → Create All Assets & Scene
/// </summary>
public static class BlockPuzzleSetupEditor
{
    private const string DataRoot    = "Assets/Data";
    private const string ConfigPath  = "Assets/Data/Config";
    private const string PiecesPath  = "Assets/Data/Pieces";
    private const string ShapesPath  = "Assets/Data/Pieces/Shapes";
    private const string ScenesPath  = "Assets/Scenes";

    // ─────────────────────────────────────────────────────────────────────────
    // Main entry point
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem("BlockPuzzle/Setup/Create All Assets and Scene")]
    public static void CreateAllAssetsAndScene()
    {
        EnsureFolders();
        var gameConfig    = CreateOrLoadGameConfig();
        var scoringConfig = CreateOrLoadScoringConfig();
        var catalog       = CreateOrLoadCatalogWithShapes();
        CreateGameScene(gameConfig, scoringConfig, catalog);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[BlockPuzzleSetup] ✅ All assets and GameScene created successfully!");
    }

    [MenuItem("BlockPuzzle/Setup/Create ScriptableObjects Only")]
    public static void CreateScriptableObjectsOnly()
    {
        EnsureFolders();
        CreateOrLoadGameConfig();
        CreateOrLoadScoringConfig();
        CreateOrLoadCatalogWithShapes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[BlockPuzzleSetup] ✅ ScriptableObjects created/updated.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Folder creation
    // ─────────────────────────────────────────────────────────────────────────

    private static void EnsureFolders()
    {
        EnsureFolder("Assets",            "Data");
        EnsureFolder(DataRoot,            "Config");
        EnsureFolder(DataRoot,            "Pieces");
        EnsureFolder(PiecesPath,          "Shapes");
        EnsureFolder("Assets",            "Scenes");
        EnsureFolder("Assets",            "Prefabs");
    }

    private static void EnsureFolder(string parent, string name)
    {
        string full = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(full))
        {
            AssetDatabase.CreateFolder(parent, name);
            Debug.Log($"[BlockPuzzleSetup] Created folder: {full}");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GameConfigSO
    // ─────────────────────────────────────────────────────────────────────────

    private static GameConfigSO CreateOrLoadGameConfig()
    {
        string path   = ConfigPath + "/GameConfigSO.asset";
        var    config = AssetDatabase.LoadAssetAtPath<GameConfigSO>(path);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<GameConfigSO>();
            AssetDatabase.CreateAsset(config, path);
        }
        // All fields use inspector defaults — nothing needed here.
        EditorUtility.SetDirty(config);
        return config;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ScoringConfigSO
    // ─────────────────────────────────────────────────────────────────────────

    private static ScoringConfigSO CreateOrLoadScoringConfig()
    {
        string path   = ConfigPath + "/ScoringConfigSO.asset";
        var    config = AssetDatabase.LoadAssetAtPath<ScoringConfigSO>(path);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<ScoringConfigSO>();
            AssetDatabase.CreateAsset(config, path);
        }
        EditorUtility.SetDirty(config);
        return config;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 18 PieceShapeSO + PieceCatalogSO
    // ─────────────────────────────────────────────────────────────────────────

    private static PieceCatalogSO CreateOrLoadCatalogWithShapes()
    {
        var shapes = new PieceShapeSO[]
        {
            CreateShape("Dot",     new[]{V(0,0)},                                         "#FFFFFF"),
            CreateShape("DominoH", new[]{V(0,0),V(1,0)},                                 "#4FC3F7"),
            CreateShape("DominoV", new[]{V(0,0),V(0,1)},                                 "#4FC3F7"),
            CreateShape("TrioH",   new[]{V(0,0),V(1,0),V(2,0)},                          "#81C784"),
            CreateShape("TrioV",   new[]{V(0,0),V(0,1),V(0,2)},                          "#81C784"),
            CreateShape("Square2", new[]{V(0,0),V(1,0),V(0,1),V(1,1)},                   "#FFD54F"),
            CreateShape("L_TR",    new[]{V(0,0),V(0,1),V(0,2),V(1,0)},                   "#FF8A65"),
            CreateShape("L_TL",    new[]{V(0,0),V(1,0),V(1,1),V(1,2)},                   "#FF8A65"),
            CreateShape("L_BR",    new[]{V(0,2),V(1,2),V(1,1),V(1,0)},                   "#FF8A65"),
            CreateShape("L_BL",    new[]{V(0,0),V(0,1),V(0,2),V(1,2)},                   "#FF8A65"),
            CreateShape("S_H",     new[]{V(1,0),V(2,0),V(0,1),V(1,1)},                   "#CE93D8"),
            CreateShape("Z_H",     new[]{V(0,0),V(1,0),V(1,1),V(2,1)},                   "#CE93D8"),
            CreateShape("T_U",     new[]{V(0,0),V(1,0),V(2,0),V(1,1)},                   "#F06292"),
            CreateShape("T_D",     new[]{V(0,1),V(1,1),V(2,1),V(1,0)},                   "#F06292"),
            CreateShape("T_R",     new[]{V(0,0),V(0,1),V(0,2),V(1,1)},                   "#F06292"),
            CreateShape("T_L",     new[]{V(1,0),V(1,1),V(1,2),V(0,1)},                   "#F06292"),
            CreateShape("QuadH",   new[]{V(0,0),V(1,0),V(2,0),V(3,0)},                   "#4DB6AC"),
            CreateShape("Square3", Square3Cells(),                                         "#E57373"),
        };

        string catalogPath = PiecesPath + "/PieceCatalogSO.asset";
        var    catalog     = AssetDatabase.LoadAssetAtPath<PieceCatalogSO>(catalogPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<PieceCatalogSO>();
            AssetDatabase.CreateAsset(catalog, catalogPath);
        }

        var so = new SerializedObject(catalog);
        var prop = so.FindProperty("_pieces");
        prop.arraySize = shapes.Length;
        for (int i = 0; i < shapes.Length; i++)
            prop.GetArrayElementAtIndex(i).objectReferenceValue = shapes[i];
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(catalog);
        return catalog;
    }

    private static PieceShapeSO CreateShape(string pieceName, Vector2Int[] cells, string hexColor)
    {
        string path  = ShapesPath + "/" + pieceName + ".asset";
        var    shape = AssetDatabase.LoadAssetAtPath<PieceShapeSO>(path);
        if (shape == null)
        {
            shape = ScriptableObject.CreateInstance<PieceShapeSO>();
            AssetDatabase.CreateAsset(shape, path);
        }

        ColorUtility.TryParseHtmlString(hexColor, out Color color);

        var so   = new SerializedObject(shape);
        so.FindProperty("_pieceName").stringValue = pieceName;
        var cellsProp = so.FindProperty("_cells");
        cellsProp.arraySize = cells.Length;
        for (int i = 0; i < cells.Length; i++)
        {
            var el = cellsProp.GetArrayElementAtIndex(i);
            el.FindPropertyRelative("x").intValue = cells[i].x;
            el.FindPropertyRelative("y").intValue = cells[i].y;
        }
        so.FindProperty("_color").colorValue = color;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(shape);
        return shape;
    }

    private static Vector2Int V(int x, int y) => new Vector2Int(x, y);

    private static Vector2Int[] Square3Cells()
    {
        var cells = new Vector2Int[9];
        int i = 0;
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                cells[i++] = new Vector2Int(x, y);
        return cells;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GameScene creation
    // ─────────────────────────────────────────────────────────────────────────

    private static void CreateGameScene(GameConfigSO gameConfig, ScoringConfigSO scoringConfig, PieceCatalogSO catalog)
    {
        const string scenePath = "Assets/Scenes/GameScene.unity";

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── Camera ────────────────────────────────────────────────────────────
        var camGO      = new GameObject("Main Camera");
        camGO.tag      = "MainCamera";
        var cam        = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.07f, 0.07f, 0.10f, 1f);
        cam.orthographic     = true;
        cam.orthographicSize = 9f;   // runtime CameraFitter will override this
        cam.transform.position = new Vector3(0f, 0f, -10f);
        camGO.AddComponent<AudioListener>();
        var cameraFitter = camGO.AddComponent<CameraFitter>();

        // ── ScoreManager ─────────────────────────────────────────────────────
        var scoreGO     = new GameObject("ScoreManager");
        var scoreSystem = scoreGO.AddComponent<ScoreSystem>();
        AssignSerializedField(scoreSystem, "_config", scoringConfig);

        // ── GameManager ──────────────────────────────────────────────────────
        var managerGO       = new GameObject("GameManager");
        var gameStateManager = managerGO.AddComponent<GameStateManager>();

        // ── Board ─────────────────────────────────────────────────────────────
        var boardGO    = new GameObject("Board");
        boardGO.transform.position = new Vector3(0f, 1.0f, 0f);
        var gridView   = boardGO.AddComponent<GridView>();
        var boardCtrl  = boardGO.AddComponent<BoardController>();
        AssignSerializedField(gridView,  "_config",      gameConfig);
        AssignSerializedField(boardCtrl, "_config",           gameConfig);
        AssignSerializedField(boardCtrl, "_gridView",         gridView);
        AssignSerializedField(boardCtrl, "_scoreSystem",      scoreSystem);
        AssignSerializedField(boardCtrl, "_gameStateManager", gameStateManager);

        // ── Tray ──────────────────────────────────────────────────────────────
        var trayGO   = new GameObject("Tray");
        trayGO.transform.position = new Vector3(0f, 1.0f, 0f); // co-located with board so trayY calc is relative to board center
        var trayCtrl = trayGO.AddComponent<PieceTrayController>();
        AssignSerializedField(trayCtrl, "_config",            gameConfig);
        AssignSerializedField(trayCtrl, "_catalog",           catalog);
        AssignSerializedField(trayCtrl, "_boardController",   boardCtrl);
        AssignSerializedField(trayCtrl, "_gameStateManager",  gameStateManager);

        // ── DragManager ───────────────────────────────────────────────────────
        var dragGO   = new GameObject("DragManager");
        var dragCtrl = dragGO.AddComponent<PieceDragController>();
        AssignSerializedField(dragCtrl, "_boardController", boardCtrl);
        AssignSerializedField(dragCtrl, "_trayController",  trayCtrl);
        AssignSerializedField(dragCtrl, "_config",          gameConfig);

        // Wire tray → drag
        AssignSerializedField(trayCtrl, "_dragController", dragCtrl);

        // Wire CameraFitter → needs config and board transform (wired after board is created)
        AssignSerializedField(cameraFitter, "_config",         gameConfig);
        AssignSerializedField(cameraFitter, "_boardTransform", boardGO.transform);

        // ── Canvas + EventSystem ──────────────────────────────────────────────
        var eventGO = new GameObject("EventSystem");
        eventGO.AddComponent<EventSystem>();
        eventGO.AddComponent<InputSystemUIInputModule>();

        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution  = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight   = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // ── HUD Panel ─────────────────────────────────────────────────────────
        var hudGO  = BuildPanel(canvasGO, "HUD", new Color(0, 0, 0, 0));
        var hudCtrl = hudGO.AddComponent<GameplayHUDController>();
        AssignSerializedField(hudCtrl, "_gameStateManager", gameStateManager);
        AssignSerializedField(hudCtrl, "_scoreSystem",      scoreSystem);

        var scoreLabel     = BuildLabel(hudGO,  "ScoreLabel",     "SCORE\n0",     new Vector2(0f, 1f),  new Vector2(0f,  1f),  new Vector2( 120f, -60f),  new Vector2(200f, 80f));
        var bestLabel      = BuildLabel(hudGO,  "BestScoreLabel", "BEST\n0",      new Vector2(1f, 1f),  new Vector2(1f,  1f),  new Vector2(-120f, -60f),  new Vector2(200f, 80f));
        var pauseBtn       = BuildButton(hudGO, "PauseButton",    "II",           new Vector2(0.5f, 1f), new Vector2(0.5f,1f),  new Vector2(0f, -60f),     new Vector2(80f,  60f));
        AssignSerializedField(hudCtrl, "_scoreLabel",    scoreLabel);
        AssignSerializedField(hudCtrl, "_bestScoreLabel", bestLabel);
        AssignSerializedField(hudCtrl, "_pauseButton",   pauseBtn);

        // ── Main Menu Panel ───────────────────────────────────────────────────
        var menuGO  = BuildPanel(canvasGO, "MainMenu", new Color(0.05f, 0.05f, 0.08f, 0.95f));
        var menuCtrl = menuGO.AddComponent<MainMenuController>();
        AssignSerializedField(menuCtrl, "_gameStateManager", gameStateManager);
        AssignSerializedField(menuCtrl, "_scoreSystem",      scoreSystem);

        BuildLabel(menuGO,  "TitleLabel",         "BLOCK PUZZLE",      new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero, new Vector2(600f, 100f));
        var menuBestLabel = BuildLabel(menuGO, "BestScoreLabel",  "BEST: 0",           new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), Vector2.zero, new Vector2(400f, 60f));
        var startBtn      = BuildButton(menuGO, "StartButton",    "START",             new Vector2(0.5f, 0.4f),  new Vector2(0.5f, 0.4f),  Vector2.zero, new Vector2(300f, 80f));
        AssignSerializedField(menuCtrl, "_bestScoreLabel", menuBestLabel);
        AssignSerializedField(menuCtrl, "_startButton",    startBtn);

        // ── Pause Menu Panel ──────────────────────────────────────────────────
        var pauseGO  = BuildPanel(canvasGO, "PauseMenu", new Color(0f, 0f, 0f, 0.75f));
        var pauseCtrl = pauseGO.AddComponent<PauseMenuController>();
        AssignSerializedField(pauseCtrl, "_gameStateManager", gameStateManager);

        BuildLabel(pauseGO, "PausedLabel",        "PAUSED",           new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(400f, 80f));
        var resumeBtn   = BuildButton(pauseGO, "ResumeButton",   "RESUME",           new Vector2(0.5f, 0.5f),  new Vector2(0.5f, 0.5f),  Vector2.zero, new Vector2(300f, 80f));
        var pauseMenuBtn = BuildButton(pauseGO, "MainMenuButton", "MAIN MENU",        new Vector2(0.5f, 0.38f), new Vector2(0.5f, 0.38f), Vector2.zero, new Vector2(300f, 80f));
        AssignSerializedField(pauseCtrl, "_resumeButton",   resumeBtn);
        AssignSerializedField(pauseCtrl, "_mainMenuButton", pauseMenuBtn);

        // ── Game Over Panel ────────────────────────────────────────────────────
        var gameOverGO  = BuildPanel(canvasGO, "GameOver", new Color(0.05f, 0.05f, 0.08f, 0.95f));
        var gameOverCtrl = gameOverGO.AddComponent<GameOverController>();
        AssignSerializedField(gameOverCtrl, "_gameStateManager", gameStateManager);
        AssignSerializedField(gameOverCtrl, "_scoreSystem",      scoreSystem);

        BuildLabel(gameOverGO, "GameOverLabel",     "GAME OVER",        new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), Vector2.zero, new Vector2(500f, 90f));
        var goFinalLabel = BuildLabel(gameOverGO, "FinalScoreLabel", "SCORE\n0",       new Vector2(0.5f, 0.58f), new Vector2(0.5f, 0.58f), Vector2.zero, new Vector2(300f, 80f));
        var goBestLabel  = BuildLabel(gameOverGO, "BestScoreLabel",  "BEST\n0",        new Vector2(0.5f, 0.46f), new Vector2(0.5f, 0.46f), Vector2.zero, new Vector2(300f, 80f));
        var restartBtn   = BuildButton(gameOverGO,"RestartButton",   "RESTART",        new Vector2(0.5f, 0.34f), new Vector2(0.5f, 0.34f), Vector2.zero, new Vector2(300f, 80f));
        var goMenuBtn    = BuildButton(gameOverGO,"MainMenuButton",  "MAIN MENU",      new Vector2(0.5f, 0.22f), new Vector2(0.5f, 0.22f), Vector2.zero, new Vector2(300f, 80f));
        AssignSerializedField(gameOverCtrl, "_finalScoreLabel", goFinalLabel);
        AssignSerializedField(gameOverCtrl, "_bestScoreLabel",  goBestLabel);
        AssignSerializedField(gameOverCtrl, "_restartButton",   restartBtn);
        AssignSerializedField(gameOverCtrl, "_mainMenuButton",  goMenuBtn);

        // ── Save scene ────────────────────────────────────────────────────────
        EditorSceneManager.SaveScene(scene, scenePath);        AddSceneToBuildSettings(scenePath);        Debug.Log("[BlockPuzzleSetup] ✅ GameScene saved to " + scenePath);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UI helper builders
    // ─────────────────────────────────────────────────────────────────────────

    private static GameObject BuildPanel(GameObject parent, string name, Color bgColor)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt   = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img  = go.AddComponent<Image>();
        img.color = bgColor;
        go.AddComponent<CanvasGroup>();
        return go;
    }

    private static TextMeshProUGUI BuildLabel(
        GameObject parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin    = anchorMin;
        rt.anchorMax    = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta    = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = 36f;
        tmp.color     = Color.white;
        return tmp;
    }

    private static Button BuildButton(
        GameObject parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = size;
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 1f, 1f);
        var btn = go.AddComponent<Button>();

        // Label child
        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var lrt = labelGO.AddComponent<RectTransform>();
        lrt.anchorMin    = Vector2.zero;
        lrt.anchorMax    = Vector2.one;
        lrt.offsetMin    = Vector2.zero;
        lrt.offsetMax    = Vector2.zero;
        var tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = 30f;
        tmp.color     = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        return btn;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Build Settings
    // ─────────────────────────────────────────────────────────────────────────

    private static void AddSceneToBuildSettings(string scenePath)
    {
        var existing = EditorBuildSettings.scenes;
        foreach (var s in existing)
            if (s.path == scenePath) return; // already added

        var updated = new EditorBuildSettingsScene[existing.Length + 1];
        for (int i = 0; i < existing.Length; i++)
            updated[i] = existing[i];
        updated[existing.Length] = new EditorBuildSettingsScene(scenePath, true);
        EditorBuildSettings.scenes = updated;
        Debug.Log("[BlockPuzzleSetup] GameScene added to Build Settings (index " + existing.Length + ").");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SerializedObject field assignment helper
    // ─────────────────────────────────────────────────────────────────────────

    private static void AssignSerializedField(Object target, string fieldName, Object value)
    {
        var so   = new SerializedObject(target);
        var prop = so.FindProperty(fieldName);
        if (prop != null)
        {
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        else
        {
            Debug.LogWarning($"[BlockPuzzleSetup] Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }
}
