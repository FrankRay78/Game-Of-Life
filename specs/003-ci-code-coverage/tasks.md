# Tasks: Code Coverage in CI

**Input**: Design documents from `/specs/003-ci-code-coverage/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

**Tests**: Not applicable — this feature delivers workflow YAML and a Python helper script with no unit-testable logic.

**Organization**: Three user stories map to two output files (`ci.yml` modified, `extract-coverage.py` new). Tasks are sequenced: CIR amendment → script creation → ci.yml changes in story order.

**Dependency on spec 002**: This feature modifies `.github/workflows/ci.yml` introduced by spec 002. Spec 002 must be implemented before these tasks execute.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to

---

## Phase 1: Setup (Pre-flight)

**Purpose**: Amend the existing CIR before writing any code — the change in comment behaviour (new comment per run vs. update-in-place) directly contradicts the current CIR-004 and must be recorded.

- [ ] T001 Amend `docs/cir/004-ci-coverage-reporting.md` to record the revised comment behaviour decision

  > **Amendment content**: In the Behaviour section, replace "the existing coverage comment is updated in place — not duplicated" with "a new comment is posted on every CI run; earlier comments remain as an audit trail of coverage changes across commits". Add a new dated amendment note explaining the change was user-directed on 2026-03-25 to preserve per-commit history. Keep the rest of the CIR intact — the choice of PR comment format (Option A) is unchanged.

**Checkpoint**: CIR amended — implementation can proceed.

---

## Phase 2: Foundational (Blocking Prerequisite)

**Purpose**: Create the coverage extraction script before any ci.yml changes. All three user stories depend on this script existing.

- [ ] T002 [P] Create `.github/scripts/extract-coverage.py` per the interface contract in `specs/003-ci-code-coverage/contracts/workflow-changes.md`

  > **Required behaviour** (do not deviate):
  > - Reads `coverage/coverage.cobertura.xml` (path hardcoded relative to repo root)
  > - Parses Cobertura XML using Python 3 `xml.etree.ElementTree` (stdlib only — no pip installs)
  > - Filters to `<class>` elements whose `filename` attribute contains `/src/` and ends with `.cs`
  > - For each matching class, derives:
  >   - `basename` — `os.path.basename(filename)` for the File column
  >   - `lines_valid` — count of `<line>` child elements
  >   - `lines_covered` — count of `<line>` elements with `hits > 0`
  >   - `line_pct` — `round(float(cls.get('line-rate', 0)) * 100)`
  >   - `branches_valid` — sum of denominators from `condition-coverage` attributes (e.g. `"100% (2/2)"` → `2`); `0` if no branch lines
  >   - `branches_covered` — sum of numerators from same attribute; `0` if no branch lines
  >   - `branch_pct` — `round(float(cls.get('branch-rate', 0)) * 100)`; display as `N/A` if `branches_valid == 0`
  > - Sorts rows alphabetically by basename
  > - Prints the markdown table to stdout in the exact format from `specs/003-ci-code-coverage/spec.md` PR Comment Format section
  > - Exits with code `1` if `coverage/coverage.cobertura.xml` is missing or cannot be parsed
  > - Exits with code `0` on success

**Checkpoint**: Script created and readable. Verify locally with `python3 .github/scripts/extract-coverage.py` (requires a `coverage/coverage.cobertura.xml` to exist).

---

## Phase 3: User Story 1 — Coverage Collected Automatically on PR Runs (Priority: P1) 🎯 MVP

**Goal**: Every PR CI run produces `coverage/coverage.cobertura.xml` as a by-product of the test run. Coverage collection is automatic — no manual step needed.

**Independent Test** (from spec.md): Open a PR and confirm that a `coverage/coverage.cobertura.xml` file is produced as a step output in the CI run (visible in the step logs) without any additional developer action.

### Implementation for User Story 1

- [ ] T003 [US1] Add `permissions` block to `.github/workflows/ci.yml` at the workflow level (above `jobs:`)

  > Add the following block between the `on:` trigger and `jobs:` sections:
  > ```yaml
  > permissions:
  >   pull-requests: write
  >   contents: read
  > ```

- [ ] T004 [US1] Add `Restore dotnet tools` step to `.github/workflows/ci.yml` in the `build-and-test` job

  > Insert after the `Setup .NET` step and before the `Restore dependencies` step:
  > ```yaml
  >       - name: Restore dotnet tools
  >         run: dotnet tool restore
  > ```

- [ ] T005 [US1] Replace the `Test` step in `.github/workflows/ci.yml` with the coverage-wrapped `Test with coverage` step

  > Replace the existing step:
  > ```yaml
  >       - name: Test
  >         run: dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal
  > ```
  > With:
  > ```yaml
  >       - name: Test with coverage
  >         run: >
  >           dotnet tool run dotnet-coverage collect
  >           --settings coverage.settings.xml
  >           "dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal"
  >           -f cobertura
  >           -o coverage/coverage.cobertura.xml
  > ```
  > Note: the test still runs with all original flags; dotnet-coverage wraps it. The `--no-build` flag is preserved.

**Checkpoint**: Push to a PR branch. In the CI run, confirm the `Test with coverage` step completes and the step log shows both test results and a path to the generated XML. The `Build` status check must still pass.

---

## Phase 4: User Story 2 — Coverage Results Visible on Pull Requests (Priority: P1)

**Goal**: After the CI run, a coverage table comment appears on the PR showing per-file line and branch coverage for all files in `Game-Of-Life.Library/src/`.

**Independent Test** (from spec.md): Open a PR and confirm that a `### Coverage Report` comment appears in the PR comments section after the CI run completes, without any manual action.

