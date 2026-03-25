# Tasks: Core CI — Automated Build and Test

**Input**: Design documents from `/specs/002-ci-build-test/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

**Tests**: Not applicable — this feature delivers a GitHub Actions YAML workflow, not application code with unit-testable logic.

**Organization**: One user story (US1: PR Merge Protection). One output file. Tasks are sequenced for safe, verifiable delivery.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to

---

## Phase 1: Setup (Pre-flight)

**Purpose**: Record the non-obvious platform decision before writing any code — the constitution requires a CIR for decisions that depart from the original spec intent.

- [X] T001 Write CIR-005 for the Linux-only CI platform decision in `docs/cir/005-linux-only-ci.md`

  > **CIR content guidance** (Intent, Behaviour, Constraints, Decisions, Date):
  > - **Intent**: Choose the runner platform(s) for the CI workflow
  > - **Behaviour**: CI runs only on `ubuntu-latest`; no Windows runner; no matrix
  > - **Constraints**: Cost, simplicity, sufficient for a platform-agnostic .NET library
  > - **Decisions**: Cross-platform matrix (original spec input) rejected — the library is platform-agnostic (.NET 10), and the test suite is deterministic; Linux-only provides equivalent validation at half the cost and complexity. User explicitly confirmed this in spec clarifications (2026-03-25).
  > - **Date**: 2026-03-25

**Checkpoint**: CIR written — constitution pre-flight complete, implementation can proceed.

---

## Phase 3: User Story 1 — PR Merge Protection (Priority: P1) 🎯 MVP

**Goal**: A pull request targeting `master` automatically triggers a build-and-test run on Linux. The result appears as a named status check (`build-and-test`) that can be required by branch protection rules, preventing merge of broken code.

**Independent Test** (from spec.md): Open a PR with a deliberate test failure → confirm the `build-and-test` check fails and the merge button is disabled. Fix the failure and push again → confirm the check re-runs automatically and the PR becomes mergeable.

### Implementation for User Story 1

- [X] T002 [US1] Create `.github/workflows/ci.yml` implementing the complete workflow per `specs/002-ci-build-test/contracts/workflow-schema.md`

  > **Required content** (do not deviate without updating contracts/):
  > - `name: CI`
  > - Trigger: `pull_request`, `branches: [master]`, `types: [opened, synchronize, reopened]` — **no** `push` trigger
  > - Single job: key `build-and-test`, `runs-on: ubuntu-latest`
  > - Step 1: `actions/checkout@v4`
  > - Step 2: `actions/setup-dotnet@v5` with `dotnet-version: '10.x'`
  > - Step 3: `dotnet restore`
  > - Step 4: `dotnet build --no-restore --configuration Release`
  > - Step 5: `dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal`

- [X] T003 [US1] Verify `.github/workflows/ci.yml` satisfies all functional requirements by tracing each FR in `specs/002-ci-build-test/spec.md` against the workflow:

  > **Traceability checklist**:
  > - FR-001: Trigger fires on PR opened/synchronised targeting master → confirm `pull_request.branches: [master]` and `types: [opened, synchronize, reopened]`
  > - FR-002: Runs on Linux → confirm `runs-on: ubuntu-latest`
  > - FR-003: Named check visible on PR → confirm job key is `build-and-test`
  > - FR-004: Pass/fail/skip counts reported → confirm `--verbosity normal` on test step
  > - FR-005: Merge blocked on failure → confirm no `continue-on-error` on any step; note branch protection rule must be configured separately
  > - FR-006: Re-evaluated on new commits → confirm `synchronize` event type is included
  > - FR-007: Completes within 10 minutes → no action needed in YAML; monitor on first run

**Checkpoint**: `ci.yml` is written and requirements are verified. Push to a PR branch to trigger a live run.

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Post-implementation hygiene and branch protection activation.

- [X] T004 Confirm branch protection is configured (or documented as pending) — after merging `ci.yml` to `master`, navigate to **Settings → Branches → master** and add `build-and-test` as a required status check per `specs/002-ci-build-test/quickstart.md`

  > This step is a manual repository settings action, not a code change. It cannot be automated in the workflow file. The merge gate (FR-005) is not enforced until this is done.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies — start immediately
- **Phase 3 (US1)**: Depends on Phase 1 (CIR written) — constitutional pre-flight required
- **Phase N (Polish)**: Depends on Phase 3 completion and `ci.yml` merged to `master`

### User Story Dependencies

- **User Story 1 (P1)**: No dependencies on other user stories — only story in this feature

### Within User Story 1

- T002 (create workflow) before T003 (verify)
- T003 before T004 (post-merge configuration)

### Parallel Opportunities

- Only T002 and T001 could theoretically run in parallel (different files), but T001 first is preferred per constitution principle (CIR before implementation).

---

## Parallel Example: User Story 1

```bash
# No parallelism within US1 — tasks are inherently sequential:
# T001 → T002 → T003 → (merge) → T004
```

---

## Implementation Strategy

### MVP First (Single Story)

1. Complete Phase 1: Write CIR-005
2. Complete Phase 3 (T002): Create `ci.yml`
3. Complete Phase 3 (T003): Verify against requirements
4. **STOP and VALIDATE**: Open a test PR and confirm the `build-and-test` check appears
5. Merge `ci.yml` to `master`
6. Complete Phase N (T004): Configure branch protection

### Full Feature Delivery

This feature has only one user story. MVP = full feature.

---

## Notes

- The sole code output is `.github/workflows/ci.yml` — no application source files are modified.
- The job name `build-and-test` must match exactly what is entered in branch protection rules. Do not rename it without updating the branch protection rule.
- `--configuration Release` on both build and test steps is deliberate — see `specs/002-ci-build-test/research.md`.
- No third-party test reporter action is used — `--verbosity normal` provides sufficient test count output in the step logs.
- Branch protection configuration (T004) is the only step that cannot be performed via a commit and requires manual action in GitHub repository settings.
