<!--
SYNC IMPACT REPORT
==================
Version change: 1.0.0 → 1.1.0

Modified principles:
- II. C# Conventions — added: Nullable and ImplicitUsings are explicitly disabled
- IV. Code Coverage Visibility — added: [ExcludeFromCodeCoverage] approved pattern

Added sections:
- Behavioral Contracts (new section documenting pinned API behaviors)

Expanded sections:
- Technology Stack — added Windows-only targeting, CA1416 accepted warnings,
  single-project architecture, Deterministic builds
- Development Workflow — refined build gate; CA1416 warnings accepted, new
  unrelated warnings must be treated as errors

Removed sections: None

Templates reviewed:
- .specify/templates/plan-template.md          ✅ no updates required
- .specify/templates/spec-template.md          ✅ no updates required
- .specify/templates/tasks-template.md         ✅ no updates required
- .specify/templates/commands/                 ✅ no commands directory present

Deferred TODOs: None

==================
Version change: 1.1.0 → 1.2.0

Modified principles:
- IV. Code Coverage Visibility — removed: [ExcludeFromCodeCoverage] on Program.cs
  (attribute no longer present; console project is outside coverage scope entirely)

Expanded sections:
- Technology Stack — replaced single-project architecture with two-project structure
  (Game-Of-Life.Library class library + Game-Of-Life.Console executable host);
  updated coverage tooling to target Game-Of-Life.Library only; ratified per CIR-003

Removed sections: None

Templates reviewed:
- .specify/templates/plan-template.md          ✅ no updates required
- .specify/templates/spec-template.md          ✅ no updates required
- .specify/templates/tasks-template.md         ✅ no updates required
- .specify/templates/commands/                 ✅ no commands directory present

Deferred TODOs: None
-->

# Game of Life Constitution

## Core Principles

### I. Simplicity First

Every change MUST justify its complexity. The simulation is a focused, single-purpose
console application — new code must serve that purpose directly. YAGNI applies:
do not add configurability, abstractions, or infrastructure for hypothetical future
requirements. Three similar lines of code are preferable to a premature abstraction.

**Rationale**: Complexity accumulates invisibly. Keeping Game of Life simple ensures
it remains a clear reference implementation and a maintainable learning artefact.

### II. C# Conventions (NON-NEGOTIABLE)

All code MUST conform to `docs/conventions/csharp.md`. Key non-negotiables:
- PascalCase for classes, methods, public properties, and constants (no ALL_CAPS).
- `var` only when the right-hand side makes the type obvious.
- File-scoped namespaces; one class per file; filename matches class name.
- Allman braces; always use braces even for single-line bodies.
- Member order: static fields → instance fields → constructors → properties → methods.
- `Nullable` and `ImplicitUsings` are explicitly **disabled** in the project file
  (per CIR-001). They MUST NOT be enabled without a CIR documenting the rationale
  and a constitution amendment.

**Rationale**: Consistent style reduces cognitive load when navigating the codebase
and ensures contributions read as though written by a single author. The nullable
and implicit-usings settings are frozen to avoid unrelated annotation noise.

### III. Test-First Development

Tests MUST be written and confirmed to fail before the corresponding implementation
is added. Tests follow MSTest v4.x with the Arrange / Act / Assert pattern. Test
names use the format `MethodName_Scenario_ExpectedResult`. Happy paths and edge
cases MUST both be covered.

**Rationale**: Writing tests first surfaces design issues early and ensures the
suite has genuine discriminating power — a test that was never red provides no
confidence.

### IV. Code Coverage Visibility

Code coverage MUST be measurable at any time via the pinned `dotnet-coverage` and
`reportgenerator` tools in `.config/dotnet-tools.json`. HTML reports are generated
into `coverage/report/`. Coverage is a signal, not a target — declining coverage
in changed areas MUST be explained before merging.

Only code that is genuinely untestable in a unit context (e.g., console I/O, OS
window management) MAY carry `[ExcludeFromCodeCoverage]`. Currently no production
code carries this attribute — `Program.cs` is in `Game-Of-Life.Console` which is
outside coverage scope. Adding `[ExcludeFromCodeCoverage]` to library code requires
explicit justification in the PR.

**Rationale**: Invisible coverage encourages false confidence. Keeping tools pinned
and runnable with a single command removes the friction that causes coverage to go
unmonitored. Constraining `[ExcludeFromCodeCoverage]` prevents it from being used
to silently suppress gaps.

### V. Change Intent Records

Non-obvious decisions MUST be recorded as Change Intent Records (CIRs) in
`docs/cir/`. Each CIR contains: **Intent**, **Behaviour** (given/when/then),
**Constraints**, **Decisions** (alternatives considered and rejected), and **Date**.
Routine changes (typo fixes, minor refactors) do not require a CIR.

**Rationale**: Code shows what was done; CIRs preserve why. Without this record,
future contributors repeat rejected alternatives and re-open closed debates.

## Behavioral Contracts

