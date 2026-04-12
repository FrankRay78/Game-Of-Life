# Game of Life

Conway's Game of Life implemented as a C# console application with a retro 80s aesthetic.

## Tech Stack

- **Language:** C# targeting .NET 10
- **Test framework:** MSTest v4.x
- **Build/test:** `dotnet` CLI

## Project Structure

| Project | Type | Contents |
|---------|------|----------|
| `Game-Of-Life.Library` | Class library | All game logic (`src/`) and tests (`tests/`) |
| `Game-Of-Life.Console` | Executable | Entry-point only (`src/Program.cs`) |

The console project references the library via `ProjectReference`. Tests run against the library project only.

See `README.md` for build, test, and coverage commands.

## General Principles

@docs/conventions/general-principles.md

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
| `GridWidth` | 50 | Grid columns |
| `GridHeight` | 25 | Grid rows |
| `Wait` | 100ms | Delay between ticks |
| `MaximumGenerations` | 0 | Max generations (0 = unlimited) |
| `YOffsetWhenRendering` | 2 | Vertical cursor offset when rendering the grid |
| `WindowPadding` | 5 | Console window margin |

## Common Commands

```bash
dotnet build                                        # build entire solution
dotnet test Game-Of-Life.Library                    # run all tests
dotnet run --project Game-Of-Life.Console           # run the simulation
```

## Adding New Patterns

Add a `public static readonly string` to `Game-Of-Life.Library/src/Patterns.cs` using the digit-string format:
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
Then apply it in `Game-Of-Life.Console/src/Program.cs` via `life.ApplyPattern(Patterns.MyPattern, startX, startY)`.
