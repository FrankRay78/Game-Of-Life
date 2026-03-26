# Feature Specification: Structured PR Raise Command

**Feature Branch**: `004-raise-pr-command`
**Created**: 2026-03-26
**Status**: Draft
**Input**: User description: "A structured way to raise a new PR ensuring the PR has the right details for Claude to review, auto-detecting newly created specs and CIRs on the branch, implemented as a custom slash command."

## Clarifications

### Session 2026-03-26

- Q: Should the PR be created immediately without showing a draft or asking for confirmation first? → A: Yes — the command creates the PR directly without any confirmation step.
- Q: Should the command remind the developer to tag @claude for a code review after PR creation? → A: No — no reminder should be shown.
- Q: What should the command output after creating the PR? → A: PR URL plus a one-line summary of detected specs and CIRs.
- Q: Should the slash command be self-contained with full instructions, or thin and referential? → A: Thin — reference `CLAUDE.md` and related convention files by name rather than duplicating their contents; rely on Claude's built-in PR creation knowledge for anything not project-specific.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Raise a PR with Auto-Detected Artifacts (Priority: P1)

A developer on a feature branch runs a single slash command. The command inspects the branch, detects any new spec folders and CIR files introduced since master, builds a complete PR description pre-filled with that information, and creates the PR immediately without requiring confirmation.

**Why this priority**: This is the core value of the feature — eliminating manual PR description assembly and ensuring specs and CIRs are always surfaced to the reviewer.

**Independent Test**: Can be tested by checking out any feature branch that contains new spec or CIR files, running `/raise-pr`, and verifying the created PR contains correct linked references to those artifacts.

**Acceptance Scenarios**:

1. **Given** the developer is on a feature branch with one new spec folder and one new CIR, **When** they run `/raise-pr`, **Then** the command creates a PR whose body contains linked references to both artifacts and outputs the PR URL.
2. **Given** the developer is on a feature branch with no new specs or CIRs, **When** they run `/raise-pr`, **Then** the PR body shows the New Artifacts section with unticked checkboxes and the PR URL is returned.

---

### User Story 2 - GitHub PR Form Uses the Standard Template (Priority: P2)

When any PR is opened against master through the GitHub UI or CLI, a structured template is automatically injected into the description field, prompting the author to fill in Summary, Spec, New Artifacts, Claude Review Notes, and a Checklist.

**Why this priority**: Even when `/raise-pr` is not used, the GitHub template ensures all PRs follow the same structure. It also provides the "Claude Review Notes" field that Claude reads when reviewing.

**Independent Test**: Can be tested by opening a new PR against master via the GitHub UI and verifying the description field is pre-populated with the template sections.

**Acceptance Scenarios**:

1. **Given** a developer opens a PR against master via GitHub UI, **When** the PR creation form loads, **Then** the description is pre-filled with the standard template (Summary, Spec, New Artifacts, Claude Review Notes, Checklist sections).
2. **Given** a developer opens a PR via `gh pr create` without a `--body` argument, **When** GitHub renders the PR, **Then** the template sections are present in the description.

---

### User Story 3 - Claude Reviews PRs with Project-Specific Guidance (Priority: P3)

When a developer triggers `@claude review` on a PR, Claude applies project-specific review rules (key invariants, naming conventions, what to skip) before returning findings, without needing those rules to be repeated in the PR description.

**Why this priority**: This reduces repetitive context-setting in every PR and ensures consistent, high-quality reviews aligned to project standards.

**Independent Test**: Can be tested by triggering `@claude review` on a PR and verifying that Claude's response references the grid invariant or test naming rules from the project guidance, without those rules having been mentioned in the PR body.

**Acceptance Scenarios**:

1. **Given** a `REVIEW.md` exists at the repo root, **When** Claude reviews a PR, **Then** Claude applies the Always Check, Skip, and prioritisation rules defined in that file.
2. **Given** the PR description contains a "Claude Review Notes" section with focus instructions, **When** Claude reviews the PR, **Then** Claude honours those instructions (e.g. focusing on a specific file or skipping test boilerplate).

---

### Edge Cases

- What happens when the command is run on the `master` branch? The command should abort immediately with a clear message.
- What happens when no commits exist between the current branch and master? The command should produce an empty summary section and still create the PR.
- What happens if `gh` CLI is not authenticated? The command should surface the error from `gh pr create` clearly.
- What happens if a PR already exists for the current branch? `gh pr create` will fail; the command should catch and report this gracefully.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The slash command MUST detect the current branch name and abort with a helpful message if run on `master`.
- **FR-002**: The slash command MUST identify files added to the branch (not present on master) and extract new spec folders (`specs/*/`) and CIR files (`docs/cir/*.md`).
- **FR-003**: The slash command MUST infer a PR title from the branch name by stripping any leading number prefix, replacing hyphens with spaces, and applying title case.
- **FR-004**: The slash command MUST generate a PR body by providing Claude with the detected artifacts (branch name, spec folders, CIR files) and directing it to populate the template sections; the command itself MUST NOT embed the template or convention content.
- **FR-005**: The slash command MUST create the PR immediately without displaying a draft or requesting confirmation, then output the PR URL followed by a one-line summary of the artifacts detected (e.g. `Detected: specs/004-raise-pr-command, docs/cir/006-slug.md`).
- **FR-006**: A GitHub PR template MUST be present at `.github/pull_request_template.md` so that all PRs opened against master are pre-filled with the standard structure.
- **FR-007**: A `REVIEW.md` file MUST exist at the repo root encoding project-specific review rules for Claude (conventions to always check, key invariants, files to skip).
- **FR-008**: `REVIEW.md` MUST instruct Claude to read and honour the "Claude Review Notes" section of the PR description when present.
- **FR-009**: The slash command MUST be concise — it MUST reference `CLAUDE.md`, `docs/conventions/general-principles.md`, and `REVIEW.md` by name for project context rather than reproducing their contents inline.

### Key Entities

- **PR Template**: The structured GitHub PR description template with sections for Summary, Spec, New Artifacts, Claude Review Notes, and Checklist.
- **Slash Command**: A concise `.claude/commands/raise-pr.md` file that Claude Code executes when the developer invokes `/raise-pr`. It provides only project-specific context (artifact detection logic, file references) and delegates PR body generation to Claude's built-in knowledge.
- **REVIEW.md**: A repo-root guidance file read by Claude during PR review, encoding project conventions and invariants.
- **Spec Folder**: A directory under `specs/NNN-slug/` containing `spec.md`, `plan.md`, and `tasks.md` for a tracked feature.
- **CIR File**: A Change Intent Record document under `docs/cir/NNN-slug.md` documenting a non-obvious decision.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A developer can raise a fully-structured PR from a feature branch in a single command invocation, without manually writing the description.
- **SC-002**: All PRs opened against master via GitHub UI automatically display the structured template sections, requiring zero additional setup per PR.
- **SC-003**: Every new spec folder and CIR file added on a branch is correctly identified and linked in the PR description without manual intervention.
- **SC-004**: Claude reviews reference project-specific conventions (test naming, grid invariant) without those rules needing to be included in the PR description.

## Assumptions

- The developer has `gh` CLI installed and authenticated.
- The repository uses `master` as the base branch for all PRs.
- Spec folders always contain at least a `spec.md` file; this is used as the detection signal.
- Branch names follow the pattern `NNN-slug-words` or `slug-words`; both are handled by the title inference logic.
