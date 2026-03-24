# Data Model: Split Project into Console and Library

**Branch**: `001-split-console-library` | **Date**: 2026-03-23

---

This feature introduces no new data entities or state changes to the simulation model. It is a pure structural reorganisation of existing source files into two projects.

The existing entities and their ownership after the split are documented below for reference.

---

## Entities (unchanged)

### Cell

- **Project**: `Game-Of-Life.Library`
- **Namespace**: `GameOfLife.Library`
- **Represents**: A single cell in the grid. Encapsulates alive/dead state and neighbour-counting logic.
- **No change** to fields, behaviour, or tests.

### Life

- **Project**: `Game-Of-Life.Library`
- **Namespace**: `GameOfLife.Library`
- **Represents**: The simulation engine. Owns the grid (`int[,]`), applies the Game of Life rules each tick, and delegates rendering.
- **No change** to fields, behaviour, or tests.

### Helper

- **Project**: `Game-Of-Life.Library`
- **Namespace**: `GameOfLife.Library`
- **Represents**: Static utility class. Converts pattern strings to `int[,]` matrices.
- **No change** to fields, behaviour, or tests.

### Patterns

- **Project**: `Game-Of-Life.Library`
- **Namespace**: `GameOfLife.Library`
- **Represents**: Static class of named pattern constants (digit-string format).
- **No change** to fields, behaviour, or tests.

### Program

- **Project**: `Game-Of-Life.Console`
- **Namespace**: `GameOfLife`
- **Represents**: Application entry-point. Defines simulation settings constants and runs the loop.
- **Change**: Gains `using GameOfLife.Library;` to reference library types. `[ExcludeFromCodeCoverage]` attribute removed (console project is outside coverage scope). No logic change.

---

## State transitions

No state transitions change. The simulation tick cycle, pattern loading, and rendering pipeline are unchanged — they are simply relocated to the library project.

---

## Summary

| Entity | Before | After |
|--------|--------|-------|
| Cell | `GameOfLife` in single `.csproj` | `GameOfLife.Library` in `Game-Of-Life.Library.csproj` |
| Life | `GameOfLife` in single `.csproj` | `GameOfLife.Library` in `Game-Of-Life.Library.csproj` |
| Helper | `GameOfLife` in single `.csproj` | `GameOfLife.Library` in `Game-Of-Life.Library.csproj` |
| Patterns | `GameOfLife` in single `.csproj` | `GameOfLife.Library` in `Game-Of-Life.Library.csproj` |
| Program | `GameOfLife` in single `.csproj` | `GameOfLife` in `Game-Of-Life.Console.csproj` |
| All tests | `GameOfLife` in single `.csproj` | `GameOfLife.Library` in `Game-Of-Life.Library.csproj` |
