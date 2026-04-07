---
title: Block Puzzle — Master Plan & Session Progress
author: Technical Director
date: 2026-04-06
status: In Progress
version: 1.0
---

# Block Puzzle — Master Implementation Plan

## Tổng quan dự án

**Game**: Block Puzzle (1010!-style)  
**Engine**: Unity (URP, basic sprites, không cần art asset)  
**Language**: C#  
**Phạm vi MVP**: Grid 10×10 · Drag & drop · 18 piece shapes · Line clear · Combo scoring · High score · Game state FSM · UI đầy đủ  
**Không có trong MVP**: Audio, custom art, particle effects phức tạp  

---

## Tiến độ tổng thể

```
Phase 1 — Foundation          ████████████████████  100%  ✅ DONE
Phase 2 — Core Gameplay       ████████████████████  100%  ✅ DONE
Phase 3 — Game Systems        ████████████████████  100%  ✅ DONE
Phase 4 — UI Controllers      ░░░░░░░░░░░░░░░░░░░░    0%  ⏳ NEXT
Phase 5 — Unity Editor Setup  ░░░░░░░░░░░░░░░░░░░░    0%  ⏳ TODO
Phase 6 — Polish              ░░░░░░░░░░░░░░░░░░░░    0%  ⏳ TODO
```

---

## Phase 1 — Foundation ✅ HOÀN THÀNH

### Design Documents

| File | Trạng thái |
|------|-----------|
| `Design/pillars.md` | ✅ Done |
| `Design/GDD.md` | ✅ Done |
| `Design/mechanics/grid-system.md` | ✅ Done |
| `Design/mechanics/piece-system.md` | ✅ Done |
| `Design/mechanics/scoring-system.md` | ✅ Done |

### ScriptableObject Definitions (C# chỉ — chưa tạo .asset)

| File | Trạng thái |
|------|-----------|
| `Assets/Scripts/Data/GameConfigSO.cs` | ✅ Done |
| `Assets/Scripts/Data/ScoringConfigSO.cs` | ✅ Done |
| `Assets/Scripts/Data/Pieces/PieceShapeSO.cs` | ✅ Done |
| `Assets/Scripts/Data/Pieces/PieceCatalogSO.cs` | ✅ Done |

### Interfaces & Utilities

| File | Trạng thái |
|------|-----------|
| `Assets/Scripts/Gameplay/Interfaces/IPlaceable.cs` | ✅ Done |
| `Assets/Scripts/Gameplay/Interfaces/IClearable.cs` | ✅ Done |
| `Assets/Scripts/Gameplay/PrimitiveSprite.cs` | ✅ Done |

---

## Phase 2 — Core Gameplay ✅ HOÀN THÀNH

### Grid System

| File | Trạng thái | Ghi chú |
|------|-----------|---------|
| `Assets/Scripts/Gameplay/Grid/GridModel.cs` | ✅ Done | Pure C#, no MonoBehaviour |
| `Assets/Scripts/Gameplay/Grid/CellView.cs` | ✅ Done | SpriteRenderer + IClearable |
| `Assets/Scripts/Gameplay/Grid/GridView.cs` | ✅ Done | Builds 100 cells, world↔grid |
| `Assets/Scripts/Gameplay/Grid/BoardController.cs` | ✅ Done | Orchestrator |

### Piece System

| File | Trạng thái | Ghi chú |
|------|-----------|---------|
| `Assets/Scripts/Gameplay/Pieces/PieceData.cs` | ✅ Done | Immutable + BoundsCenter |
| `Assets/Scripts/Gameplay/Pieces/PieceGenerator.cs` | ✅ Done | Random từ catalog |
| `Assets/Scripts/Gameplay/Pieces/PieceView.cs` | ✅ Done | OnMouseDown → event |
| `Assets/Scripts/Gameplay/Pieces/PieceTrayController.cs` | ✅ Done | 3-slot, auto-refresh |
| `Assets/Scripts/Gameplay/Pieces/PieceDragController.cs` | ✅ Done | Update drag + preview |

---

## Phase 3 — Game Systems ✅ HOÀN THÀNH

| File | Trạng thái | Ghi chú |
|------|-----------|---------|
| `Assets/Scripts/Core/SaveSystem.cs` | ✅ Done | PlayerPrefs wrapper |
| `Assets/Scripts/Core/ScoreSystem.cs` | ✅ Done | Combo logic, auto best-save |
| `Assets/Scripts/Core/GameStateManager.cs` | ✅ Done | FSM 4 states |

