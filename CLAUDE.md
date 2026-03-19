# Game of Life

Conway's Game of Life implemented as a C# console application with a retro 80s aesthetic.

## Tech Stack

- **Language:** C# targeting .NET Framework 4.7.2
- **IDE:** Visual Studio 2019
- **Test framework:** MSTest v1.3.2
- **Coverage:** OpenCover + ReportGenerator

## Project Structure

```
Game-Of-Life/           # Console EXE — rendering only (Program.cs)
Game-Of-Life_LIB/       # Core logic (no UI dependency)
  Cell.cs               # Conway's rules — stateless, static
  Life.cs               # Grid + generation management
  Helper.cs             # String <-> int[,] serialization
  Patterns.cs           # Named starting patterns (readonly strings)
Game-Of-Life_TESTS/     # MSTest unit tests
  Life_Tests.cs
  Helper_Tests.cs
  Example-Pattern-Tests.cs
```

## Build & Test

`build.bat` runs the full pipeline: NuGet restore → MSBuild → vstest → OpenCover → ReportGenerator.

> ⚠️ Paths in `build.bat` are hardcoded to a specific machine. Update them before running on a new machine.

To build manually with Visual Studio: open `Game-Of-Life.sln` and build in Debug configuration.

To run tests manually:
```
vstest.console.exe "Game-Of-Life_TESTS\bin\Debug\Game-Of-Life_TESTS.dll" /TestAdapterPath:"packages\MSTest.TestAdapter.1.3.2"
```

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

Add a `public static readonly string` to `Game-Of-Life_LIB/Patterns.cs` using the digit-string format:
```csharp
public static readonly string MyPattern =
    "010\n" +
    "111\n" +
    "010";
```
Then apply it in `Program.cs` via `life.ApplyPattern(Patterns.MyPattern, startX, startY)`.
