---
title: Piece System
author: Game Designer
date: 2026-04-06
status: Approved
version: 1.0
---

# Piece System Design Spec

**Pillar references**: Pillar 1 (Easy to learn), Pillar 2 (Satisfying feedback)

---

## Overview

Pieces are the player's primary agency. Each turn, the player receives 3 pieces and must place them all. Piece shapes vary in size and complexity, creating strategic variety. The piece tray limits options intentionally, forcing players to plan ahead.

---

## Design Goals

1. Pieces are immediately distinguishable by shape and color
2. A 3-piece tray creates short-term planning without overwhelming decision space
3. Piece variety maintains long-term interest without requiring tutorials
4. Drag-and-drop feels precise and satisfying — ghost preview shows outcome clearly

---

## Piece Catalog (18 shapes)

All pieces use offsets from their origin `(0, 0)` (bottom-left of bounding box).

| ID | Name | Cells | Size |
|----|------|-------|------|
| 1 | Dot | `(0,0)` | 1×1 |
| 2 | DominoH | `(0,0),(1,0)` | 1×2 |
| 3 | DominoV | `(0,0),(0,1)` | 2×1 |
| 4 | TrioH | `(0,0),(1,0),(2,0)` | 1×3 |
| 5 | TrioV | `(0,0),(0,1),(0,2)` | 3×1 |
| 6 | Square2 | `(0,0),(1,0),(0,1),(1,1)` | 2×2 |
| 7 | L_TR | `(0,0),(0,1),(0,2),(1,0)` | L-shape |
| 8 | L_TL | `(0,0),(1,0),(1,1),(1,2)` | J-shape |
| 9 | L_BR | `(0,2),(1,2),(1,1),(1,0)` | L mirrored |
| 10 | L_BL | `(0,0),(0,1),(0,2),(1,2)` | J mirrored |
| 11 | S_H | `(1,0),(2,0),(0,1),(1,1)` | S-shape |
| 12 | Z_H | `(0,0),(1,0),(1,1),(2,1)` | Z-shape |
| 13 | T_U | `(0,0),(1,0),(2,0),(1,1)` | T-shape |
| 14 | T_D | `(0,1),(1,1),(2,1),(1,0)` | T inverted |
| 15 | T_R | `(0,0),(0,1),(0,2),(1,1)` | T rotated right |
| 16 | T_L | `(1,0),(1,1),(1,2),(0,1)` | T rotated left |
| 17 | QuadH | `(0,0),(1,0),(2,0),(3,0)` | 1×4 |
| 18 | Square3 | `(0,0),(1,0),(2,0),(0,1),(1,1),(2,1),(0,2),(1,2),(2,2)` | 3×3 |

### Piece Colors (MVP — high contrast palette)

| ID | Shape | Color |
|----|-------|-------|
| 1 | Dot | `#FFFFFF` (white) |
| 2–3 | Dominoes | `#4FC3F7` (light blue) |
| 4–5 | Trios | `#81C784` (green) |
| 6 | Square2 | `#FFD54F` (amber) |
| 7–10 | L/J shapes | `#FF8A65` (orange) |
| 11–12 | S/Z shapes | `#CE93D8` (purple) |
| 13–16 | T shapes | `#F06292` (pink) |
| 17 | QuadH | `#4DB6AC` (teal) |
| 18 | Square3 | `#E57373` (red) |

---

## Piece Tray Rules

- Always shows exactly **3 pieces** at once (configurable via `GameConfigSO.PieceTraySize`)
- Pieces are selected **uniformly at random** from the catalog (all 18 pieces equally likely)
- New tray is generated **only when all 3 pieces have been placed** — never mid-turn
- Players may place the 3 pieces in **any order**
- If a piece cannot currently be placed (no valid position), it is **greyed out** but not removed

---

## Drag & Drop

### Interaction Flow
1. Player presses mouse button on a tray piece
2. Piece lifts to follow cursor (sorting order raised, scale jumps to grid cell size)
3. Ghost preview renders on grid below cursor position
4. Ghost color: **green** (valid placement) or **red** (invalid)
5. Player releases mouse:
   - Over valid grid position → piece snaps to grid, tray slot empties
   - Anywhere else → piece returns to tray slot (snap animation)

### Snapping
- The piece's bounding box center aligns to the cursor's grid cell
- `GetOriginFromCursorGridPos(cursorGridPos)` computes correct placement origin

---

## Edge Cases

| Scenario | Expected Behavior |
|----------|-------------------|
| Player drags piece off-screen | Release triggers invalid placement → return to tray |
| Player releases piece over another tray piece | Invalid → return to tray |
| One piece cannot fit anywhere | Grey out that piece; game continues if others still fit |
| All 3 pieces cannot fit | Game Over after current tray exhausted |
| Tray has only 1 or 2 pieces (mid-tray) | Only remaining pieces checked for Game Over |

---

## Success Metrics (Playtest)

- Players feel the drag response is tight and precise (no drift, no missed inputs)
- Ghost preview is read correctly as valid/invalid on first exposure
- Piece shape variety keeps the first 5 games feeling different