### Implementation for User Story 2

- [ ] T006 [US2] Add `Extract coverage metrics` step to `.github/workflows/ci.yml` after the `Test with coverage` step

  > ```yaml
  >       - name: Extract coverage metrics
  >         run: python3 .github/scripts/extract-coverage.py > coverage-comment.md
  > ```

- [ ] T007 [US2] Add `Post coverage comment` step to `.github/workflows/ci.yml` after the `Extract coverage metrics` step

  > ```yaml
  >       - name: Post coverage comment
  >         env:
  >           GH_TOKEN: ${{ github.token }}
  >         run: |
  >           gh pr comment ${{ github.event.pull_request.number }} \
  >             --body-file coverage-comment.md
  > ```

**Checkpoint**: Push to a PR branch. After the CI run completes, verify a `### Coverage Report` markdown table comment appears in the PR comments section with rows for Cell.cs, Helper.cs, Life.cs, and Patterns.cs.

---

## Phase 5: User Story 3 — New Coverage Comment Added on Every CI Run (Priority: P2)

**Goal**: Confirm that each CI run adds a new PR comment rather than editing an earlier one — preserving per-commit coverage history.

**Independent Test** (from spec.md): Push two successive commits to a PR branch. After each CI run, confirm the PR has two separate `### Coverage Report` comments and the first has not been modified.

### Implementation for User Story 3

- [ ] T008 [US3] Verify that the `Post coverage comment` step in `.github/workflows/ci.yml` (T007) uses `gh pr comment` without any `--edit-last` or update flag, and add an inline YAML comment confirming the behaviour is intentional

  > Locate the `Post coverage comment` step and confirm the `gh pr comment` command has NO `--edit-last` flag. Add a comment line immediately above the `run:` key:
  > ```yaml
  >       - name: Post coverage comment
  >         # Always adds a new comment — intentional per spec 003 FR-004.
  >         # Each CI run appends a comment; earlier comments are preserved as history.
  >         env:
  >           GH_TOKEN: ${{ github.token }}
  >         run: |
  >           gh pr comment ${{ github.event.pull_request.number }} \
  >             --body-file coverage-comment.md
  > ```

**Checkpoint**: Two commits to the same PR produce two separate `### Coverage Report` comments; neither is edited after the fact.

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Keep the repository clean and confirm spec compliance end-to-end.

- [ ] T009 Add `coverage/` and `coverage-comment.md` to `.gitignore` (these are CI-generated transient files and must not be committed)

  > Check whether a root `.gitignore` already exists. If it does, append:
  > ```
  > # CI coverage artifacts (generated at runtime)
  > coverage/
  > coverage-comment.md
  > ```
  > If no `.gitignore` exists at the repo root, create one with the above entries.

