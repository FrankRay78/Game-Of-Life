**Intent:** Separate reusable game logic from the console host by splitting the single `.csproj` into two projects: `Game-Of-Life.Library` (class library, contains all game logic and tests) and `Game-Of-Life.Console` (executable, contains only the entry-point).

**Behaviour:**
- Given the repository is checked out, when `dotnet build Game-Of-Life.Library` is run in isolation, then it compiles successfully with zero errors and no dependency on any console host
- Given the library project, when `dotnet test Game-Of-Life.Library` is run, then all 36 tests pass with zero failures
- Given the solution, when `dotnet build` is run from the repo root, then both projects compile — CA1416 platform-compatibility warnings appear only in `Game-Of-Life.Console` (expected, accepted per constitution)
- Given the console project, when `dotnet run --project Game-Of-Life.Console` is run, then the simulation starts and runs identically to pre-split behaviour
- `[ExcludeFromCodeCoverage]` is removed from `Program.cs` entirely — the console project is outside coverage scope

**Constraints:**
- No logic changes — pure structural reorganisation; all existing tests pass unchanged
- Both projects retain `Nullable=disable`, `ImplicitUsings=disable`, `Deterministic=true`
- The console project has no MSTest package references
- The library project has no `ProjectReference` entries
- Constitution amended (1.1.0 → 1.2.0) to ratify the new structure

**Decisions:**
- **Two projects over three**: The user explicitly specified `Game-Of-Life.Library` and `Game-Of-Life.Console` as the target structure. A third `Game-Of-Life.Tests` project was not requested and would add overhead for no current benefit — tests co-located in the library serve the same purpose.
- **Single project (status quo) rejected**: CIR-001 preserved the single-project structure as a pragmatic choice for a small codebase. With the `GameOfLife.Library` namespace already in place (also from CIR-001), the project boundary was always the missing structural element. Keeping the library code inside an executable prevents it from being referenced cleanly by any future consumer and mixes concerns that the namespace already signals should be separate.
- **`[ExcludeFromCodeCoverage]` removed**: Once `Program.cs` moves to the console project, `dotnet test` targets only the library, making the attribute redundant. Removing it keeps the codebase honest — nothing is excluded from coverage without a genuine reason.
- **Namespace unchanged**: `GameOfLife.Library` was already the namespace for all library types (per CIR-001). No source file required a namespace edit; only the physical project boundary was missing.

**Date:** 2026-03-23