---

## Phase 4 — UI Controllers ⏳ TIẾP THEO

Tạo 4 UI Controller classes + UXML/USS hoặc UGUI Canvas:

| File | Trạng thái | Mô tả |
|------|-----------|-------|
| `Assets/Scripts/UI/GameplayHUDController.cs` | ❌ TODO | Score + Best Score live display |
| `Assets/Scripts/UI/MainMenuController.cs` | ❌ TODO | Start button, best score display |
| `Assets/Scripts/UI/PauseMenuController.cs` | ❌ TODO | Resume / Main Menu buttons |
| `Assets/Scripts/UI/GameOverController.cs` | ❌ TODO | Final score, best score, Restart / Main Menu |

### Chi tiết Phase 4

**GameplayHUDController**
- Bind `ScoreSystem.OnScoreChanged` → update hiện score label
- Bind `ScoreSystem.OnBestScoreChanged` → update best score label
- Bind `GameStateManager.OnStateChanged` → ẩn/hiện HUD
- Pause button → `GameStateManager.PauseGame()`

**MainMenuController**
- Hiển thị `SaveSystem.LoadBestScore()` khi mở
- Start button → `GameStateManager.StartGame()`
- Active khi state == `MainMenu`

**PauseMenuController**
- Resume button → `GameStateManager.ResumeGame()`
- Main Menu button → `GameStateManager.GoToMainMenu()`
- Active khi state == `Paused`

**GameOverController**
- Hiển thị current score và best score
- Restart button → `GameStateManager.StartGame()` + `ScoreSystem.ResetScore()`
- Main Menu button → `GameStateManager.GoToMainMenu()`
- Active khi state == `GameOver`

---

## Phase 5 — Unity Editor Setup ⏳ TODO

Cần làm **thủ công trong Unity Editor**:

### 5.1 Tạo ScriptableObject Assets

```
Assets/Data/Config/
├── GameConfigSO.asset     ← Create > BlockPuzzle > Config > GameConfig
└── ScoringConfigSO.asset  ← Create > BlockPuzzle > Config > ScoringConfig

Assets/Data/Pieces/
├── PieceCatalogSO.asset   ← Create > BlockPuzzle > Pieces > PieceCatalog
└── Shapes/
    ├── Dot.asset
    ├── DominoH.asset
    ├── DominoV.asset
    ├── TrioH.asset
    ├── TrioV.asset
    ├── Square2.asset
    ├── L_TR.asset
    ├── L_TL.asset
    ├── L_BR.asset
    ├── L_BL.asset
    ├── S_H.asset
    ├── Z_H.asset
    ├── T_U.asset
    ├── T_D.asset
    ├── T_R.asset
    ├── T_L.asset
    ├── QuadH.asset
    └── Square3.asset
```

### 5.2 Điền dữ liệu 18 Pieces (tham chiếu Design/mechanics/piece-system.md)

| Asset | Cells (offsets) | Color |
|-------|-----------------|-------|
| Dot | `(0,0)` | #FFFFFF |
| DominoH | `(0,0),(1,0)` | #4FC3F7 |
| DominoV | `(0,0),(0,1)` | #4FC3F7 |
| TrioH | `(0,0),(1,0),(2,0)` | #81C784 |
| TrioV | `(0,0),(0,1),(0,2)` | #81C784 |
| Square2 | `(0,0),(1,0),(0,1),(1,1)` | #FFD54F |
| L_TR | `(0,0),(0,1),(0,2),(1,0)` | #FF8A65 |
| L_TL | `(0,0),(1,0),(1,1),(1,2)` | #FF8A65 |
| L_BR | `(0,2),(1,2),(1,1),(1,0)` | #FF8A65 |
| L_BL | `(0,0),(0,1),(0,2),(1,2)` | #FF8A65 |
| S_H | `(1,0),(2,0),(0,1),(1,1)` | #CE93D8 |
| Z_H | `(0,0),(1,0),(1,1),(2,1)` | #CE93D8 |
| T_U | `(0,0),(1,0),(2,0),(1,1)` | #F06292 |
| T_D | `(0,1),(1,1),(2,1),(1,0)` | #F06292 |
| T_R | `(0,0),(0,1),(0,2),(1,1)` | #F06292 |
| T_L | `(1,0),(1,1),(1,2),(0,1)` | #F06292 |
| QuadH | `(0,0),(1,0),(2,0),(3,0)` | #4DB6AC |
| Square3 | `(0,0)...(2,2)` all 9 cells | #E57373 |

