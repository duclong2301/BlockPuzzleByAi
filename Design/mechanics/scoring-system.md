---
title: Scoring System
author: Game Designer
date: 2026-04-06
status: Approved
version: 1.0
---

# Scoring System Design Spec

**Pillar references**: Pillar 2 (Satisfying feedback), Pillar 1 (Easy to learn)

---

## Overview

The scoring system rewards players for placing pieces efficiently and for clearing lines, especially in quick succession. The system must be legible (players understand why they scored) while still providing depth for skilled play through combos.

---

## Design Goals

1. Score increases with every action — the game always feels progressive
2. Line clears give a noticeably larger reward than cell placements
3. Combos (multiple lines in one go) give a bonus that rewards planning
4. Score is simple enough to explain in one sentence

---

## Scoring Rules

### Per-Cell Placement
- **+1 point** per cell occupied when a piece is placed
- A 3×3 piece (9 cells) gives +9 points just from placement

### Line Clear Bonus
- **+10 points** per line (row or column) cleared in a single placement
- Clearing 1 row = +10
- Clearing 2 rows = +20
- Clearing 1 row + 1 column = +20

### Combo Bonus
- A **combo** is active when a placement clears ≥1 line
- Combo counter increments each time a placement triggers a line clear
- Combo counter resets when a placement clears **zero** lines
- **Combo bonus = ComboBaseBonus × (comboCount - 1)**
- First clear: no combo bonus (`comboCount = 1`)
- Second consecutive clear: `+20 × 1 = +20` bonus
- Third consecutive clear: `+20 × 2 = +40` bonus

### Score Formula (per placement)

```
score = (cells_placed × PointsPerCell)
      + (lines_cleared × PointsPerLineClear)
      + (comboMultiplier)

where comboMultiplier = ComboBaseBonus × max(0, comboCount - 1)
```

---

## Data Spec

Configured in `ScoringConfigSO` (ScriptableObject at `Assets/Data/Config/ScoringConfigSO.asset`):

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `PointsPerCell` | int | 1 | Points awarded per cell placed |
| `PointsPerLineClear` | int | 10 | Points awarded per line cleared |
| `ComboBaseBonus` | int | 20 | Multiplier base for combo streaks |

---

## Example Score Scenarios

| Action | Cells | Lines | Combo | Score |
|--------|-------|-------|-------|-------|
| Place 1×1 dot | 1 | 0 | 0 | 1 |
| Place 1×3 strip | 3 | 0 | 0 | 3 |
| Place piece, clear 1 row | 3 | 1 | Start | 3+10 = 13 |
| Next piece, clear 2 rows | 4 | 2 | ×1 | 4+20+20 = 44 |
| Next piece, clear 1 row, 1 col | 9 | 2 | ×2 | 9+20+40 = 69 |
| Next piece, no clear | 2 | 0 | Reset | 2 |

---

## Best Score (Persistence)

- Best score is stored via `SaveSystem` (wrapping `PlayerPrefs`)
- Updated in real-time if current score exceeds best score during a game
- `ScoreSystem` fires `OnBestScoreChanged` event when best score is surpassed
- Displayed on: Main Menu screen and Game Over screen

---

## Edge Cases

| Scenario | Expected Behavior |
|----------|-------------------|
| Score overflows int | Practically impossible (max ~2 billion) but no special handling needed |
| Combo resets to 0 | Next clear starts a fresh combo streak from comboCount=1 |
| Multiple lines cleared at once | All count toward `lines_cleared` in the single formula application |
| Player places piece with 0 valid lines | Combo resets; only cell points awarded |

---

## Success Metrics (Playtest)

- Players notice and comment on the combo streak (it feels meaningful, not invisible)
- Players understand score increases after every placement without explanation
- Score numbers are not so inflated that they feel meaningless
