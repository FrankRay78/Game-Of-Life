# Implementation Plan: Split Project into Console and Library

**Branch**: `001-split-console-library` | **Date**: 2026-03-23 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-split-console-library/spec.md`

## Summary

Restructure the repository from a single `.csproj` (Exe + tests) into two projects: `Game-Of-Life.Library` (class library containing all game logic and tests) and `Game-Of-Life.Console` (thin console host containing only `Program.cs`). Accompanied by a CIR and a constitution amendment to formally record and ratify the architectural decision.

## Technical Context

**Language/Version**: C# targeting .NET 10
**Primary Dependencies**: MSTest v4.x (in library project), `Microsoft.NET.Test.Sdk` 18.3.x
**Storage**: N/A
**Testing**: `dotnet test Game-Of-Life.Library` — MSTest runs from a class library project when `Microsoft.NET.Test.Sdk` is referenced
**Target Platform**: Windows (Console, Windows-specific APIs)
**Project Type**: Class library (`Game-Of-Life.Library`) + Console executable (`Game-Of-Life.Console`)
**Performance Goals**: No change — pure structural reorganisation
**Constraints**: `Nullable` and `ImplicitUsings` disabled; `Deterministic` enabled; no new CA1416 suppressions; all existing tests must pass unchanged; console project has no MSTest references; `[ExcludeFromCodeCoverage]` removed from `Program.cs`
**Scale/Scope**: 4 source files + 4 test files + 1 entry-point file

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Simplicity First | ✅ Pass | Pure structural move; no new abstractions or features |
| II. C# Conventions | ✅ Pass | All files retain correct naming, braces, member order; namespace update is minimal |
| III. Test-First Development | ✅ Pass | No new logic; existing tests continue to cover existing behaviour |
| IV. Code Coverage Visibility | ✅ Pass | `dotnet test` targets the library project; coverage tooling path updated in quickstart |
| V. Change Intent Records | ✅ Pass | CIR-002 is a required deliverable of this feature |
| Technology Stack — single-project rule | ⚠️ Violation (justified) | Constitution explicitly prohibits multi-project split. This feature IS the CIR + constitutional amendment. See Complexity Tracking. |

**Post-design re-check**: All items still pass. The justified violation is fully documented.

## Project Structure

### Documentation (this feature)

```text
specs/001-split-console-library/
├── plan.md              ← this file
├── research.md          ← Phase 0 complete
├── data-model.md        ← Phase 1 complete
├── quickstart.md        ← Phase 1 complete
└── tasks.md             ← Phase 2 output (/speckit.tasks — not yet created)
```

### Source Code (repository root — target layout)

```text
Game-Of-Life/
├── Game-Of-Life.sln                         ← updated to reference both projects
│
├── Game-Of-Life.Library/
│   ├── Game-Of-Life.Library.csproj          ← new; OutputType=Library; MSTest refs
│   ├── src/
│   │   ├── Cell.cs                          ← moved from src/
│   │   ├── Helper.cs                        ← moved from src/
│   │   ├── Life.cs                          ← moved from src/
│   │   └── Patterns.cs                      ← moved from src/
│   └── tests/
│       ├── CellTests.cs                     ← moved from tests/
│       ├── ExamplePatternTests.cs           ← moved from tests/
│       ├── HelperTests.cs                   ← moved from tests/
│       └── LifeTests.cs                     ← moved from tests/
│
└── Game-Of-Life.Console/
    ├── Game-Of-Life.Console.csproj          ← new; OutputType=Exe; ProjectReference to library
    └── src/
        └── Program.cs                       ← moved from src/; adds using GameOfLife.Library;
```

**Old artefacts removed**: `Game-Of-Life.csproj` (root), `src/` (root), `tests/` (root)

**Structure Decision**: Two-project layout as described above. Tests co-located with library source in `Game-Of-Life.Library`. Console project is a thin host with a single source file.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Multi-project split (breaks Technology Stack single-project rule) | Separating library logic from the console host removes the anti-pattern of bundling reusable code inside an executable. Library code becomes independently testable and reusable without a console dependency. | Keeping a single project leaves the clean-architecture concern unresolved. The user explicitly identified this as a "dirty pattern" worth correcting. The CIR and constitution amendment make the decision formal and auditable. |
