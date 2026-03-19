**Intent:** Migrate from .NET Framework 4.7.2 to .NET 10 to eliminate machine-specific build tooling and enable portable `dotnet` CLI workflows.

**Behaviour:**
- Given a fresh clone with .NET 10 SDK installed, `dotnet build` succeeds with no errors
- Given a fresh clone, `dotnet test` runs all existing tests and they pass
- Given a fresh clone, `dotnet run` launches the Game of Life simulation
- `build.bat` no longer exists in the repository

**Constraints:**
- Must preserve all existing test coverage and behaviour exactly
- No code logic changes — structural/tooling migration only
- `Nullable` and `ImplicitUsings` left disabled to avoid annotation noise unrelated to the migration goal

**Decisions:**
- **Single project over two projects**: Kept source and tests in one `.csproj` at the repo root rather than splitting into separate `src/` and `tests/` projects. Rationale: the codebase is small; a separate test project assembly adds overhead with no current benefit. `dotnet test` works on EXE projects in .NET 5+.
- **`GameOfLife.Library` namespace for core types**: Renamed `Game_Of_Life_LIB` to `GameOfLife.Library` rather than collapsing everything into `GameOfLife`. Rationale: preserves the logical separation between rendering (`GameOfLife`) and reusable core logic (`GameOfLife.Library`), even though they now share one project file.
- **MSTest 3.x over xUnit/NUnit**: Retained MSTest to minimise diff — the existing tests use MSTest 1.x APIs which are fully compatible with 3.x. Switching frameworks would change every test file for no functional gain.
- **Removed OpenCover + ReportGenerator**: These tools required hardcoded paths and net472 targets. Coverage in .NET 10 is handled via `dotnet test --collect:"XPlat Code Coverage"` with no project-level config.

**Date:** 2026-03-19
