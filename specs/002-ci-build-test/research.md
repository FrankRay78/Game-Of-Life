# Research: Core CI — Automated Build and Test

**Phase 0 output** | Date: 2026-03-25

---

## Decision: `pull_request` trigger event types

**Chosen**: `types: [opened, synchronize, reopened]`

**Rationale**: GitHub Actions `pull_request` fires on `opened`, `synchronize`, and
`reopened` by default when no `types` filter is specified. Explicitly declaring all
three is clearer and defensive — it ensures that re-opened PRs (after being closed)
also get validated. `synchronize` is the event that fires when commits are pushed to
an existing PR branch, which is required for FR-006 (automatic re-evaluation on new commits).

**Alternatives considered**:
- `types: [opened, synchronize]` only — excludes `reopened`, meaning a re-opened PR
  would carry a stale check result until the next push. Marginal risk but inconsistent
  UX; rejected in favour of including `reopened`.
- No explicit `types` (use defaults) — functionally equivalent but less explicit;
  rejected in favour of self-documenting YAML.

---

## Decision: dotnet step structure (separate steps vs combined)

**Chosen**: Three separate steps — `dotnet restore`, `dotnet build --no-restore`,
`dotnet test --no-build`

**Rationale**: Separating restore, build, and test into distinct steps gives GitHub
Actions a clean, named stage for each operation. Failures are immediately attributable
to the correct step (e.g., "Restore failed" vs "Build failed" vs "Test failed").
`--no-restore` and `--no-build` flags prevent each step from redundantly repeating
prior work, keeping the overall run time optimal.

**Alternatives considered**:
- Single `dotnet test` command (which implicitly restores and builds) — simpler YAML
  but produces a single undifferentiated step that blurs the failure attribution.
  Rejected for clarity of failure diagnosis.
- `dotnet build` then `dotnet test Game-Of-Life.Library` (no explicit restore step) —
  `dotnet build` includes restore by default, so the restore step can be omitted.
  However, an explicit restore step makes dependency fetching transparent and separates
  network-dependent work from compilation work. Retained for observability.

---

## Decision: `--configuration Release` for build and test

**Chosen**: `--configuration Release` on both `dotnet build` and `dotnet test`

**Rationale**: Using Release configuration ensures CI validates the same binary that
would be shipped, not a debug build with different optimisations and conditional code.
Consistency between build and test steps is enforced via `--no-build` on the test step.

**Alternatives considered**:
- Omit `--configuration` (defaults to Debug) — tests a configuration that diverges
  from production. Rejected to avoid false confidence.
- Use Debug explicitly — same concern as above. Rejected.

---

## Decision: `actions/setup-dotnet` requirement

**Chosen**: Include `actions/setup-dotnet@v4` with `dotnet-version: '10.x'`

**Rationale**: While GitHub-hosted `ubuntu-latest` runners pre-install several .NET
SDK versions, .NET 10 is a recent release and may not be pre-installed or may be
an older patch version. Explicitly setting up the SDK with `dotnet-version: '10.x'`
pins to the latest .NET 10 patch, ensures reproducibility, and avoids silent version
drift as runner images change over time.

**Alternatives considered**:
- Rely on pre-installed SDK — fragile; runner image contents change without notice.
  Rejected for reproducibility.
- Pin a specific patch version (e.g., `10.0.100`) — too granular; `10.x` floats to
  the latest patch automatically, which is the correct behaviour for a non-library CI.

**Note**: Use `actions/setup-dotnet@v5` — v5 adds explicit .NET 10 support and is
the current recommended version as of early 2026. v4 lacks .NET 10 guarantees.

---

## Decision: Status check name and branch protection integration

**Chosen**: Job name `build-and-test`; referenced by exact name in branch protection rule

**Rationale**: GitHub Actions creates a check run for each job in a workflow, named
by the job key. The job key `build-and-test` becomes the check name shown on the PR
status section. Branch protection rules reference this exact string as a required
status check. The name is descriptive, hyphen-separated (GitHub convention), and
unambiguous.

**Alternatives considered**:
- Shorter name (e.g., `ci`) — less descriptive; when spec 003 adds a coverage job,
  `ci` becomes ambiguous. `build-and-test` clearly scopes the check to this feature.
- Display name via `name:` field on the job — the `name:` field overrides the display
  name in the UI but the *check run name* used by branch protection still uses the job
  key. Using a display name without changing the job key adds confusion. Not needed.

---

## Decision: `dotnet test` target — project path vs solution

**Chosen**: `dotnet test Game-Of-Life.Library`

**Rationale**: The spec explicitly states tests run against `Game-Of-Life.Library`
only (`Game-Of-Life.Console` has no tests). Targeting the library project directly
avoids any ambiguity and matches the local test command in CLAUDE.md.

**Alternatives considered**:
- `dotnet test` (solution-level) — discovers test projects automatically but includes
  `Game-Of-Life.Console`, which has no test runner configured and may produce spurious
  warnings or errors. Rejected for precision.

---

## Decision: Test result visibility

**Chosen**: `--verbosity normal` on `dotnet test`; no third-party test reporter action

**Rationale**: `--verbosity normal` causes MSTest to emit test counts (passed/failed/
skipped) to stdout, which is captured in the GitHub Actions step log and satisfies
FR-004 and FR-005. GitHub Actions also natively renders MSTest `[TestMethod]` failure
messages as annotations on the PR. No additional tooling is needed.

**Alternatives considered**:
- `--logger "trx"` + artifact upload — produces a downloadable XML report but requires
  extra steps and a third-party action to render it in the PR. Adds complexity without
  meaningful benefit over the log output. Rejected (constitution Principle I).
- Third-party actions (`dorny/test-reporter`, `EnricoMi/publish-unit-test-result-action`)
  — provide richer PR annotations but introduce external dependencies and maintenance
  surface. Rejected; standard log output is sufficient for this project's scale.
