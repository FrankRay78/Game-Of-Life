# Research: Split Project into Console and Library

**Branch**: `001-split-console-library` | **Date**: 2026-03-23

---

## Decision 1: Library project structure — combined src+tests vs separate test project

**Decision**: Keep `src/` and `tests/` together in `Game-Of-Life.Library` as a single project.

**Rationale**: The user explicitly requested this layout. In .NET, a class library project can host MSTest tests when `Microsoft.NET.Test.Sdk`, `MSTest.TestAdapter`, and `MSTest.TestFramework` are referenced. `dotnet test` discovers and runs tests from library-output projects without requiring `OutputType=Exe`. This is a supported and documented pattern.

**Alternatives considered**:
- Separate `Game-Of-Life.Tests` project — rejected because the user specified two projects, not three.
- Tests remaining in the console project — rejected because it defeats the purpose of the split.

---

## Decision 2: OutputType for the library project

**Decision**: `OutputType` is omitted from the library `.csproj` (SDK default is `Library`).

**Rationale**: The library project does not produce an executable. Omitting `OutputType` (or setting it to `Library`) ensures no entry-point is emitted. MSTest test discovery works against library assemblies when `Microsoft.NET.Test.Sdk` is present.

**Alternatives considered**:
- `OutputType=Exe` in the library — rejected; would require a `Main` method and produce an entry-point assembly, which is incorrect for a library.

---

## Decision 2b: MSTest packages in the console project

**Decision**: The console project (`Game-Of-Life.Console.csproj`) carries **no** MSTest package references. Only `Game-Of-Life.Library.csproj` references `Microsoft.NET.Test.Sdk`, `MSTest.TestAdapter`, and `MSTest.TestFramework`.

**Rationale**: The console project contains no tests. Adding test packages to it would be noise, could confuse test discovery tooling, and violates the principle that the console project is a thin host with minimal dependencies.

---

## Decision 3: RootNamespace across both projects

**Decision**: Both projects retain `GameOfLife` as `RootNamespace`. The library uses `RootNamespace=GameOfLife.Library`; the console uses `RootNamespace=GameOfLife`.

**Rationale**: Changing the namespace of the library types (`Cell`, `Life`, `Helper`, `Patterns`) to `GameOfLife.Library` is the clean, conventional approach that reflects the project name. `Program.cs` in the console adds `using GameOfLife.Library;` to reference library types. This is a one-line change per file and leaves the public API clean.

**Alternatives considered**:
- Keeping `GameOfLife` as the namespace in both — would work at compile time but loses the clarity that namespace ↔ project boundary provides. Rejected.
- `GameOfLife.Console` for `Program.cs` namespace — acceptable but unnecessary; `GameOfLife` is fine for a thin entry-point that has no reusable types.

---

## Decision 4: Project and directory naming (hyphens)

**Decision**: Project directories and `.csproj` filenames use `Game-Of-Life.Library` and `Game-Of-Life.Console` (with hyphens) to match the existing project naming convention. `AssemblyName` is set to `Game-Of-Life.Library` / `Game-Of-Life.Console` in each `.csproj`.

**Rationale**: The existing project is named `Game-Of-Life`. Hyphens are valid in file and directory names on Windows, and in `.csproj` `AssemblyName` values. Preserving the pattern is consistent.

**Alternatives considered**:
- Renaming to `GameOfLife.Library` (no hyphens) — cleaner for NuGet/namespace alignment but breaks the established project-naming convention. Rejected.

---

## Decision 5: Solution file

**Decision**: Update the existing `Game-Of-Life.sln` to reference both new projects and remove the old single-project entry.

**Rationale**: The `.sln` file drives `dotnet build` and IDE project loading. Both projects must be registered so `dotnet build` from the repo root builds the entire solution.

**Alternatives considered**:
- Deleting the `.sln` and relying on directory traversal — not reliable; `dotnet test` and coverage tooling depend on the solution or explicit project paths.

---

## Decision 6: Deterministic builds, Nullable, ImplicitUsings

**Decision**: Both new `.csproj` files carry `<Deterministic>true</Deterministic>`, `<Nullable>disable</Nullable>`, and `<ImplicitUsings>disable</ImplicitUsings>`.

**Rationale**: Required by the constitution (Principle II, Technology Stack). These settings must propagate to all new projects without exception.

---

## Decision 7: [ExcludeFromCodeCoverage] placement

**Decision**: `[ExcludeFromCodeCoverage]` is **removed** from `Program.cs`.

**Rationale**: After the split, `dotnet test` and coverage collection target `Game-Of-Life.Library` exclusively. `Program.cs` lives in `Game-Of-Life.Console`, which is never in scope for coverage measurement. The attribute is therefore redundant and should be deleted. The constitution constitution note about `Program.cs` holding this attribute will be removed as part of the constitution amendment.

---

## Decision 8: CIR and constitution amendment

**Decision**: A new CIR (`docs/cir/CIR-002.md`) is created documenting this architectural decision. The constitution is amended (MINOR version bump: 1.1.0 → 1.2.0) to replace the single-project Technology Stack entry with the new two-project structure and to remove the prohibition on multi-project splits.

**Rationale**: The constitution explicitly requires a CIR and amendment for any change to project structure. The spec (FR-009, FR-010) mandates both. Without them, the PR cannot be merged per the CIR gate and Code Review gate.

---

## Decision 9: CA1416 warnings after split

**Decision**: CA1416 warnings will only appear in the console project (which retains the Windows-specific console API calls). The library project should produce zero CA1416 warnings.

**Rationale**: The rendering logic (`RenderGridToConsoleDeltasOnly`) currently lives in the library alongside game logic. Post-split, if this method uses Windows-specific APIs (`Console.WindowWidth`, etc.) it will generate CA1416 in the library. These warnings are accepted per the constitution and must not be suppressed wholesale.

**Note for implementation**: Verify whether any library-side rendering calls trigger CA1416. If they do, document in the PR (accepted; expected). No action required beyond that.

---

## Unresolved issues

None. All NEEDS CLARIFICATION items resolved.
