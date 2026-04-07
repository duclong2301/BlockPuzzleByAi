---
title: Block Puzzle — Game Design Document
author: Game Designer
date: 2026-04-06
status: Draft
version: 1.0
---

# Block Puzzle — Game Design Document

## Overview

**Genre**: Puzzle / Casual  
**Platform**: PC (Windows, primary)  
**Engine**: Unity (URP, basic sprites)  
**Scope**: Single-player, no audio in MVP  
**Target Session Length**: 5–20 minutes

Block Puzzle is a tile-placement puzzle game in the style of 1010!. Players are given a sequence of polyomino pieces and must place them onto a 10×10 grid. When a full row or column is formed, it is cleared and the player earns points. The game ends when no remaining piece can fit on the board.

---

## Design Pillars

See [Design/pillars.md](./pillars.md).

---

## Core Mechanics

### The Grid
- 10 × 10 cell grid
- Cells are either **empty** or **occupied** (with a color from the piece that filled them)
- The grid never shifts — placed cells stay until a line is cleared
- See full spec: [mechanics/grid-system.md](./mechanics/grid-system.md)

### Piece Tray
- At any time, **3 pieces** are displayed in a tray below the grid
- Player places pieces in any order
- When all 3 are placed, a new set of 3 is generated immediately
- A new set is never generated until all current pieces are placed
- Pieces are randomly selected from the piece catalog

### Piece Types
- 18 polyomino shapes (1×1, 1×2, 1×3, 2×2, L-shapes, T-shapes, S/Z-shapes, 3×3)
- Each piece has a fixed color per shape type
- See full spec: [mechanics/piece-system.md](./mechanics/piece-system.md)

### Placement Rules
- A piece can be placed on any valid grid position where all its cells are empty and within bounds
- Placement is done by **drag and drop** with a ghost preview showing the landing position
- If placement is invalid, the piece returns to its tray slot
- **One piece can be dragged at a time**

### Line Clearing
- When a **row** is entirely filled, it is cleared
- When a **column** is entirely filled, it is cleared
- Both rows and columns are checked simultaneously after every placement
- Multiple lines can be cleared in a single placement (combo)
- Cleared cells become empty, available for future placement

### Game Over
- After any piece placement, the game checks all remaining tray pieces
- If **none** of the remaining pieces can be placed anywhere on the board → **Game Over**
- The check also runs after generating a new tray of 3 pieces

### Scoring
- See full spec: [mechanics/scoring-system.md](./mechanics/scoring-system.md)
- Points are awarded per cell placed and per line cleared
- Combo bonus for clearing multiple lines in one placement or consecutive placements with clears

---

## Target Experience

| Moment | Expected Player Feeling |
|--------|------------------------|
| Placing a piece perfectly | "That fit perfectly — satisfying snap" |
| Clearing a combo of 2+ lines | "Yes! I planned that — exciting reward" |
| Running low on space | "Tense, strategic, I need to clear a line" |
| Game Over | "One more game — I know I can do better" |
| New high score | "Achievement — genuine pride" |

---

## Game States

```
MainMenu
  ↓ Start Game
Playing
  ↓ Pause         ↑ Resume
Paused
Playing
  ↓ No valid moves
GameOver
  ↓ Restart → Playing
  ↓ Main Menu → MainMenu
```

---

## Progression & Persistence

- **Current Score**: displayed live during play
- **Best Score (High Score)**: persisted via `PlayerPrefs`, shown on main menu and game over screen
- No level system in MVP — endless survival mode only

---

## Visual Design (Basic Graphics MVP)

- All visuals are **Unity sprites** (procedurally generated white squares, colored via SpriteRenderer)
- Grid background: very dark cells with small gap between cells for readability
- Occupied cells: colored by piece color (distinct per piece type)
- Ghost preview:
  - Valid placement: semi-transparent **green**
  - Invalid placement: semi-transparent **red**
- UI: TextMeshPro text on Camera overlay canvas
- No custom textures, models, or art assets required for MVP

---

## Scope Definition

### MVP (Ship First)
- [x] 10×10 grid with row/column clearing
- [x] 18 piece shapes in catalog
- [x] Drag-and-drop placement with ghost preview
- [x] 3-piece tray with auto-refresh
- [x] Score system with combo bonus
- [x] High score persistence
- [x] Game Over detection and screen
- [x] Main Menu, Pause Menu
- [x] Basic sprite visuals (no art assets needed)

### Post-MVP (Future)
- [ ] Audio (SFX + music)
- [ ] Line-clear animation (flash/fade)
- [ ] Score pop-up text animation
- [ ] Accessibility: colorblind piece shapes
- [ ] Multiple themes / color palettes
- [ ] Daily challenge mode
- [ ] Leaderboard / cloud save

---

## Open Questions

| # | Question | Priority | Owner |
|---|----------|----------|-------|
| 1 | Should pieces have fixed colors or randomized colors per tray? | High | Game Designer |
| 2 | Is a 5-second "game over" delay good for showing final score? | Medium | UX Designer |
| 3 | Should combo reset after placing a piece without clearing? | High | Game Designer |