- [ ] T010 Validate final `.github/workflows/ci.yml` against all functional requirements in `specs/003-ci-code-coverage/spec.md`

  > **Traceability checklist**:
  > - FR-001: Coverage collected automatically on every PR event → confirm `dotnet-coverage collect` step is present and no manual trigger required
  > - FR-002: Scope is `Game-Of-Life.Library/src/*.cs` only → confirm `--settings coverage.settings.xml` flag is on the collect command; verify `coverage.settings.xml` includes only `Game-Of-Life.Library.dll` module and `*/src/*` sources
  > - FR-003: XML extracted directly, no reportgenerator → confirm `reportgenerator` does not appear anywhere in ci.yml; confirm `extract-coverage.py` reads Cobertura XML directly
  > - FR-004: New comment per run, no editing earlier comments → confirm T008 check passed; confirm no `--edit-last` flag
  > - FR-005: Both line % and branch % reported → confirm `extract-coverage.py` outputs both columns
  > - FR-006: Below-100% shown as informational → confirm no `exit 1` in script for low coverage; confirm no `fail-fast` or status check set on coverage result
  > - FR-007: CI does not fail or block merge on coverage → confirm no step sets a failure condition based on coverage percentages
  > - SC-001–SC-004: all satisfied by correct implementation; SC-005 removed — no time constraint applies

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies — start immediately
- **Phase 2 (Foundational)**: Depends on Phase 1 (CIR amended) — T002 can start in parallel with T001 (different files) but CIR should be done first as a matter of process
- **Phase 3 (US1)**: Depends on Phase 2 (T002 complete) — ci.yml changes reference the script
- **Phase 4 (US2)**: Depends on Phase 3 (US1 complete) — coverage XML must exist before extract/comment steps make sense
- **Phase 5 (US3)**: Depends on Phase 4 (T007 complete) — verifies T007's behaviour
- **Phase N (Polish)**: Depends on all story phases complete

### User Story Dependencies

- **US1 (P1)**: No dependencies on US2 or US3
- **US2 (P1)**: Depends on US1 (coverage XML must be produced before it can be read)
- **US3 (P2)**: Depends on US2 (verifies the comment step from T007)

### Within Each Phase

- T003, T004, T005 modify the same file (ci.yml) — sequential
- T006, T007 modify the same file (ci.yml) — sequential, depend on T005
- T008 modifies the same file (ci.yml) — sequential, depends on T007
- T009 modifies `.gitignore` — independent of ci.yml tasks; can run anytime after T002

### Parallel Opportunities

- T001 (CIR) and T002 (Python script) are different files and can run in parallel
- T009 (gitignore) is independent of all ci.yml edits — can run in parallel with any phase

---

## Parallel Example: User Story 1

```bash
# T001 and T002 can run simultaneously (different files):
Task: "Amend docs/cir/004-ci-coverage-reporting.md"
Task: "Create .github/scripts/extract-coverage.py"

# T003, T004, T005 are sequential edits to ci.yml:
# T003 → T004 → T005
```

---

## Implementation Strategy

### MVP First (User Story 1 only)

1. Complete Phase 1: Amend CIR-004
2. Complete Phase 2 (T002): Create `extract-coverage.py`
3. Complete Phase 3 (T003–T005): Add permissions, tool restore, coverage-wrapped test step
4. **STOP and VALIDATE**: Push to a PR — confirm CI run produces `coverage.cobertura.xml`
5. Continue to Phase 4 (US2) once collection is confirmed working

### Incremental Delivery

1. Phase 1 + 2 → Pre-flight complete
2. Phase 3 (US1) → Test run produces coverage XML ← validate here
3. Phase 4 (US2) → Coverage table appears on PR ← validate here
4. Phase 5 (US3) → Second commit confirms new-comment behaviour ← validate here
5. Phase N → Clean up and final compliance check

---

## Notes

- All ci.yml edits modify the single file `.github/workflows/ci.yml` — conflicts are unlikely since spec 002 and 003 are on the same branch, but stage carefully.
- `dotnet tool restore` (T004) is a new step not present in spec 002's ci.yml — this must be added or `dotnet tool run dotnet-coverage` will fail with a "tool not found" error.
- The `extract-coverage.py` script must handle `branches_valid == 0` gracefully (display `N/A`) — see `specs/003-ci-code-coverage/data-model.md` Invariants section.
- `GH_TOKEN` uses the built-in `${{ github.token }}` — no additional repository secrets are needed.
- The `coverage.settings.xml` file at the repo root already correctly scopes coverage to `Game-Of-Life.Library.dll` and `*/src/*` — do not modify it.