These behaviors are **pinned by tests** and MUST NOT be changed without a CIR and
corresponding test updates. They represent deliberate design decisions, not bugs.

**`ApplyPattern` boundary overflow throws**
`Life.ApplyPattern(pattern, startX, startY)` performs no bounds checking. If the
pattern extends beyond the grid boundary, an `IndexOutOfRangeException` is thrown.
This is intentional — the caller is responsible for placement. Do not add a guard
without a CIR.

**Ragged pattern rows are padded**
`Helper.StringToIntMatrix` supports ragged pattern strings. Rows shorter than the
widest row are right-padded with zeros. This means patterns do not need to be
rectangular.

**Non-digit characters in patterns are treated as 0**
Any character in a pattern string that is not a digit (0–9) is treated as dead (`0`).
This includes letters, spaces, and symbols. Digit values are preserved as-is (not
clamped to 0/1), so `2`, `3`, `9` remain valid cell values even though the
simulation only distinguishes alive (non-zero) from dead (zero) at the rule level.

**Pattern naming uses `Snake_Case_Category_Name`**
Public pattern constants in `Patterns.cs` use `Snake_Case` with category prefixes
(e.g., `Still_Life_Block`, `Oscillator_Blinker`, `R_Pentomino`). New patterns MUST
follow this form.

## Technology Stack

- **Language / Runtime**: C# targeting .NET 10
- **Platform target**: Windows — console rendering uses Windows-specific APIs
  (`Console.WindowWidth`, `Console.WindowHeight`). CA1416 platform-compatibility
  warnings are expected and accepted; they MUST NOT be suppressed wholesale.
- **Project structure**: Two `.csproj` files (per CIR-003):
  - `Game-Of-Life.Library` — class library containing all game logic (`src/`) and
    tests (`tests/`). No dependency on the console host.
  - `Game-Of-Life.Console` — executable containing only the entry-point (`src/Program.cs`).
    References `Game-Of-Life.Library` via `ProjectReference`. No MSTest packages.
  Changes to this structure require a CIR and a constitution amendment.
- **Build determinism**: `<Deterministic>true</Deterministic>` is set. Builds MUST
  remain deterministic; do not introduce non-deterministic inputs (timestamps,
  random seeds at build time, etc.).
- **Nullable / ImplicitUsings**: Both disabled. See Principle II.
- **Test framework**: MSTest v4.x
- **Coverage tools**: `dotnet-coverage` + `reportgenerator`
  (pinned in `.config/dotnet-tools.json`)
- **Build / run**: `dotnet` CLI (`dotnet build`, `dotnet test`, `dotnet run`)
- **Grid representation**: `int[,]` — dimension 0 is x (width), dimension 1 is y
  (height); cell values: `1` = alive, `0` = dead
- **Boundary**: fixed, non-wrapping — out-of-bounds neighbours are treated as dead
- **Rendering**: delta-only (`RenderGridToConsoleDeltasOnly`) to minimise console
  flicker; previous grid is cached each tick for comparison
- **Pattern format**: newline-delimited digit strings; non-digit characters treated
  as 0; ragged rows padded with zeros (see Behavioral Contracts)

Deviations from this stack require a CIR and a constitution amendment.

## Development Workflow

- **Build gate**: `dotnet build` MUST produce zero errors before any PR is opened.
  CA1416 platform-compatibility warnings are expected (Windows-only console APIs)
  and are accepted. Any new warning **not** related to CA1416 MUST be treated as an
  error and resolved before merging.
- **Test gate**: `dotnet test` MUST pass in full before merging.
- **Coverage check**: Run `dotnet tool run dotnet-coverage …` + `reportgenerator`
  and review the HTML report when touching production code.
- **CIR gate**: Any non-obvious decision encountered during development MUST be
  recorded in `docs/cir/` before the PR is merged.
- **Code review**: All PRs MUST verify compliance with each Core Principle above.
  Complexity introduced without justification is grounds for rejection.

## Governance

This constitution supersedes all other practices, preferences, and informal
agreements. When a conflict exists between this document and any other source
of guidance, this document prevails.

**Amendment procedure**: Propose the change, document the rationale as a CIR
(or inline in the PR description for constitution-only changes), increment the
version following semantic versioning (see below), and update `LAST_AMENDED_DATE`.

**Versioning policy**:
- MAJOR: backward-incompatible governance change — principle removed or fundamentally
  redefined in a way that invalidates prior compliance.
- MINOR: new principle or section added, or materially expanded guidance.
- PATCH: clarifications, wording improvements, typo fixes, non-semantic refinements.

**Compliance review**: Verify constitution compliance on every PR. Any reviewer
may block a merge for a compliance violation; the author MUST address it (not simply
override it).

**Runtime guidance**: Use `CLAUDE.md` for day-to-day development guidance that
does not rise to the level of a constitutional principle.

---

**Version**: 1.2.0 | **Ratified**: 2026-03-23 | **Last Amended**: 2026-03-23
