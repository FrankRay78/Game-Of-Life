# Tasks: Structured PR Raise Command

**Input**: Design documents from `/specs/004-raise-pr-command/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, contracts/raise-pr-command.md ✅

**Tests**: No automated tests — feature is pure configuration; each story verified manually per its Independent Test criteria.

**Organization**: Tasks grouped by user story. US2 deliverable (PR template) is in the Foundational phase because it is a blocking dependency for US1 (the slash command references its structure). US2's Phase 4 entry is a verification checkpoint only.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to

---

## Phase 1: Setup

No project initialization required. All target directories (`.github/`, `.claude/commands/`, repo root) already exist.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Create the PR template that is both the US2 deliverable and the structure the slash command (US1) uses to build PR bodies.

**⚠️ CRITICAL**: US1 (slash command) must not be implemented until this phase is complete.

- [x] T001 Create `.github/pull_request_template.md` with five sections: Summary (with HTML comment guidance), Spec (link to spec folder or N/A), New Artifacts (two unticked checkboxes for spec folder and CIR), Claude Review Notes (HTML comment with example hints), and Checklist (CI passes, tests named correctly, CIR written, CLAUDE.md updated if needed)

**Checkpoint**: PR template exists — US1 and US2 implementation can now proceed.

---

## Phase 3: User Story 1 — Raise a PR with Auto-Detected Artifacts (Priority: P1) 🎯 MVP

**Goal**: Developer runs `/raise-pr` on a feature branch; PR is created immediately with spec/CIR links pre-filled and a detection summary output.

**Independent Test**: Check out any feature branch with at least one new spec folder or CIR file. Run `/raise-pr`. Verify: (a) the created PR body contains linked references to the detected artifacts, (b) the PR URL is output, (c) a `Detected:` summary line is shown, (d) no confirmation prompt was presented.

- [x] T002 [US1] Create `.claude/commands/raise-pr.md` — concise YAML-frontmatter command implementing the 8 steps per `specs/004-raise-pr-command/contracts/raise-pr-command.md`: branch guard (abort if `master`), added-file detection, spec/CIR extraction, commit list, title inference, body construction using `.github/pull_request_template.md` structure, `gh pr create`, and URL + detection output. Zero-commit guard: if `git log master..HEAD` returns no commits, abort with message "No commits found on this branch compared to master — nothing to raise a PR for." Preamble must reference `CLAUDE.md` and `docs/conventions/general-principles.md` by name; must not reproduce their content inline.
- [x] T003 [P] [US1] Update `.claude/settings.json`: add `"Bash(git rev-parse:*)"`, `"Bash(git diff:*)"`, `"Bash(git log:*)"`, and `"Bash(gh pr create:*)"` to the `allow` array

**Checkpoint**: Run `/raise-pr` on `004-raise-pr-command` branch to validate US1 end-to-end.

---

## Phase 4: User Story 2 — GitHub PR Form Uses Standard Template (Priority: P2)

**Goal**: All PRs opened against master via GitHub UI are automatically pre-filled with the template sections.

**Independent Test**: After this branch is merged to master, open a new PR against master via GitHub UI. Verify the description field is pre-populated with all five template sections.

**Note**: GitHub reads the PR template from the base branch (master). The template takes effect only once this branch is merged. The implementation task was completed in Phase 2 (T001). This phase validates correctness.

- [x] T004 [US2] Review `.github/pull_request_template.md` content against the plan.md Phase 1 design: confirm all five sections are present, HTML comments are author-facing guidance (not rendered), and the file contains no implementation details or content duplicated from `CLAUDE.md`

**Checkpoint**: Template content verified. GitHub injection will be active after merge to master.

---

## Phase 5: User Story 3 — Claude Reviews PRs with Project-Specific Guidance (Priority: P3)

**Goal**: `REVIEW.md` at repo root encodes project-specific rules that Claude reads when reviewing any PR.

**Independent Test**: With `REVIEW.md` in place, trigger `@claude review` on any PR. Verify Claude's response references the grid invariant (`grid[x, y]`) or test naming pattern (`MethodName_Scenario_ExpectedResult`) without those rules having been stated in the PR body.

- [x] T005 [US3] Create `REVIEW.md` at repo root with three sections: How to Prioritise (read Claude Review Notes first, then spec link if present, then invariant checks, then conventions), Skip Unconditionally (`TestResults/`, `coverage/`, `bin/`, `obj/`, generated files), Reference Documents (links to `CLAUDE.md` and `docs/conventions/csharp.md`). Must not reproduce convention or invariant content inline — reference files by name only.

**Checkpoint**: REVIEW.md present with exactly three sections, no inlined convention content.

---

## Phase 6: Polish & Cross-Cutting Concerns

- [x] T006 Write CIR at `docs/cir/006-raise-pr-no-confirmation.md` documenting the no-confirmation design decision per the CIR template in `docs/conventions/general-principles.md`. Required by Constitution Principle V before this branch is merged.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Foundational (Phase 2)**: No dependencies — start immediately
- **US1 (Phase 3)**: Depends on Phase 2 (T001 must be complete before T002)
- **US2 (Phase 4)**: Depends on Phase 2 (T001) — verification only, can run after T001
- **US3 (Phase 5)**: No dependencies on other phases — fully independent
- **Polish (Phase 6)**: Should be done before merge, no code dependencies

### Parallel Opportunities

- T003 [P] can run in parallel with T002 (different files)
- T004, T005, T006 can all run in parallel once T001 and T002 are complete
- US3 (T005) can be started at any time — fully independent

---

## Parallel Example: US1

```text
After T001 completes:
  → T002  Create .claude/commands/raise-pr.md
  → T003  Update .claude/settings.json  [runs in parallel with T002]
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 2: Create PR template (T001)
2. Complete Phase 3: Create slash command + settings (T002, T003)
3. **STOP and VALIDATE**: Run `/raise-pr` on `004-raise-pr-command` — verify US1 end-to-end
4. Proceed to US2 verification, US3, and Polish

### Incremental Delivery

1. T001 → Foundation ready (PR template exists)
2. T002 + T003 → US1 complete (slash command works)
3. T004 → US2 verified (template content confirmed)
4. T005 → US3 complete (REVIEW.md in place)
5. T006 → CIR written, ready to merge

---

## Notes

- All tasks produce markdown/config files — no compilation or automated test runs required
- Manual validation steps are described in each story's Independent Test
- US2 GitHub injection can only be tested after merging to master (by design — GitHub reads template from base branch)
- Commit after each task or logical group before raising the PR
