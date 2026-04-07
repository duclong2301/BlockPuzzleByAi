---
title: Grid System
author: Game Designer
date: 2026-04-06
status: Approved
version: 1.0
---

# Grid System Design Spec

**Pillar references**: Pillar 1 (Easy to learn), Pillar 3 (Clean & readable)

---

## Overview

The grid is the primary play space. It is a fixed 10×10 cell board where players place polyomino pieces. The grid enables the core loop by providing constrained placement space, triggering line clears, and driving the game-ending condition.

---

## Design Goals

1. Player can read the board state (occupied vs. empty) at a glance
2. Row and column clearing feels fair and predictable — no hidden rules
3. Valid / invalid placement is communicated before the player commits
4. Piece snapping is precise and never frustrating

---

## Mechanics

### Grid Dimensions
- **Width**: 10 cells  
- **Height**: 10 cells  
- **Total cells**: 100  
- **Cell size** (world units): 1.0 (configurable via `GameConfigSO`)

### Cell States
| State | Description | Visual |
|-------|-------------|--------|
| Empty | No piece occupies this cell | Dark background |
| Occupied | A piece's cell fills this slot | Piece color |
| Ghost-Valid | Drag preview — piece can be placed here | Semi-transparent green |
| Ghost-Invalid | Drag preview — piece cannot be placed here | Semi-transparent red |

### Placement
1. Player drags a piece from the tray over the grid
2. The grid computes the **placement origin** = grid position under cursor offset by piece center
3. `GridModel.CanPlace(cells, origin)` checks all cells:
   - Each cell must be within `[0, width)` × `[0, height)`
   - Each cell must be currently empty
4. Result feeds the ghost preview color (green/red)
5. On mouse release:
   - If valid → `GridModel.PlacePiece()` is called, cells become occupied
   - If invalid → piece returns to tray

### Line Clearing
After every valid placement:
1. Check all 10 rows: a row is **full** if every cell in it is occupied
2. Check all 10 columns: a column is **full** if every cell in it is occupied
3. All identified full rows and columns are cleared simultaneously
4. Clearing order: rows first, then columns (visual only — gameplay result is identical either way)
5. Cells in cleared rows/columns become empty

**Why simultaneous?** Prevents asymmetric outcomes where clearing a row first would make a column check inaccurate.

### Game Over Condition
After a placement (or after a new tray is generated), check:
- For each remaining tray piece: does `GridModel.HasAnyValidPlacement(piece.cells)` return true?
- If **none** return true → trigger Game Over
- If at least one returns true → game continues

---

## Data Spec

Configured in `GameConfigSO` (ScriptableObject at `Assets/Data/Config/GameConfigSO.asset`):

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `GridWidth` | int | 10 | Grid column count |
| `GridHeight` | int | 10 | Grid row count |
| `CellSize` | float | 1.0 | World-unit size of one cell |
| `PieceTraySize` | int | 3 | Number of pieces shown at once |
| `TrayPieceScale` | float | 0.7 | Display scale of pieces in tray |

---

## Edge Cases

| Scenario | Expected Behavior |
|----------|-------------------|
| Piece partially off-grid | Invalid — all cells must be in bounds |
| Piece placed on occupied cell | Invalid — all cells must be empty |
| Row AND column both full from same placement | Both clear simultaneously |
| All 100 cells occupied (impossible in practice) | All cleared cells from simultaneous clear |
| Grid completely empty | No change — new piece placed normally |
| Piece is 1×1 | Valid placement on any single empty cell |

---

## Success Metrics (Playtest)

- Players understand grid state without explanation within the first 60 seconds
- Ghost preview correctly predicts outcome >99% of the time (no misleading edge cases)
- No placement feels "unfair" or "stuck" without explanation