### 5.3 Tạo Scene `Scenes/GameScene.unity`

Scene hierarchy:
```
Main Camera          ← Orthographic, Size=7, Position=(0,0,-10)
GameManager          ← GameStateManager
  ├── [Component] GameStateManager

Board                ← Position=(0,0.5,0)
  ├── [Component] GridView    (assign GameConfigSO)
  └── [Component] BoardController (assign GameConfigSO, GridView ref, ScoreSystem ref)

ScoreManager         ← Position=(0,0,0)
  └── [Component] ScoreSystem (assign ScoringConfigSO)

Tray                 ← Position=(0,0,0)
  └── [Component] PieceTrayController (assign GameConfigSO, PieceCatalogSO, refs)

DragManager          ← Position=(0,0,0)
  └── [Component] PieceDragController (assign BoardController, PieceTrayController, GameConfigSO)

Canvas               ← Screen Space - Overlay
  ├── HUD            ← GameplayHUDController
  ├── MainMenu       ← MainMenuController
  ├── PauseMenu      ← PauseMenuController (default inactive)
  └── GameOver       ← GameOverController (default inactive)
```

---

## Phase 6 — Polish ⏳ TODO

| Tính năng | Mô tả | Ưu tiên |
|-----------|-------|---------|
| Line-clear animation | Flash + scale-down trên CellView bằng Coroutine | Medium |
| Piece snap animation | `iTween` hoặc `LeanTween` lerp về tray khi invalid | Low |
| Score pop-up text | TextMeshPro floating "+15" khi đặt piece | Medium |
| Greyed-out piece | Khi piece không fit đâu, SetColor alpha giảm | High |
| Combo indicator | Label "COMBO x3!" trên HUD khi combo > 1 | Medium |
| Screen shake | Cinemachine Impulse khi clear nhiều line | Low |
| Pause time scale | `Time.timeScale = 0` khi Paused | High |

---

## Dependency Graph

```
ScriptableObjects
        │
        ▼
   SaveSystem ──────────────────────┐
        │                           │
        ▼                           ▼
  ScoreSystem ◄──── BoardController ◄──── PieceDragController
        │                ▲                        ▲
        │           GridModel                     │
        │           GridView                      │
        │           CellView             PieceTrayController
        │                                        ▲
        │                               PieceView / PieceData
        │                               PieceGenerator
        ▼
  GameStateManager ──► UI Controllers (read-only via events)
```

---

## Quy tắc quan trọng (không được vi phạm)

1. **UI không bao giờ set game state trực tiếp** — chỉ gọi methods trên `GameStateManager`
2. **UI không đọc score từ ScoreSystem.CurrentScore polling** — chỉ bind qua `OnScoreChanged` event
3. **Không hardcode bất kỳ số nào** — mọi giá trị qua ScriptableObject
4. **Không `GetComponent<T>()` trong Update()** — cache trong Awake()
5. **GridModel là pure C#** — không thêm MonoBehaviour hoặc Unity-specific dependencies

---

## Hướng dẫn cho session tiếp theo

### Session tiếp theo bắt đầu ở đây:

> **"Tiếp tục Phase 4 — viết 4 UI Controllers"**

Thứ tự implement Phase 4:
1. `GameplayHUDController.cs` (quan trọng nhất, thấy kết quả ngay)
2. `GameStateManager` phải được inject vào mọi UI controller
3. `GameOverController.cs` — cần gọi `ScoreSystem.ResetScore()` khi restart
4. `MainMenuController.cs`
5. `PauseMenuController.cs`

Sau Phase 4, chuyển sang Phase 5 (Editor setup) để game có thể chạy thực sự.

### Lệnh kiểm tra lỗi compile

Trong VS Code, dùng: Get Errors trên thư mục `Assets/Scripts`

### Files cần đọc trước khi bắt đầu session mới

1. File này (`docs/MASTER_PLAN.md`) — xem tiến độ
2. `Design/GDD.md` — nhắc lại yêu cầu
3. `Assets/Scripts/Core/GameStateManager.cs` — hiểu FSM transitions
4. `Assets/Scripts/Core/ScoreSystem.cs` — hiểu events để bind UI

---

*Cập nhật lần cuối: 2026-04-06 — Session 1 kết thúc sau Phase 3*
