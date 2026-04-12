# Feature Specification: Code Coverage in CI

**Feature Branch**: `003-ci-code-coverage`
**Created**: 2026-03-24
**Status**: Draft
**Input**: User description: "Code Coverage in CI - Extend the CI workflow to collect code coverage during the test run and report results. Uses the existing dotnet-coverage and reportgenerator local tools already configured in the project. Coverage results should be visible on pull requests. The project targets 100% line and branch coverage on core library files (Cell.cs, Helper.cs, Life.cs), with Program.cs excluded from coverage measurement."

## Clarifications

### Session 2026-03-25

- Q: What is the coverage scope? → A: All `.cs` files under `Game-Of-Life.Library/src/` — Program.cs is not in scope (it is in a separate project entirely).
- Q: Should reportgenerator be used to produce the coverage report? → A: No — extract metrics directly from the XML output produced by dotnet-coverage; do not use reportgenerator.
- Q: When a second commit is pushed to a PR, should the existing coverage comment be updated or a new one added? → A: Add a new comment for each CI run; do not modify earlier comments.
- Q: Should coverage CI also run on every branch push, or only on PR events (aligned with spec 002)? → A: PR events only — no coverage run on arbitrary branch pushes.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Coverage Collected Automatically on PR Runs (Priority: P1)

When the CI test run executes on a pull request, coverage data is automatically collected at the same time. No separate trigger or manual step is required. A developer who opens or updates a PR can trust that coverage has been measured as part of the standard validation cycle.

**Why this priority**: Coverage collection must be seamlessly integrated into the existing test run — it has no value if it requires a manual step or a separate workflow invocation.

**Independent Test**: Can be fully tested by opening a PR and confirming that a coverage report is produced as an output of the CI run without any additional action.

**Acceptance Scenarios**:

1. **Given** a test run is triggered by a PR event, **When** the run completes, **Then** a coverage report has been produced covering line and branch coverage for all files in `Game-Of-Life.Library/src/`.
2. **Given** the coverage run completes, **When** a developer views the CI run, **Then** they can access the coverage report without running anything locally.

---

### User Story 2 - Coverage Results Visible on Pull Requests (Priority: P1)

When a contributor submits a pull request, coverage information is surfaced directly in the PR view. The contributor and reviewer can see the coverage impact of the change without navigating away from the PR.

**Why this priority**: Coverage data that is buried in logs or only available as a download provides little practical value during code review. The goal is for coverage to be a natural part of the review conversation.

**Independent Test**: Can be fully tested by opening a PR and confirming that coverage results are visible within the PR without additional steps.

**Acceptance Scenarios**:

1. **Given** a pull request is opened or updated, **When** the CI run completes, **Then** coverage results are surfaced in the PR in a way that is immediately visible to contributors and reviewers.
2. **Given** a change reduces coverage below the target threshold, **When** the CI run completes, **Then** the coverage comment on the PR reflects the drop as informational — the PR is not blocked from merging.
3. **Given** a change maintains or improves coverage, **When** the CI run completes, **Then** this is reflected positively in the coverage result shown on the PR.

---

### User Story 3 - New Coverage Comment Added on Every CI Run (Priority: P2)

The CI system posts a new coverage comment on the pull request each time a CI run completes. Each comment reflects the current state of line and branch coverage for that run. Earlier comments remain in place, providing a visible history of how coverage changed across commits.

**Why this priority**: Appending a new comment per run gives reviewers a clear, auditable trail of how coverage evolved over the lifetime of the PR without overwriting prior results.

**Independent Test**: Can be fully tested by pushing two successive commits to a PR and confirming a new coverage comment is added after each run, with the earlier comment still present.

**Acceptance Scenarios**:

1. **Given** a PR has reduced line or branch coverage below 100%, **When** the CI run completes, **Then** the new PR comment clearly indicates the shortfall with specific percentages — but the PR remains mergeable.
2. **Given** all core library files have 100% line and branch coverage, **When** the CI run completes, **Then** the new PR comment confirms the target is met.
3. **Given** a second commit is pushed to the same PR, **When** the new CI run completes, **Then** a new coverage comment is posted; the comment from the previous run is not modified or deleted.

---

### Edge Cases

- What happens when a new source file is added to `Game-Of-Life.Library/src/` without any tests — is it automatically included in coverage measurement?
- What happens if the coverage tooling fails to produce an XML report — is the CI run treated as failed, or does it continue with a warning?
- What happens when coverage is exactly at 100% and a refactor moves code between files — does coverage remain stable across file renames?

## Requirements *(mandatory)*

### PR Comment Format

The coverage comment posted on each pull request run MUST follow this layout:

**### Coverage Report**

| File | Lines | Line % | Branches | Branch % |
|------|-------|--------|----------|----------|
| Cell.cs | 45 | 100% | 12 | 100% |
| Helper.cs | 38 | 100% | 8 | 100% |
| Life.cs | 62 | 100% | 18 | 100% |
| Patterns.cs | 20 | 100% | 4 | 100% |

*(Line and branch counts reflect totals in scope; percentages reflect coverage achieved.)*

### Functional Requirements

- **FR-001**: The CI system MUST collect line and branch coverage data automatically as part of the test run on every PR event, with no additional manual trigger required.
- **FR-002**: Coverage measurement MUST include all `.cs` files under `Game-Of-Life.Library/src/` and MUST NOT include any files from outside that path.
- **FR-003**: Coverage metrics MUST be extracted directly from the XML output file produced by dotnet-coverage; reportgenerator MUST NOT be used.
- **FR-004**: The CI system MUST post a new automated comment on the pull request each time a CI run completes, containing the current line and branch coverage percentages. Earlier comments from prior runs MUST NOT be modified or deleted.
- **FR-005**: The CI system MUST report both line coverage percentage and branch coverage percentage as distinct metrics within the PR comment.
- **FR-006**: The PR comment MUST clearly indicate when measured coverage is below 100% for any source file, displayed as informational — coverage shortfalls do not block the PR from being merged.
- **FR-007**: The CI system MUST NOT fail or block a pull request solely on the basis of a coverage result.

### Assumptions

- Coverage collection runs on Linux only, consistent with the build-and-test workflow defined in spec 002.
- Coverage is triggered only by PR events (opened, synchronised, reopened) targeting master (the `synchronize` and `reopened` GitHub event types) — not on arbitrary branch pushes.
- If the coverage tooling fails to produce an XML report, the CI step MUST fail visibly — it must not silently post an empty or malformed comment.
- Coverage metrics are extracted directly from the XML output produced by dotnet-coverage; reportgenerator is not required.
- "Core library files" refers to all `.cs` files in `Game-Of-Life.Library/src/` — any new source files added there are automatically in scope without configuration changes.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Coverage data is produced on 100% of CI runs that include a test execution step — no run completes without a coverage report.
- **SC-002**: Coverage scope is confirmed as all files in `Game-Of-Life.Library/src/` — adding or renaming a file there is automatically reflected in the coverage report with no configuration change.
- **SC-003**: A drop below 100% line or branch coverage on any in-scope file is surfaced clearly on every affected PR.
- **SC-004**: Contributors can view coverage results on a PR without downloading files or navigating outside the PR view.
