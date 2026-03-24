# Tasks: Split Project into Console and Library

**Input**: Design documents from `/specs/001-split-console-library/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, quickstart.md ✅

**Tests**: No new test tasks — all existing tests are migrated as-is. The constitution requires test-first only for new logic; this is a pure structural move.

**Organization**: Tasks grouped by user story to enable independent delivery of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)

---

## Phase 1: Setup (Create New Project Scaffolding)

**Purpose**: Create both new `.csproj` files and register them in the solution. This is the only prerequisite that blocks US1 and US2.

- [x] T001 Create `Game-Of-Life.Library/Game-Of-Life.Library.csproj` — `OutputType` omitted (defaults to Library), `TargetFramework=net10.0`, `AssemblyName=Game-Of-Life.Library`, `RootNamespace=GameOfLife.Library`, `Nullable=disable`, `ImplicitUsings=disable`, `Deterministic=true`; include MSTest package references (`Microsoft.NET.Test.Sdk` 18.3.*, `MSTest.TestAdapter` 4.1.*, `MSTest.TestFramework` 4.1.*)
- [x] T002 [P] Create `Game-Of-Life.Console/Game-Of-Life.Console.csproj` — `OutputType=Exe`, `TargetFramework=net10.0`, `AssemblyName=Game-Of-Life.Console`, `RootNamespace=GameOfLife`, `Nullable=disable`, `ImplicitUsings=disable`, `Deterministic=true`, `StartupObject=GameOfLife.Program`; include only a `ProjectReference` to `..\Game-Of-Life.Library\Game-Of-Life.Library.csproj` — no MSTest packages
- [x] T003 Update `Game-Of-Life.sln` to add both new projects (`Game-Of-Life.Library/Game-Of-Life.Library.csproj` and `Game-Of-Life.Console/Game-Of-Life.Console.csproj`) and remove the old root `Game-Of-Life.csproj` project entry

**Checkpoint**: Both new projects are registered in the solution. No source files have been moved yet.

---

## Phase 2: User Story 1 — Library Project Exists, Builds in Isolation, All Tests Pass (Priority: P1) 🎯 MVP

**Goal**: Move all game logic and tests into `Game-Of-Life.Library`, update namespaces, confirm the library builds and its full test suite passes independently.

**Independent Test**: `dotnet build Game-Of-Life.Library` succeeds with zero errors; `dotnet test Game-Of-Life.Library` passes with all tests green.

### Implementation for User Story 1

- [x] T004 [P] [US1] Move `src/Cell.cs` to `Game-Of-Life.Library/src/Cell.cs` and update the file-scoped namespace declaration to `GameOfLife.Library`
- [x] T005 [P] [US1] Move `src/Helper.cs` to `Game-Of-Life.Library/src/Helper.cs` and update the file-scoped namespace declaration to `GameOfLife.Library`
- [x] T006 [P] [US1] Move `src/Life.cs` to `Game-Of-Life.Library/src/Life.cs` and update the file-scoped namespace declaration to `GameOfLife.Library`
- [x] T007 [P] [US1] Move `src/Patterns.cs` to `Game-Of-Life.Library/src/Patterns.cs` and update the file-scoped namespace declaration to `GameOfLife.Library`
- [x] T008 [P] [US1] Move `tests/CellTests.cs` to `Game-Of-Life.Library/tests/CellTests.cs` and update `using GameOfLife;` to `using GameOfLife.Library;`
- [x] T009 [P] [US1] Move `tests/ExamplePatternTests.cs` to `Game-Of-Life.Library/tests/ExamplePatternTests.cs` and update `using GameOfLife;` to `using GameOfLife.Library;`
- [x] T010 [P] [US1] Move `tests/HelperTests.cs` to `Game-Of-Life.Library/tests/HelperTests.cs` and update `using GameOfLife;` to `using GameOfLife.Library;`
- [x] T011 [P] [US1] Move `tests/LifeTests.cs` to `Game-Of-Life.Library/tests/LifeTests.cs` and update `using GameOfLife;` to `using GameOfLife.Library;`
- [x] T012 [US1] Delete the old root `Game-Of-Life.csproj` file and remove the now-empty `src/` and `tests/` root directories (depends on T004–T011)
- [x] T013 [US1] Run `dotnet build Game-Of-Life.Library` — verify zero errors and no unexpected warnings; then run `dotnet test Game-Of-Life.Library` — verify all tests pass (depends on T012)

**Checkpoint**: `Game-Of-Life.Library` builds and all tests pass in complete isolation from the console project.

---

## Phase 3: User Story 2 — Console Project Is a Thin Host, Simulation Runs Identically (Priority: P2)

**Goal**: Move `Program.cs` into `Game-Of-Life.Console`, wire up the library reference, remove the now-redundant `[ExcludeFromCodeCoverage]` attribute, and confirm the simulation runs identically.

**Independent Test**: `dotnet run --project Game-Of-Life.Console` starts the simulation and it runs and renders identically to pre-split behaviour.

### Implementation for User Story 2

- [x] T014 [US2] Move `src/Program.cs` to `Game-Of-Life.Console/src/Program.cs`; add `using GameOfLife.Library;` at the top; remove the `[ExcludeFromCodeCoverage]` attribute from the class and remove its `using System.Diagnostics.CodeAnalysis;` directive if it is no longer used elsewhere in the file (depends on T013)
- [x] T015 [US2] Run `dotnet run --project Game-Of-Life.Console` and confirm the simulation starts, renders the grid, and progresses through generations as before (depends on T014)

**Checkpoint**: Full solution builds (`dotnet build`), tests pass (`dotnet test Game-Of-Life.Library`), and simulation runs correctly.

---

## Phase 4: User Story 3 — Project Documentation Updated (Priority: P3)

**Goal**: Create CIR-002 recording the architectural decision, amend the constitution to reflect and ratify the two-project structure, and update CLAUDE.md.

**Independent Test**: All three documents are present, committed, and accurately describe the new layout and the rationale for the change.

### Implementation for User Story 3

- [x] T016 [P] [US3] Create `docs/cir/003-split-console-library.md` — document **Intent** (separate library code from console host), **Behaviour** (given/when/then scenarios from spec US1–US3), **Constraints** (constitution compliance, no logic changes, all tests pass), **Decisions** (single-project alternative rejected because it bundles reusable code inside an executable — the "dirty pattern" the user identified; three-project split rejected because the user specified exactly two)
- [x] T017 [P] [US3] Amend `.specify/memory/constitution.md` — in the Technology Stack section, replace the single-project rule with the two-project structure (`Game-Of-Life.Library` + `Game-Of-Life.Console`); remove the prohibition on multi-project splits; remove the sentence about `[ExcludeFromCodeCoverage]` being on `Program.cs` only (it is now removed entirely); update the coverage tooling section to target `Game-Of-Life.Library`; bump version from 1.1.0 to 1.2.0 and update `LAST_AMENDED_DATE` to 2026-03-23
- [x] T018 [US3] Update `CLAUDE.md` — update the Tech Stack section and any project-structure references to describe the new two-project layout; update build/test/run commands to use `--project` flags as shown in `quickstart.md` (depends on T016 and T017 for consistency)

**Checkpoint**: CIR-002, amended constitution, and updated CLAUDE.md are all present and consistent with the new layout.

---

## Final Phase: Polish & Full Solution Verification

**Purpose**: End-to-end validation that the entire solution is coherent.

- [x] T019 Run `dotnet build` from the repo root — confirm zero errors; confirm the only CA1416 warnings (if any) are in `Game-Of-Life.Library` rendering code and are accepted per the constitution; confirm no unexpected new warnings
- [x] T020 Run `dotnet test Game-Of-Life.Library` one final time from the repo root — confirm all tests pass with zero failures
- [x] T021 [P] Review `specs/001-split-console-library/quickstart.md` coverage commands; run `dotnet tool run dotnet-coverage collect "dotnet test Game-Of-Life.Library" -f xml -o coverage/coverage.xml` and confirm it succeeds against the new project path

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **US1 (Phase 2)**: Depends on Setup (T001–T003) — T004–T011 can run in parallel once T003 is done; T012 depends on T004–T011; T013 depends on T012
- **US2 (Phase 3)**: Depends on US1 checkpoint (T013) — console project references the library, so library must build first
- **US3 (Phase 4)**: Can start after Setup (T003); T016 and T017 are parallel; T018 depends on T016 and T017
- **Final Phase**: Depends on US1, US2, and US3 complete

### User Story Dependencies

- **US1 (P1)**: Starts after Phase 1 — no dependency on US2 or US3
- **US2 (P2)**: Starts after US1 checkpoint — console project depends on a working library
- **US3 (P3)**: Starts after Phase 1 — documentation is independent of code moves

### Parallel Opportunities

- T001 and T002 (Phase 1) can run in parallel — different files
- T004–T011 (US1 file moves) can all run in parallel — each is a different file
- T016 and T017 (US3) can run in parallel — different files
- US3 documentation work can proceed alongside US1 and US2 code work

---

## Parallel Example: User Story 1

```text
# All of these can run in parallel after T003 completes:
T004 — move Cell.cs + update namespace
T005 — move Helper.cs + update namespace
T006 — move Life.cs + update namespace
T007 — move Patterns.cs + update namespace
T008 — move CellTests.cs + update using
T009 — move ExamplePatternTests.cs + update using
T010 — move HelperTests.cs + update using
T011 — move LifeTests.cs + update using

# Then sequentially:
T012 — delete old .csproj + empty directories
T013 — build + test verification
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001–T003)
2. Complete Phase 2: US1 (T004–T013)
3. **STOP and VALIDATE**: `dotnet build Game-Of-Life.Library` + `dotnet test Game-Of-Life.Library`
4. Library is independently verified — US1 is done

### Incremental Delivery

1. Setup → US1 → library verified independently (**MVP**)
2. US2 → console verified, simulation runs end-to-end
3. US3 → documentation complete, PR ready to merge

---

## Notes

- No new tests are written — this is a pure structural move. All existing tests travel with the library.
- Namespace changes are mechanical: `GameOfLife` → `GameOfLife.Library` in moved source and test files; `Program.cs` keeps `GameOfLife` namespace but gains `using GameOfLife.Library;`.
- The old root `Game-Of-Life.csproj` was removed from the `.sln` in T003. T012 deletes the orphaned file from disk.
- `[ExcludeFromCodeCoverage]` is removed (not kept) — it is redundant once the console project is outside coverage scope.
- Commit after T013 (US1 checkpoint) and again after T015 (US2 checkpoint) to preserve clean rollback points.
