---
title: Block Puzzle — Design Pillars
author: Game Designer
date: 2026-04-06
status: Approved
version: 1.0
---

# Block Puzzle — Design Pillars

These pillars are the creative foundation. Every design decision must trace back to at least one pillar.

---

## Pillar 1: Easy to Learn, Hard to Master

**Statement**: The rules are explainable in 10 seconds. Strategic depth takes hours to explore.

- New players understand the goal immediately: fit pieces on a grid, clear lines, survive
- Advanced players find depth in combo setups, space management, and piece sequencing
- No tutorial required — the game teaches itself through play
- Difficulty emerges from the board state, not from rules complexity

**Design implications**:
- UI must be non-intimidating with zero jargon
- Fail states are instantly readable ("no space for this piece")
- Combo system rewards planning without punishing casual play

---

## Pillar 2: Satisfying Feedback

**Statement**: Every action must feel deliberate and rewarding.

- Placing a piece has visual confirmation (snap, highlight)
- Clearing a line triggers a distinct visual reward
- Combos feel progressively exciting, not just numerically bigger
- Invalid placements give clear, immediate negative feedback

**Design implications**:
- Use animation (flash, scale, color shift) on every meaningful event
- Preview ghost shows exactly where a piece will land
- Score pop-up shows points earned per action
- No silent failures — every invalid action has visible feedback

---

## Pillar 3: Clean & Readable

**Statement**: The player always knows their exact situation at a glance.

- Board state is readable in under 1 second
- Available pieces are clearly distinct from one another
- Score and best-score are always visible, never intrusive
- Grid lines and cell states use high-contrast, unambiguous colors

**Design implications**:
- Minimal UI chrome — the board is the primary visual
- Piece colors must differ sufficiently for color-blind accessibility
- No screen clutter — only score, board, and piece tray on the main screen
- Game over state is immediate and obvious
