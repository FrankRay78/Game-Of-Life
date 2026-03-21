# Game of Life

Conway's Game of Life implemented as a C# console application with a retro 80s aesthetic.

## Tech Stack

- **Language:** C# targeting .NET 10
- **Test framework:** MSTest v4.x
- **Build/test:** `dotnet` CLI

See `README.md` for build, test, and coverage commands.

## C# Conventions

@docs/conventions/csharp.md

## Key Conventions

- **Grid type:** `int[,]` — dimension 0 is **x** (width), dimension 1 is **y** (height)
- **Cell values:** `1` = alive, `0` = dead
- **Pattern format:** newline-delimited digit strings (non-digit characters treated as 0)
- **Grid boundary:** fixed, non-wrapping — out-of-bounds neighbours are treated as dead
- **Rendering:** delta-only (`RenderGridToConsoleDeltasOnly`) — only changed cells are redrawn each tick to reduce flicker

## Simulation Settings (Program.cs constants)

| Constant | Default | Purpose |
|----------|---------|---------|
| `GRID_WIDTH` | 50 | Grid columns |
| `GRID_HEIGHT` | 25 | Grid rows |
| `WAIT` | 100ms | Delay between ticks |
| `MAXIMUM_GENERATIONS` | 0 | Max generations (0 = unlimited) |
| `WINDOW_PADDING` | 5 | Console window margin |

## Adding New Patterns

Add a `public static readonly string` to `src/Patterns.cs` using the digit-string format:
```csharp
namespace GameOfLife.Library
{
    public static class Patterns
    {
        public static readonly string MyPattern =
            "010\n" +
            "111\n" +
            "010";
    }
}
```
Then apply it in `src/Program.cs` via `life.ApplyPattern(Patterns.MyPattern, startX, startY)`.

## Change Intent Records

Non-obvious decisions made during development are documented as Change Intent Records (CIRs) in `docs/cir/`.
See [Change Intent Records](https://blog.bryanl.dev/posts/change-intent-records/) for the rationale.

### Template

**Intent:** What was the goal or objective?

**Behaviour:** What are the expected outcomes? (given/when/then)

**Constraints:** What boundaries or guardrails applied?

**Decisions:** What alternatives were considered and rejected, and why?

**Date:** YYYY-MM-DD
